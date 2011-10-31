# See http://bb-ant-tools.sourceforge.net/docs for details.
# See https://github.com/blackberry/WebWorks/blob/master/packager/src/net/rim/tumbler/rapc/Rapc.java for rapc command-line
require 'albacore/albacoretask'
require File.join(File.expand_path(File.dirname(__FILE__)), 'config', 'rapcconfig.rb')
require 'find'

class Rapc
    include Albacore::Task
    include Albacore::RunCommand
    include RapcConfig

    # To generate doc in markdown format, copy the properties to another file and run the following command on them
    # > sed -e 's/attr_accessor //g' -e 's/attr_array //g' -e 's/^[ \t]*//' -e '/^$/d' -e 's/:\([^,]*\),/\n * **\1**<br\/>\n/g' -e '/^:/ s/^:\([^ ]*\)/\n * **\1**<br\/>/g' -e 's/\s*#/\n  /g'

    attr_accessor :jdkhome,     # *Optional.* Path to the Java SDK installation. If not specified, the `JDK_HOME` or `JAVA_HOME` environment variables will be used.
        :jdehome,     # *Optional.* Path to the RIM BlackBerry JDE installation. If not specified, the `JDE_HOME` environment variable will be used.
        :exepath,     # *Optional.* Explicitly define the directory containing the preverify command used by the rapc compiler. When not defined, the system's default `PATH` is used.
        :destdir,     # *Optional.* Root output directory of .cod file. Standard output will be saved to `<destdir>/Standard/<sdkversion>`. Default is `deliverables`.
        :quiet,       # *Optional.* Tells the rapc command to be less chatty, default is *true*.
        :verbose,     # *Optional.* Turn on verbose output of the rapc compiler, default is *false*.
        :nodebug,     # *Optional.* Turn off generation of .debug files, default is *false*.
        :nowarn,      # *Optional.* Disable warning messages, default is *false*.
        :warnerror,   # *Optional.* Treat all warnings as errors, default is *false*.
        :noconvert,   # *Optional.* Don't convert images to png, default is *false*.
        :nopreverify, # *Optional.* Do not call the preverify executable, default is *false*.
        :createweb,   # *Optional.* If true, extracts OTA compatible cods to `<destdir>/Standard/<sdkversion>`, default is *false*.
        :rapcfile,    # *Optional.* The rapc spec file (optional). Default is `<destdir>/output.rapc`
        :output,      # *Required.* The name out the output files.
        :sdkversion,  # *Required.* The version of the SDK that is targeted. It is used for calculating paths and declaring tags. eg: "5.0.0".
        :library      # *Optional.* If true, a library, rather than an app, will be output. Default is *false*.

    attr_array :source,      # *Required.* A list of all source files and resource files. Required.
        :imports,     # *Optional.* A list of `.jar` files to import. The appropriate `net_rim_api.jar` for the sdkversion will be imported by default.
        :tags         # *Optional.* List of preprocessor tags. By default the `PREPROCESSOR` tag and a tag named `BlackBerrySDK<sdkversion>` will be declared. Set to nil to disable all tags.

    alias :sources :source
    alias :sources= :source=


    def initialize
        @tags = [] # Must be empty array, nil will be interpreted differently.
        @destdir = "deliverables"
        @quiet = true
        super()
        update_attributes Albacore.configuration.rapc.to_hash
    end

    def execute
        autoparams()
        build_app()
    end

    def build_app()
        output_dir = File.join(@destdir, "Standard", @sdkversion)
        FileUtils.mkdir_p(output_dir)
        codename = File.join(output_dir, @output)
        @rapcfile =  "#{codename}.rapc" if @rapcfile == nil

        output_type = @library ? "-library" : "-codename"

        failure_message = "Specified .rapc file: #{@rapcfile} does not exist."
        fail_with_message failure_message if !File.exists?(@rapcfile)

        command_parameters = []
        command_parameters << "-cr" # Compress Resources
        command_parameters << "#{output_type}=\"#{codename}\" \"#{@rapcfile}\""
        command_parameters << "-import=\"" + build_args(@imports) + "\"" if @imports
        command_parameters << "-exepath=\"" + File.expand_path(@exepath) + "\"" if @exepath

        command_parameters << "-nodebug" if @nodebug
        command_parameters << "-nowarn" if @nowarn
        command_parameters << "-quiet" if @quiet
        command_parameters << "-wx" if @warnerror
        command_parameters << "-noconvertpng" if @noconvert
        command_parameters << "-nopreverified" if @nopreverify

        command_parameters << "-define=" + build_args(@tags.unshift("PREPROCESSOR")) if @tags != nil

        command_parameters << @source.map { |f| "\"" + f + "\"" }

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

    def autoparams()
        sdktag = "BlackBerrySDK#{@sdkversion}"
        @tags << sdktag if @tags != nil && @tags.index(sdktag) == nil

        # Environment variables used:
        #  JDE_HOME: Path to the RIM JDE installation.
        #  JDK_HOME or JAVA_HOME: Path the the Java SDK installation.

        jde_home = ENV['JDE_HOME']       # load any env var.
        jde_home = @jdehome if @jdehome  # override env var with explicit jdehome

        if jde_home
            jde_home = jde_home.gsub(File::ALT_SEPARATOR, File::SEPARATOR)
            if Dir.exists?(jde_home)
                cwd = Dir.pwd
                Dir.chdir(File.join(jde_home, "plugins"))
                comp_pack_name = Dir.glob("net.rim.ejde.componentpack#{@sdkversion}*").first
                Dir.chdir(cwd)

                if comp_pack_name
                    @command = File.join(jde_home, "plugins", comp_pack_name, "components", "bin", "rapc.exe") if !@command
                    # @imports can be either nil, string, or array
                    # nil    -> set it to sdk_api_jar
                    # string -> if it's not sdk_api_jar, make it into an array and add sdk_api_jar
                    # array  -> add sdk_api_jar if it's not already there
                    sdk_api_jar = File.join(jde_home, "plugins", comp_pack_name, "components", "lib", "net_rim_api.jar")
                    @imports = sdk_api_jar if !@imports
                    if @imports.is_a? Array
                        @imports << sdk_api_jar unless @imports.include? sdk_api_jar
                    else
                        @imports = [@imports, sdk_api_jar] unless @imports == sdk_api_jar
                    end
                else
                    @logger.warn "Could not find the component pack directory for SDK #{@sdkversion}. Fix the JDE_HOME environment variable or set 'command' and 'imports' properties manually."
                end
            else
                @logger.warn "Specified JDE_HOME, '#{jde_home}' does not exist."
            end
        else
            @logger.warn "No JDE_HOME environment variable or jdehome property set. Cannot autoconfigure RAPC."
        end


        jdk_home = ENV['JDK_HOME'] || ENV['JAVA_HOME']
        jdk_home = @jdkhome if @jdkhome  # override env var with explicit jdehome

        if jdk_home and Dir.exists?(jdk_home)
            jdk_bin = File.join(jdk_home, "bin")
            jdk_bin = jdk_bin.gsub(File::SEPARATOR, File::ALT_SEPARATOR || File::SEPARATOR) # Use platform specific path separator
            @logger.warn "Java compiler 'javac' does not exist at the specified path." unless File.exists?(File.join(jdk_bin, "javac.exe"))

            ENV['PATH'] = jdk_bin + ";" + ENV['PATH'] unless ENV['PATH'].include? jdk_bin
        else
            @logger.warn "No JDK_HOME environment variable or jdehome property set. Cannot autoconfigure JDK."
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

    def build_args(args)
        if args.is_a? Array
            args.join File::PATH_SEPARATOR
        else
            return args
        end
    end

end

