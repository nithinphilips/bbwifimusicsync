PRODUCT       = "bbwifimusicsync"
PRODUCT_LONG  = "Wireless Music Sync"
DESCRIPTION   = "An Application to wirelessly Sync iTunes playlists."
VERSION       = "0.3.1" # We omit the build segment of the version number.
AUTHORS       = "Nithin Philips"
COPYRIGHT     = "(c) 2011 #{AUTHORS}"
PROJECT_URL   = "https://sourceforge.net/projects/bbwifimusicsync/"
TRADEMARKS    = "BlackBerry(r) is a registered trademark of Research in Motion"

NSIS_PATH     = "C:/Program Files (x86)/NSIS/makensis.exe"


CONFIGURATION = "Release"
BUILD_DIR     = File.expand_path("build")
OUTPUT_DIR    = "#{BUILD_DIR}/out"
BIN_DIR       = "#{BUILD_DIR}/bin"
SRC_DIR       = "#{BUILD_DIR}/src"
NUGET_DIR     = "#{BUILD_DIR}/nug"
PACKAGES_DIR  = "packages"

PACKAGE       = "#{PRODUCT}-#{VERSION}"
BIN_PACKAGE   = "#{PACKAGE}-bin"
SRC_PACKAGE   = "#{PACKAGE}-src"
INS_PACKAGE   = "#{PACKAGE}-setup"

require 'albacore'
FileList["./albacore/*.rb"].each { |f| require f }
require 'rgl/dot'
require 'rgl/implicit'
require 'zip/zip'
require 'zip/zipfilesystem'



task :default => [:dist]

task :dist => [:clean, :dist_zip, :dist_src, :installer, :test]

task :dist_with_log => [:clean, :copylogconfig, :dist_zip, :dist_src, :installer, :test]

#Albacore.configure do |config|
#  config.rapcpath = ""
#end

desc "Compile"
msbuild :compile  => :assemblyinfo do |msb|
    msb.properties :configuration => CONFIGURATION, "OutputPath" => OUTPUT_DIR
    msb.targets :Clean, :Build
    msb.solution = "WifiMusicSync.sln"
    msb.verbosity = "quiet"
end

desc "Moving binaries"
task :build => :compile  do
    FileUtils.rm_rf BIN_DIR
    Dir.mkdir("#{BIN_DIR}")
    Dir.mkdir("#{BIN_DIR}/#{PACKAGE}")
    Dir.mkdir("#{BIN_DIR}/#{PACKAGE}/app")
    Dir.mkdir("#{BIN_DIR}/#{PACKAGE}/MusicSync.BlackBerryApp")

    binaries = FileList["#{OUTPUT_DIR}/*.dll", "#{OUTPUT_DIR}/*.exe", "#{OUTPUT_DIR}/*.exe.config", "#{OUTPUT_DIR}/*.dll.config", "README.md", "COPYING.txt"]
    .exclude(/Backup/)
    .exclude(/Afterthought.Amender.exe/)
    .exclude(/KayakExamples.exe/)

    FileUtils.cp_r binaries, "#{BIN_DIR}/#{PACKAGE}/"

    binaries = FileList["./BlackberryClient/WifiMusicSync/deliverables/Web/**"]
    FileUtils.cp_r binaries, "#{BIN_DIR}/#{PACKAGE}/app"

    binaries = FileList["./BlackberryClient/WifiMusicSync/deliverables/Standard/**"]
    FileUtils.cp_r binaries, "#{BIN_DIR}/#{PACKAGE}/MusicSync.BlackBerryApp"
end

desc "Copy log conifg files to the bin dir"
task :copylogconfig => :build  do
    files = FileList["#{OUTPUT_DIR}/*.log.config"]
    FileUtils.cp_r files, "#{BIN_DIR}/#{PACKAGE}/"
end

desc "Packaging source"
task :dist_src do |z|

    gitModules = [
        {"dir" => ".",                "prefix" => "#{PACKAGE}" },
        {"dir" => "lib/Afterthought", "prefix" => "#{PACKAGE}/lib/Afterthought"}
    ]

    workingdir = Dir.pwd

    FileUtils.rm_rf "#{BUILD_DIR}/src"
    gitModules.each { |m|
        prefix = m["prefix"]
        filename = "#{BUILD_DIR}/src-temp.zip"

        Dir.chdir(m["dir"])
        sh "git archive HEAD --format=zip -9 --prefix=\"#{prefix}/\" > \"#{filename}\""
        Dir.chdir(workingdir)

        extract_zip(filename, "#{BUILD_DIR}/src")
        FileUtils.rm_rf filename
    }

    FileUtils.rm_rf "#{BUILD_DIR}/#{SRC_PACKAGE}.zip"
    zip_dir("#{BUILD_DIR}/src", "#{BUILD_DIR}/#{SRC_PACKAGE}.zip")
end

def zip_dir(dir, file)
    path = File.expand_path(dir)
    Zip::ZipFile.open(file, Zip::ZipFile::CREATE) do |zipfile|
        Dir["#{path}/**/**"].each do |file|
            zipfile.add(file.sub(path + '/',''),file)
        end
    end
end

def extract_zip(file, dest)
    Zip::ZipFile.open(file) { |zip_file|
        zip_file.each { |f|
            f_path=File.join(dest, f.name)
            FileUtils.mkdir_p(File.dirname(f_path))
            zip_file.extract(f, f_path) unless File.exist?(f_path)
        }
    }
