# See http://bb-ant-tools.sourceforge.net/docs for details.

require 'albacore/albacoretask'

class NsisFileList
    include Albacore::Task
    include Albacore::RunCommand

    attr_accessor :add_files_list, :remove_files_list

    attr_array :dirs           # Directories containing source files.

    def initialize
        @dirs = []
        super()
    end

    def execute
        cwd = File.expand_path(".").gsub!("/", "\\")

        File.open(@add_files_list, 'w') do |add_file|
            File.open(@remove_files_list, 'w') do |rem_file|
                for dir in @dirs
                    dir.gsub!("/", "\\")
                    RecurseTree(dir, add_file, rem_file, dir, cwd)
                end
            end
        end
    end

    def RecurseTree(directory, add_file, rem_file, src_root, cwd)

        fail_with_message "At least one directory must be specified as dirs" if !directory
        fail_with_message "An add_files_list must be specified" if !add_file
        fail_with_message "A remove_files_list must be specified" if !rem_file

        out_path = directory.sub(src_root, "$INSTDIR")

        add_file.puts  ""
        add_file.puts  "SetOutPath \"#{out_path}\""
        add_file.puts  "SetOverwrite ifnewer"

        dir = Dir.open(directory)

        for path in dir
            path = directory + "\\" + path;
            unless FileTest.directory?(path)
                add_line = path.sub(cwd, "..")
                add_file.puts "File \"#{add_line}\""

                rem_line = path.sub(src_root, "$INSTDIR")
                rem_file.puts "Delete \"#{rem_line}\""
            end
        end

        for path in dir
            next if path == "." or path == ".."

            path = directory + "\\" + path;

            if File.lstat(path).directory? then
                RecurseTree(path, add_file, rem_file, src_root, cwd)
            end
        end


        rem_file.puts "RmDir \"#{out_path}\""
        rem_file.puts ""
    end

    def check_file(file)
        return if file
        msg = 'output file cannot be nil'
        fail_with_message msg
    end

end

