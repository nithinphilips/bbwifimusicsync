require 'albacore/albacoretask'
require File.join(File.expand_path(File.dirname(__FILE__)), 'config', 'sigtoolconfig.rb')

class SigTool
    include Albacore::Task
    include Albacore::RunCommand
    include SigToolConfig

    # To generate doc in markdown format, copy the properties to another file and run the following command on them
    # > sed -e 's/attr_accessor //g' -e 's/attr_array //g' -e 's/^[ \t]*//' -e '/^$/d' -e 's/:\([^,]*\),/\n * **\1**<br\/>\n/g' -e '/^:/ s/^:\([^ ]*\)/\n * **\1**<br\/>/g' -e 's/\s*#/\n  /g'

    attr_accessor :jdkhome,     # *Optional.* Path to the Java or Java SDK installation. If not specified, the `JDK_HOME` or `JAVA_HOME` environment variables will be used.
                  :jdehome,     # *Optional.* Path to the RIM BlackBerry JDE installation. If not specified, the `JDE_HOME` environment variable will be used.
                  :sigtooljar,  # *Optional.* The path to the signature tool jar file. If not set, the `JDE_HOME` environment variable will be used to find the sigtool.
                  :forceclose,  # *Optional.* Close signature tool even if signature failed. Default is *false*.
                  :close,       # *Optional.* Close after signature request if no errors have occurred. Default is *true*.
                  :request,     # *Optional.* Request signature when application launches. Default is *true*.
                  :password     # *Optional.* The password for the signing keys. Removes the interactive password prompt. This implicitly sets "close" and "request" to *true*.

    attr_array :codfiles         # *Required.* Cod files to sign.

    def initialize
        @close = true
        @request = true
        super()
        update_attributes Albacore.configuration.sigtool.to_hash
    end

    def execute
        autoparams()
        sign_cod(@codfiles)
    end

    def sign_cod(codfiles)
        check_cod codfiles

        fail_with_message "sigtooljar path must be specified" if !@sigtooljar

        command_parameters = []
        command_parameters << "-jar"
        command_parameters << "\"" + @sigtooljar + "\""

        if @password != nil
            @close = true
            @request = true
            command_parameters << "-p"
            command_parameters << "\"" + @password + "\""
        end

        if @codfiles.is_a? Array
            @codfiles.each { |f|
                f = f + ".signed"
                FileUtils.touch f
            }

            @codfiles = @codfiles.map{ |f| "\"" + f + "\"" }.join(" ")
        else
            @codfiles = "\"" + @codfiles + "\""
        end

        command_parameters << "-C" if @forceclose == true
        command_parameters << "-c" if @close == true
        command_parameters << "-a" if @request == true

        command_parameters << @codfiles

        result = run_command "SigTool", command_parameters.join(" ")

        failure_message = 'SigTool Failed. See Rake Log For Details'
        fail_with_message failure_message if !result
    end

    def check_cod(file)
        return if file
        msg = 'cod file cannot be nil'
        fail_with_message msg
    end

    def autoparams()

        # Environment variables used:
        #  JDE_HOME: Path to the RIM JDE installation.
        #  JDK_HOME or JAVA_HOME: Path the the Java SDK installation.

        jde_home = ENV['JDE_HOME']       # load any env var.
        jde_home = @jdehome if @jdehome  # override env var with explicit jdehome

        if jde_home
            jde_home = jde_home.gsub(File::ALT_SEPARATOR, File::SEPARATOR)
            if Dir.exists?(jde_home)
                    @sigtooljar = File.join(jde_home, "plugins", "net.rim.ejde", "vmTools", "SignatureTool.jar") unless @sigtooljar
            else
                @logger.warn "Specified JDE_HOME, '#{jde_home}' does not exist."
            end
        else
            @logger.warn "No JDE_HOME environment variable or jdehome property set. Cannot autoconfigure SigTool."
        end


        jdk_home = ENV['JDK_HOME'] || ENV['JAVA_HOME']
        jdk_home = @jdkhome if @jdkhome  # override env var with explicit jdehome

        if jdk_home and Dir.exists?(jdk_home)
            java_exe = File.join(jdk_home, "bin", "javaw")
            java_exe = java_exe.gsub(File::ALT_SEPARATOR, File::SEPARATOR)

            @command = java_exe unless @command
        else
            @logger.warn "No JDK_HOME environment variable or jdehome property set. Cannot autoconfigure JDK."
        end
    end

end

