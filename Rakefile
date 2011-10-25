PRODUCT       = "bbwifimusicsync"
PRODUCT_LONG  = "Wireless Music Sync"
DESCRIPTION   = ""
VERSION       = "0.2.0"                 # We omit the build segment of the version number.
AUTHORS       = "Nithin Philips"
COPYRIGHT     = "(c) 2011 #{AUTHORS}"
PROJECT_URL   = "https://sourceforge.net/projects/bbwifimusicsync/"
TRADEMARKS    = "BlackBerry(r) is a registered trademark of Research in Motion"

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
require 'find'
FileList["./albacore/*.rb"].each { |f| require f }

task :default => [:dist]

task :dist => [:dist_zip, :dist_src, :installer, :test]

#Albacore.configure do |config|
#  config.rapcpath = ""
#end


rapcmanifest :rapcmanifest do |m|
        m.output_file = "file.rapc"
        m.name = "WifiMusicSync"
        m.title = "Wireless Music Sync"
        m.version = "1.0.0"
        m.vendor = AUTHORS
        m.description = DESCRIPTION
        m.type = "CLDC"
        m.icons << "a.ico" << "b.ico" << "c.ico" << "d.ico"
        m.focus_icons << "e.ico"

end

desc "Build"
msbuild :build  => :assemblyinfo do |msb|
        msb.properties :configuration => CONFIGURATION, "OutputPath" => OUTPUT_DIR
        msb.targets :Clean, :Build
        msb.solution = "WifiMusicSync.sln"
        msb.verbosity = "quiet"
end

desc "Moving binaries"
task :binaries => :build  do
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

desc "Enabling logging"
task :logging => :binaries  do
        files = FileList["#{OUTPUT_DIR}/*.log.config"]
        FileUtils.cp_r files, "#{BIN_DIR}/#{PACKAGE}/"
end

desc "Packaging source"
task :dist_src do |z|
        #TODO: This won't include Afterthought git submodule. We need to fix it.
        sh "git archive HEAD --format=zip -9 --prefix=\"#{PACKAGE}/\" > \"#{BUILD_DIR}/#{SRC_PACKAGE}.zip\""
end

desc "Packaging binaries"
zip :dist_zip => [:binaries] do |z|
        z.directories_to_zip BIN_DIR
        z.output_file = "#{BIN_PACKAGE}.zip"
        z.output_path = BUILD_DIR
end

desc "Updating installer file list"
nsisfilelist :installerfiles => [:binaries] do |n|
#nsisfilelist :installerfiles do |n|
        n.dirs << File.expand_path("#{BIN_DIR}/#{PACKAGE}/")
        n.add_files_list = File.expand_path("MusicSync.Installer/files_ADD.nsi")
        n.remove_files_list = File.expand_path("MusicSync.Installer/files_REM.nsi")
end

desc "Building installer"
exec :installer => [:installerfiles] do |exec|
        exec.command = 'C:/Program Files (x86)/NSIS/makensis.exe'
        exec.parameters = [
                "/V3",
                "/DPRODUCT_VERSION=#{VERSION}",
                "/DOUT_FILE=#{BUILD_DIR}/#{INS_PACKAGE}.exe",
                File.expand_path("MusicSync.Installer/Installer.nsi")]
end

mstest :test => [:build] do |test|
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

task :assemblyinfo => [:libasminfo, :testsasminfo, :serverasminfo, :desktopasminfo]

assemblyinfo :libasminfo => :clean do |a|
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

assemblyinfo :testsasminfo => :clean do |a|
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

assemblyinfo :serverasminfo => :clean do |a|
        a.title        = "MusicSync.Server"
        a.description  = "A server for wirelessly syncing music to BlackBerry phones"
        a.output_file  = "MusicSync.Desktop/Properties/AssemblyInfo.cs"

        a.product_name = PRODUCT_LONG
        a.version      = VERSION
        a.file_version = VERSION
        a.copyright    = COPYRIGHT
        a.company_name = AUTHORS
        a.trademark    = TRADEMARKS
        a.namespaces "System.Runtime.CompilerServices"
end

assemblyinfo :desktopasminfo => :clean do |a|
        a.title        = "MusicSync.Desktop"
        a.description  = "A desktop client for syncing music to BlackBerry phones"
        a.output_file  = "MusicSync.Server/Properties/AssemblyInfo.cs"

        a.product_name = PRODUCT_LONG
        a.version      = VERSION
        a.file_version = VERSION
        a.copyright    = COPYRIGHT
        a.company_name = AUTHORS
        a.trademark    = TRADEMARKS
        a.namespaces "System.Runtime.CompilerServices"
end

