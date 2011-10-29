# See http://bb-ant-tools.sourceforge.net/docs for details.
# See https://github.com/blackberry/WebWorks/blob/master/packager/src/net/rim/tumbler/rapc/Rapc.java for rapc command-line
require 'albacore/albacoretask'
require './albacore/config/rapcconfig.rb'

class Rapc
    include Albacore::Task
    include Albacore::RunCommand
    include RapcConfig

    attr_accessor :jdkbin,      # Path to the folder where javac.exe is located. Used by rapc.exe
                  :exepath,     # Explicitly define the directory containing the preverify command used by the rapc compiler. When not defined, the systems default PATH is used.
                  :destdir,     # Output directory of .cod file.
                  :importref,   # Path of .jar files to import given as a reference to a path defined elsewhere. The rapc task will always add the net_rim_api.jar file by default. This attribute is optional.
                  :quiet,       # Tells the rapc command to be less chatty, default to false.
                  :verbose,     # Turn on verbose output of the rapc compiler, default is false.
                  :nodebug,     # Turn off generation of .debug files, default is false.
                  :nowarn,      # Disable warning messages, default is false.
                  :warnerror,   # Treat all warnings as errors, default is false.
                  :noconvert,   # Don't convert images to png, default is false.
                  :nopreverify, # Do not call the preverify executable, default is false.
                  :rapcfile,    # The rapc spec file (optional). Default is destdir/output.rapc
                  :output,      # The name out the output files
                  :createweb,   # If true, extracts OTA compatible cods.
                  :sdkversion   # The version of the SDK that is targeted. It is used for calculating paths and declaring tags. eg: "5.0.0"

    attr_array :source,      # An array of all source files + resource files
               :imports,     # Path of .jar files to import. Path strings consist of relative or absolute path names delimited by a ';' (Windows) or ':' (Unix) character.
               :tags         # list of preprocessor tags. See the note below regarding preprocessor usage.

    def initialize
        @command = "rapc.exe"
        @tags = []
        super()
        update_attributes Albacore.configuration.rapc.to_hash
    end

    def execute
        build_app()
    end

    def build_app()

        sdktag = "BlackBerrySDK#{sdkversion}"
        @tags << sdktag if @tags.index(sdktag) == nil

        output_dir = File.join(@destdir, "Standard", @sdkversion)
        FileUtils.mkdir_p(output_dir)
        codename = File.join(output_dir, @output)
        @rapcfile =  "#{codename}.rapc" if @rapcfile == nil

        failure_message = "Specified .rapc file: #{@rapcfile} does not exist."
        fail_with_message failure_message if !File.exists?(@rapcfile)

        ENV['PATH'] = @jdkbin + ";" + ENV['PATH'] if @jdkbin != nil and !ENV['PATH'].include? @jdkbin

        command_parameters = []
        command_parameters << "-cr -codename=\"#{codename}\" \"#{@rapcfile}\""
        command_parameters << "-import=" + build_args(@imports) if @imports != nil
        command_parameters << "-exepath=" + File.expand_path(@exepath) if @exepath != nil and !@exepath.empty?

        command_parameters << "-nodebug" if @nodebug == true
        command_parameters << "-nowarn" if @nowarn == true
        command_parameters << "-wx" if @warnerror == true
        command_parameters << "-noconvertpng" if @noconvert == true
        command_parameters << "-nopreverified" if @nopreverify == true

        command_parameters << "-define=PREPROCESSOR;" + build_tags(@tags) if @tags != nil

        # Wrap each source file argument in quotes
        finalfiles = @source.map { |f| "\"" + f + "\"" }
        command_parameters << finalfiles.join(" ")

        puts @command + " " + command_parameters.join(" ")

        result = run_command "RAPC", command_parameters.join(" ")
        failure_message = 'RAPC Failed. See Build Log For Detail'
        fail_with_message failure_message if !result

        if @createweb
            web_output_dir = File.join(@destdir, "Web", @sdkversion)
            FileUtils.mkdir_p(web_output_dir)
            FileUtils.cp_r(output_dir, File.join(@destdir, "Web"))

            web_output_cod = File.join(web_output_dir, @output) + ".cod"
            web_output_zip = File.join(web_output_dir, @output) + ".zip"

            FileUtils.mv(web_output_cod, web_output_zip)
            extract_zip(web_output_zip, web_output_dir)

            FileUtils.rm(web_output_zip)
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

    def build_tags(tags)
        tags.join ";" # Use ; for windows, : for unix
    end

    def build_args(args)
        if args.is_a? Array
            args.join ";" # Use ; for windows, : for unix
        else
            return args
        end
    end

end

