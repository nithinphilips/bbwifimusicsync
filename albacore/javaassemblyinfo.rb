# See http://bb-ant-tools.sourceforge.net/docs for details.

require 'albacore/albacoretask'
require File.join(File.expand_path(File.dirname(__FILE__)), 'config', 'javaassemblyinfoconfig.rb')

class JavaAssemblyInfo
    include Albacore::Task
    include Albacore::RunCommand
    include JavaAssemblyInfoConfig

    attr_accessor :title, # The title of the app.
        :description, # The description of the app.
        :copyright, # Copyright notice.
        :version, # Version
        :output_dir, # The folder to save the output. You should organize by the package name. (eg: src/com/google/)
        :class_name, # The name of the class. Default is "AssemblyInfo".
        :package_name # The name of the package. If not specified, package keyword will be omitted.

    def initialize
        @title = ""
        @description = ""
        @copyright = ""
        @version = "1.0.0"
        @class_name = "AssemblyInfo"
        super()
        update_attributes Albacore.configuration.javaassemblyinfo.to_hash
    end

    def execute
        output_file = "#{@output_dir}/#{@class_name}.java"
        date = Time.now.strftime("%b %d %Y")

        FileUtils.mkdir_p(File.dirname(output_file))
        @logger.info "Generating Java AssemblyInfo file At: " + File.expand_path(output_file)

        File.open(output_file, 'w') do |f|
            f.puts "package #{@package_name};" if @package_name
            f.puts "public final class #{@class_name}"
            f.puts "{"
            f.puts "    public static final String Title = \"#{@title}\";"
            f.puts "    public static final String Copyright = \"#{@copyright}\";"
            f.puts "    public static final String Description = \"#{@description}\";"
            f.puts "    public static final String Version = \"#{@version}\";"
            f.puts "    public static final String Date = \"#{date}\";"
            f.puts "}"
        end

    end

end

