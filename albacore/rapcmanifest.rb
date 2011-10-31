require 'albacore/albacoretask'
require File.join(File.expand_path(File.dirname(__FILE__)), 'config', 'rapcmanifestconfig.rb')

class EntryPoint
    attr_accessor :title,  # Module title. The title appears below the icon and in the applications list on the device.
    :system_module,        # Set to true for a system module. System modules run in the background and do not have an icon on the home screen.
    :run_on_startup,       # Set to true if application should start when device starts.
    :startup_tier,         # Startup priority relative to other applications. Valid values are 0-7, lower value = higher priority.
    :ribbon_position,      # Position of icon in ribbon. Higher values move the icon closer to the top of the ribbon.
    :name_resource_bundle, # Name of resource bundle that contains the module title, eg: ca.slashdev.MyAppResources
    :name_resource_id,     # The id of the resource that contains the module title, eg: 1234.
    :midlet_class,
    :icons,                # A list of icons to be display on the ribbon.
    :focus_icons,          # A list of icons that appear on hover/focus on the ribbon.
    :arguments             # A list of arguments passed to the main method.

    alias :icon :icons
    alias :focus_icon :focus_icons
    alias :argument :arguments
    alias :icon= :icons=
    alias :focus_icon= :focus_icons=
    alias :argument= :arguments=

    def initialize
        @icons = []
        @focus_icons = []
    end
end

class RapcManifest < EntryPoint
    include Albacore::Task
    include RapcManifestConfig

    attr_accessor :output_file, # Path to save the output. Must end in `.rapc`. Optional if sdkversion is specified. When both `sdkversion` and `output_file` are specified, `output_file` wins.
        :sdkversion,         # The version of the SDK that is being targeted. It is used for calculating paths, and can be used in lieu of `output_file`. Output will be saved to `<destdir>/Standard/<sdkversion>/<output>.rapc`
        :destdir,            # *Optional.* Root output directory. Output will be saved to `<destdir>/Standard/<sdkversion>/<output>.rapc`. Default is `deliverables`.
        :output,             # *Optional.* When used along with destdir and sdkversion, sets the output file name. Default is the module's `name`.
        :name,               # *Required.* Module name. The name is used to identify app while installing and in the Application management screen.
        :version,            # *Required.* Module version number. Version number strings must contain only numbers and dots.
        :vendor,             # *Optional.* Company name. The vendor string appears in the applications properties on the device.
        :description,        # *Optional.* Module description. The description string appears in the application properties on the device.
        :type                # *Optional.* Type of cod file to create. Valid values are :LIBRARY, :CLDC, :MIDLET. When :MIDLET is specified, the midletclass property is required. Default is :CLDC

    attr_array :entry_points # A list of entry points


    alias :entry_point :entry_points
    alias :entry_point= :entry_points=

    def initialize
        @type = :CLDC
        @entry_points = []
        @destdir = "deliverables"
        super()
        update_attributes Albacore.configuration.rapcmanifest.to_hash
    end

    # Implicit entry point is this object: entry_point[0]
    def implicit_entry_point
        self
    end

    def execute
        autoparams()
        write_rapcmanifest
    end

    def write_rapcmanifest()
        check_output_file @output_file
        FileUtils.mkdir_p(File.dirname(output_file))
         @logger.info "Generating RAPC manifest File At: " + File.expand_path(output_file)

        if @entry_points == nil
            # use the implicit
            @entry_points = [implicit_entry_point]
        elsif @entry_points.is_a?(EntryPoint)
            # make into an array
            @entry_points = [implicit_entry_point, @entry_points]
        else
            # push implicit to the top
            @entry_points.unshift(implicit_entry_point)
        end

        jar_name = File.basename(output_file, ".rapc")

        File.open(output_file, 'w') do |f|
            f.puts "MIDlet-Name: #{@name}"
            f.puts "MIDlet-Version: #{@version}"
            f.puts "MIDlet-Vendor: #{@vendor}"

            f.puts "MIDlet-Description: #{@description}" if @description

            f.puts "MIDlet-Jar-URL: #{jar_name}.jar"
            f.puts "MIDlet-Jar-Size: 0"
            f.puts "MicroEdition-Profile: MIDP-2.0"
            f.puts "MicroEdition-Configuration: CLDC-1.1"

            if type == :CLDC
                @entry_points.to_enum.with_index(1) do |entry, entry_index|

                    # Work with both strings and arrays
                    entry.icons = [entry.icons] if entry.icons.is_a? String
                    entry.focus_icons = [entry.focus_icons] if entry.focus_icons.is_a? String

                    f.puts "MIDlet-#{entry_index}: #{entry.title},#{entry.icons[0]}," + build_args(entry.arguments)
                    f.puts "RIM-MIDlet-Position-#{entry_index}: #{entry.ribbon_position}" if entry.ribbon_position

                    if entry.name_resource_bundle != nil && entry.name_resource_id != nil
                        f.puts "RIM-MIDlet-NameResourceBundle-#{entry_index}: #{entry.name_resource_bundle}"
                        f.puts "RIM-MIDlet-NameResourceId-#{entry_index}: #{entry.name_resource_id}"
                    end

                    icon_index = 0;
                    entry.icons.each_with_index do |icon, index|
                        if index != 0
                            icon_index += 1
                            f.puts "RIM-MIDlet-Icon-#{entry_index}-#{icon_index}: #{icon}"
                        end
                    end
                    entry.focus_icons.each_with_index do |icon|
                        icon_index += 1
                        f.puts "RIM-MIDlet-Icon-#{entry_index}-#{icon_index}: #{icon},focused"
                    end

                    f.puts "RIM-MIDlet-Icon-Count-#{entry_index}: #{icon_index}" if icon_index > 0

                    flags = 0x00
                    flags |= 0xE1-((2*entry.startup_tier)<<4) if entry.run_on_startup
                    flags |= 0x02 if entry.system_module
                    f.puts "RIM-MIDlet-Flags-#{entry_index}: #{flags}"
                end
            elsif type == :MIDLET
                @entry_points.to_enum.with_index(1) do |entry, entry_index|

                    # Work with both strings and arrays
                    entry.icons = [entry.icons] if entry.icons.is_a? String

                    f.puts "MIDlet-#{entry_index}: #{entry.title},#{entry.icons[0]},#{entry.midlet_class}"
                    f.puts "RIM-MIDlet-Position-#{entry_index}: #{entry.ribbon_position}" if entry.ribbon_position > 0
                    flags = 0xE0
                    flags |= 0x02 if entry.system_module
                    f.puts "RIM-MIDlet-Flags-#{entry_index}: #{flags}"
                end
            else
                flags = 0x02
                flags |= 0xE1 - ((2 * @startup_tier) << 4) if @run_on_startup
                f.puts "RIM-Library-Flags: #{flags}"
            end
        end
    end

    def build_args(args)
        if args.is_a? Array
            args.join " "
        elsif args == nil
            return ""
        else
            return args
        end
    end

    def check_output_file(file)
        return true if file
        fail_with_message 'output_file cannot be nil'
        return false
    end

    def autoparams
        output_file_name = @name
        output_file_name = @output if @output

        @output_file = File.join(@destdir, "Standard", @sdkversion, output_file_name) + ".rapc" unless @output_file
    end
end

