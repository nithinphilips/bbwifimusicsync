require 'ostruct'
require 'albacore/support/openstruct'

module SigToolConfig
    include Albacore::Configuration

    def self.sigtoolconfig
        @config ||= OpenStruct.new.extend(OpenStructToHash).extend(SigToolConfig)
    end

    def sigtool
        config ||= SigToolConfig.sigtoolconfig
        yield(config) if block_given?
        config
    end

   def self.included(mod)
      # nothing to do
   end

end


