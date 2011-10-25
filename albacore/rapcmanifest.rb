require 'albacore/albacoretask'

class EntryPoint
        
end

class RapcManifest
  include Albacore::Task
  
  attr_accessor :output_file
  attr_accessor :name, :version, :vendor, :description
  attr_accessor :title, :firsticon, :arguments
  attr_accessor :nameresourcebundle, :nameresourceid, :flags, :type, :runonstartup, :startuptier, :midlet_class, :ribbon_position, :system_module
  
  attr_array :icons, :focus_icons
  
  def initialize
    super()
    #update_attributes Albacore.configuration.assemblyinfo.to_hash
    @icons = []
    @focus_icons = []
  end

  def execute
    write_rapcmanifest @output_file, @name, @title, @arguments, @version, @vendor,  @description, @icons, @focus_icons,
                       @nameresourcebundle, @nameresourceid, @flags, @type, @runonstartup, @startuptier, @midlet_class,
                       @ribbon_position, @system_module
  end
  
  def write_rapcmanifest(output_file, name, title, arguments, version, vendor, description, icons, focus_icons, nameresourcebundle, nameresourceid, flags, type, runonstartup, startuptier, midlet_class, ribbon_position, system_module)
          @logger.info "Generating RAPC manifest File At: " + File.expand_path(output_file)
          check_output_file @output_file

          File.open(output_file, 'w') do |f|
                  f.puts "MIDlet-Name: #{name}"
                  f.puts "MIDlet-Version: #{version}"
                  f.puts "MIDlet-Vendor: #{vendor}"


                  f.puts "MIDlet-Description: #{description}" if description != nil

                  f.puts "MIDlet-Jar-URL: #{name}.jar"
                  f.puts "MIDlet-Jar-Size: 0"
                  f.puts "MicroEdition-Profile: MIDP-2.0"
                  f.puts "MicroEdition-Configuration: CLDC-1.1"

                  if type == "CLDC"
                          f.puts "MIDlet-1: #{title},#{icons[0]},#{arguments}"
                          f.puts "RIM-MIDlet-Position-1: #{ribbon_position}" if ribbon_position != nil

                          if nameresourcebundle != nil && nameresourceid != nil
                                f.puts "RIM-MIDlet-NameResourceBundle-1: #{nameresourcebundle}"
                                f.puts "RIM-MIDlet-NameResourceId-1: #{nameresourceid}"
                          end

                          icon_index = 0;
                          icons.each_with_index do |icon, index|
                                  if index != 0
                                          icon_index += 1
                                          f.puts "RIM-MIDlet-Icon-1-#{icon_index}: #{icon}"
                                  end
                          end

                          focus_icons.each_with_index do |icon|
                                  icon_index += 1
                                  f.puts "RIM-MIDlet-Icon-1-#{icon_index}: #{icon},focused"
                          end

                          f.puts "RIM_MIDlet-Icon-Count-1: #{icon_index}" if icon_index > 0

                          flags = 0x00
                          flags |= 0xE1-((2*startupTier)<<4) if runonstartup
                          flags |= 0x02 if system_module
                          f.puts "RIM-MIDlet-Flags-1: #{flags}"
                  elsif type == "MIDLET"
                        f.puts "MIDlet-1: #{title},#{icons[0]},#{midletclass}"
                        f.puts "RIM-MIDlet-Position-1: #{ribbon_position}" if ribbon_position > 0
                        flags = 0xE0
                        flags |= 0x02 if system_module
                        f.puts "RIM-MIDlet-Flags-1: #{flags}"
                  else
                        flags = 0x02
                        flags |= 0xE1 - ((2 * startuptier) << 4) if runonstartup
                        f.puts "RIM-Library-Flags: #{flags}"
                  end
                  f.puts "RIM-MIDlet-Icon-Count-1: 1"

          end
  end

  def check_output_file(file)
    return true if file
    fail_with_message 'output_file cannot be nil'
    return false
  end
end

