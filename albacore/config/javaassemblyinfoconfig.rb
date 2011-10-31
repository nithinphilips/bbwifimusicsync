require 'ostruct'
require 'albacore/support/openstruct'

module JavaAssemblyInfoConfig
    include Albacore::Configuration

    def self.javaassemblyinfoconfig
        @config ||= OpenStruct.new.extend(OpenStructToHash).extend(JavaAssemblyInfoConfig)
    end

    def javaassemblyinfo
        config ||= JavaAssemblyInfoConfig.javaassemblyinfoconfig
        yield(config) if block_given?
        config
    end

   def self.included(mod)
      # nothing to do
   end

end
