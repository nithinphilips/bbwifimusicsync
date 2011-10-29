require 'albacore/albacoretask'
require './albacore/config/sigtoolconfig.rb'

class SigTool
    include Albacore::Task
    include Albacore::RunCommand
    include SigToolConfig

    attr_accessor :jrebin,     # Path to the folder where javaw.exe is located
                  :sigtooljar, # Set the path to the signature tool jar file. When this attribute is set the jdehome attribute is optional.  Optional
                  :forceclose, # Close signature tool even if signature failed. Default is false.    Optional
                  :close,      # Close after signature request if no errors have occurred. Default is true.  Optional
                  :request,    # Request signature when application launches. Default is true.   Optional
                  :password    # Removes the interactive password prompt. This implicitly sets "close" and "request" to true.    Optional

    attr_array :codfile    # cod files to sign.

    def initialize
        @close = true
        @request = true
        @command = "javaw"
        super()
        update_attributes Albacore.configuration.sigtool.to_hash
    end

    def execute
        sign_cod(@codfile)
    end

    def sign_cod(codfile)
        check_cod codfile

        fail_with_message "sigtooljar path must be specified" if !sigtooljar

        ENV['PATH'] = @jrebin + ";" + ENV['PATH'] if @jrebin != nil and !ENV['PATH'].include? @jrebin

        command_parameters = []
        command_parameters << "-jar"
        command_parameters << "\"" + @sigtooljar + "\""


        if @password != nil
            @close = true
            @request = true
            command_parameters << "-p"
            command_parameters << "\"" + @password + "\""
        end

        if @codfile.is_a? Array
            @codfile.each { |f|
                f = f + ".signed"
                FileUtils.touch f
            }

            @codfile = @codfile.map{ |f| "\"" + f + "\"" }.join(" ")
        else
            @codfile = "\"" + @codfile + "\""
        end

        command_parameters << "-C" if @forceclose == true
        command_parameters << "-c" if @close == true
        command_parameters << "-a" if @request == true

        command_parameters << @codfile

        puts @command + " " + command_parameters.join(" ")

        result = run_command "SigTool", command_parameters.join(" ")

        failure_message = 'SigTool Failed. See Rake Log For Details'
        fail_with_message failure_message if !result
    end

    def check_cod(file)
        return if file
        msg = 'cod file cannot be nil'
        fail_with_message msg
    end

end

