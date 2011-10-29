# See http://bb-ant-tools.sourceforge.net/docs for details.

require 'albacore/albacoretask'

class JavaAssemblyInfo
        include Albacore::Task
        include Albacore::RunCommand

        attr_accessor :title, :description, :copyright, :version, :output_dir, :class_name, :package_name

        def initialize
                super()
        end

        def execute
                output_file = "#{@output_dir}/#{@class_name}.java"
                date = Time.now.strftime("%b %d %Y")

                File.open(output_file, 'w') do |f|
                    f.puts "package #{@package_name};"
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

        def check_file(file)
                return if file
                msg = 'output file cannot be nil'
                fail_with_message msg
        end

end