end

desc "Packaging binaries"
zip :dist_zip => [:build] do |z|
    z.directories_to_zip BIN_DIR
    z.output_file = "#{BIN_PACKAGE}.zip"
    z.output_path = BUILD_DIR
end

desc "Updating installer file list"
nsisfilelist :installerfiles => [:build] do |n|
    #nsisfilelist :installerfiles do |n|
    n.dirs << File.expand_path("#{BIN_DIR}/#{PACKAGE}/")
    n.add_files_list = File.expand_path("MusicSync.Installer/files_ADD.nsi")
    n.remove_files_list = File.expand_path("MusicSync.Installer/files_REM.nsi")
end

desc "Building installer"
exec :installer => [:installerfiles] do |exec|
    exec.command = NSIS_PATH
    exec.parameters = [
        "/V3",
        "/DPRODUCT_VERSION=#{VERSION}",
        "/DOUT_FILE=#{BUILD_DIR}/#{INS_PACKAGE}.exe",
        File.expand_path("MusicSync.Installer/Installer.nsi")]
end

mstest :test => [:compile] do |test|
    test.command = "C:/Program Files (x86)/Microsoft Visual Studio 10.0/Common7/IDE/mstest.exe"
    test.assemblies "#{OUTPUT_DIR}/MusicSync.Tests.dll"
end

def ensure_submodules()
    system("git submodule init")
    system("git submodule update")
end

task :clean do
    FileUtils.rm_rf BUILD_DIR
end

rapc :build_bb do |r|
    r.output = PRODUCT
end

rapcmanifest :rapcmanifest do |m|
    m.output_file = "file.rapc"
    m.name = PRODUCT
    m.title = PRODUCT_LONG
    m.version = "1.0.0"
    m.vendor = AUTHORS
    m.description = DESCRIPTION
    m.type = "CLDC"
    m.icons << "music-sync-68.png"
    m.focus_icons << "music-sync-glow-68.png"
end

task :assemblyinfo => [:libasminfo, :testsasminfo, :serverasminfo, :desktopasminfo, :configuratorasminfo]

assemblyinfo :libasminfo do |a|
    a.title        = "libMusicSync"
    a.description  = "A supporting library for wirelessly syncing music to BlackBerry phones"
    a.output_file  = "libMusicSync/Properties/AssemblyInfo.cs"

    a.product_name = PRODUCT_LONG
    a.version      = VERSION
    a.file_version = VERSION
    a.copyright    = COPYRIGHT
    a.company_name = AUTHORS
    a.trademark    = TRADEMARKS
    a.namespaces "System.Runtime.CompilerServices"
end

assemblyinfo :testsasminfo do |a|
    a.title        = "MusicSync.Tests"
    a.description  = "A set of tests for Wireless Music Sync features"
    a.output_file  = "MusicSync.Tests/Properties/AssemblyInfo.cs"

    a.product_name = PRODUCT_LONG
    a.version      = VERSION
    a.file_version = VERSION
    a.copyright    = COPYRIGHT
    a.company_name = AUTHORS
    a.trademark    = TRADEMARKS
    a.namespaces "System.Runtime.CompilerServices"
end

assemblyinfo :serverasminfo do |a|
    a.title        = "MusicSync.Server"
    a.description  = "A server for wirelessly syncing music to BlackBerry phones"
    a.output_file  = "MusicSync.Server/Properties/AssemblyInfo.cs"

    a.product_name = PRODUCT_LONG
    a.version      = VERSION
    a.file_version = VERSION
    a.copyright    = COPYRIGHT
    a.company_name = AUTHORS
    a.trademark    = TRADEMARKS
    a.namespaces "System.Runtime.CompilerServices"
end

assemblyinfo :desktopasminfo do |a|
    a.title        = "MusicSync.Desktop"
    a.description  = "A desktop client for syncing music to BlackBerry phones"
    a.output_file  = "MusicSync.Desktop/Properties/AssemblyInfo.cs"

    a.product_name = PRODUCT_LONG
    a.version      = VERSION
    a.file_version = VERSION
    a.copyright    = COPYRIGHT
    a.company_name = AUTHORS
    a.trademark    = TRADEMARKS
    a.namespaces "System.Runtime.CompilerServices"
end

assemblyinfo :configuratorasminfo do |a|
    a.title        = "MusicSync.Configurator"
    a.description  = "A tool to configure Wireless Music Sync"
    a.output_file  = "MusicSync.Configurator/Properties/AssemblyInfo.cs"

    a.product_name = PRODUCT_LONG
    a.version      = VERSION
    a.file_version = VERSION
    a.copyright    = COPYRIGHT
    a.company_name = AUTHORS
    a.trademark    = TRADEMARKS
    a.namespaces "System.Runtime.CompilerServices"
end

desc "Generate a graph of all the tasks and their relationship"
task :dep_graph do |task|
    this_task = task.name
    dep = RGL::ImplicitGraph.new { |g|
        # vertices of the graph are all defined tasks without this task
        g.vertex_iterator do |b|
            Rake::Task.tasks.each do |t|
                b.call(t) unless t.name == this_task
            end
        end
        # neighbors of task t are its prerequisites
        g.adjacent_iterator { |t, b| t.prerequisites.each(&b) }
        g.directed = true
    }

    dep.write_to_graphic_file('png', this_task)
    puts "Wrote dependency graph to #{this_task}.png."
end
