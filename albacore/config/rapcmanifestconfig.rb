require 'ostruct'
require 'albacore/support/openstruct'

module RapcManifestConfig
    include Albacore::Configuration

    def self.rapcmanifestconfig
        @config ||= OpenStruct.new.extend(OpenStructToHash).extend(RapcManifestConfig)
    end

    def rapcmanifest
        config ||= RapcManifestConfig.rapcmanifestconfig
        yield(config) if block_given?
        config
    end

   def self.included(mod)
       # nothing to do
   end

end
