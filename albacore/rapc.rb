# See http://bb-ant-tools.sourceforge.net/docs for details.

require 'albacore/albacoretask'

class Rapc
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_accessor :jdehome,     # Sets the JDE home directory. This attribute is required when the jde.home property is not defined at the project level.
                :jdkhome,     # Sets an alternative JVM home directory. Use this to override the version of the JVM used to execute the rapc compiler. When this attribute is not defined, the systems default JVM is used.
                :exepath,     # Explicitly define the directory containing the preverify command used by the rapc compiler. When not defined, the systems default PATH is used.
                :destdir,     # Output directory of .cod file. When not defined, the base directory of the Ant build file is used.
                :output,      # Name of output file, eg: [output].cod, [output].cso, etc.
                :importref,   # Path of .jar files to import given as a reference to a path defined elsewhere. The rapc task will always add the net_rim_api.jar file by default. This attribute is optional.
                :quiet,       # Tells the rapc command to be less chatty, default to false.
                :verbose,     # Turn on verbose output of the rapc compiler, default is false.
                :nodebug,     # Turn off generation of .debug files, default is false.
                :nowarn,      # Disable warning messages, default is false.
                :warnerror,   # Treat all warnings as errors, default is false.
                :noconvert,   # Don't convert images to png, default is false.
                :nopreverify, # Do not call the preverify executable, default is false.
                :rapcfile

  attr_array :sourcdirs,      # Directory containing source files. 
             :import,         # Path of .jar files to import. Path strings consist of relative or absolute path names delimited by a ';' (Windows) or ':' (Unix) character. The rapc task will always add the net_rim_api.jar file by default. This attribute is optional.
             :tags            # ':' or ';' delimited list of preprocessor tags. See the note below regarding preprocessor usage.
  
  def initialize
    super()
    update_attributes msbuild.to_hash
  end
  
  def execute
    build_solution(@solution)
  end
  
  def build_solution(solution)
    check_solution solution
    
    command_parameters = []
    command_parameters << "\"#{solution}\""
    command_parameters << "\"-verbose\"" if @verbose != nil
    command_parameters << "\"-quiet\"" if @quiet != nil

    command_parameters << "\"/logger:#{@loggermodule}\"" if @loggermodule != nil
    command_parameters << build_properties if @properties != nil
    command_parameters << "\"/target:#{build_targets}\"" if @targets != nil
    
    result = run_command "C:\Users\Nithin\AppData\Local\Eclipse\plugins\net.rim.ejde.componentpack6.0.0_6.0.0.30\components\bin\rapc.exe", command_parameters.join(" ")
    
    failure_message = 'MSBuild Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end
  
  def check_solution(file)
    return if file
    msg = 'solution cannot be nil'
    fail_with_message msg
  end
  
  def build_targets
    @targets.join ";"
  end

  def build_properties
    option_text = []
    @properties.each do |key, value|
      option_text << "/p:#{key}\=\"#{value}\""
    end
    option_text.join(" ")
  end
end

