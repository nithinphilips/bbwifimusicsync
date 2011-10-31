Rake Tasks for BlackBerry
=========================
These are a collection of [Rake](http://rake.rubyforge.org/) tasks for
compiling BlackBerry apps. The tasks use
[Albacore](https://github.com/derickbailey/Albacore/) libraries, so to use
these tasks you'll need Albacore, along with Rake. These tasks are mostly based
on the [BB Ant Tools](http://http://bb-ant-tools.sourceforge.net/) project.

You should read the [Albacore Getting Started
guide](https://github.com/derickbailey/Albacore/wiki/Getting-Started) before
proceeding.

The RapcManifest and Rapc tasks share some common properties, ensuring that the
correct file names are always used. These properties are:

 * **sdkversion**:
 * **destdir**: Optional, Default is `deliverables`
 * **output**:

Set these properties to the same value in the related RapcManifest and Rapc
tasks. The calculated paths will be:

 * `<destdir>\Standard\<sdkversion>\output.xxx`, and
 * `<destdir>\Web\<sdkversion>\output.xxx`, for OTA deployable output.

Configuration
-------------
As with other Albacore tasks, the BlackBerry tasks can read configuration
from an external `.yml` file. This is useful for site-specific configuration
details, such as RIM signing key password, JDE and JAVA paths, etc. You can
also use the [centralized configuration
blocks](https://github.com/derickbailey/Albacore/wiki/Configuration) to reduce
duplicate code.

The configuration files must have the same name as the task, for example,
consider the following task:

    sigtool :sign_cod => [:build] do |s|
        s.codfiles    = FileList[ "deliverables/**/*.cod" ]
    end

Since the task is named `sign_cod`, to set the `password` property, create a
file named `sign_cod.yml` in the same directory as the `Rakefile` and enter the
following:

    password: "<your password>"

Now, you don't have to manually enter the password every time, nor do you have
to worry about accidentally checking it into the version control repository.

Read more about the [Albacore YAML
configuration](https://github.com/derickbailey/Albacore/wiki/YAMLConfig).

Available Tasks
---------------
###RapcManifest task
*Inherits* EntryPoint class

This task generates a `.rapc` manifest file used by the `rapc` BlackBerry
compiler. In the JDE, this file is generated from
`BlackBerry_App_Descriptor.xml` file.

CLDC and MIDLET apps can have multiple entry points, with different icons etc.
If your app has only one entry point, you can set any property available in the
EntryPoint class directly on the RapcManifest class. For additional entry
points, create the necessary EntryPoint objects and set the `entry_points`
property.

Example:

    rapcmanifest :manifest do |m|
        m.sdkversion  = "6.0.0"
        m.output      = "App"
        m.name        = "App"
        m.version     = "1.0.0"
        m.vendor      = "Your Name"
        m.description = "A Cool App"
        m.type        = :CLDC
        m.title       = "App"
        m.icon        = "../../../res/icon.png"
        m.focus_icon  = "../../../res/icon_glow.png"
    end

####Properties

 * **output_file**<br/>
   Path to save the output. Must end in `.rapc`. Optional if sdkversion is
   specified. When both `sdkversion` and `output_file` are specified,
   `output_file` wins.

 * **sdkversion**<br/>
   The version of the SDK that is being targeted. It is used for calculating
   paths, and can be used in lieu of `output_file`. Output will be saved to
   `<destdir>/Standard/<sdkversion>/<output>.rapc`

 * **destdir**<br/>
   *Optional.* Root output directory. Output will be saved to
   `<destdir>/Standard/<sdkversion>/<output>.rapc`. Default is `deliverables`.

 * **output**<br/>
   *Optional.* When used along with destdir and sdkversion, sets the output
   file name. Default is the module's `name`.

 * **name**<br/>
   *Required.* Module name. The name is used to identify app while installing
   and in the Application management screen.

 * **version**<br/>
   *Required.* Module version number. Version number strings must contain only
   numbers and dots.

 * **vendor**<br/>
   *Optional.* Company name. The vendor string appears in the applications
   properties on the device.

 * **description**<br/>
   *Optional.* Module description. The description string appears in the
   application properties on the device.

 * **type**<br/>
   *Optional.* Type of cod file to create. Valid values are :LIBRARY :CLDC,
   :MIDLET. When :MIDLET is specified, the midletclass property is required.
   Default is :CLDC

 * **entry_points** (alias: **entry_point**)<br/>
   A list of EntryPoints.

 * You may also set, any of the properties available in the EntryPoint class.

###EntryPoint class
The EntryPoint class (not a task) represents an entry point. You can use this
class to create additional entry points in a RapcManifest.

Example:

    rapcmanifest :manifest2 do |m|
        m.sdkversion  = "6.0.0"
        m.output      = "App"
        m.name        = "App"
        m.version     = "1.0.0"
        m.vendor      = "Your Name"
        m.description = "A Cool App"
        m.type        = :CLDC
        m.title       = "App"
        m.icon        = "../../../res/icon.png"
        m.focus_icon  = "../../../res/icon_glow.png"

        ep            = EntryPoint.new()
        ep.title      = "App One"
        e.argument    = "-hello"
        ep.icon       = "../../../res/icon_1.png"
        ep.focus_icon = "../../../res/icon_glow_1.png"

        ep2            = EntryPoint.new()
        ep2.title      = "App Two"
        ep2.arguments  = ["-hello", "-goodbye"]
        ep2.icon       = "../../../res/icon_2.png"
        ep2.focus_icon = "../../../res/icon_glow_2.png"

        m.entry_point = [ep, ep2]
    end

####Properties

 * **title**<br/>
   Module title. The title appears below the icon and in the applications list
   on the device.

 * **system_module**<br/>
   Set to true for a system module. System modules run in the background and do
   not have an icon on the home screen.

 * **run_on\_startup**<br/>
   Set to true if application should start when device starts.

 * **startup_tier**<br/>
   Startup priority relative to other applications. Valid values are 0-7, lower
   value = higher priority.

 * **ribbon_position**<br/>
   Position of icon in ribbon. Higher values move the icon closer to the top of
   the ribbon.

 * **name_resource\_bundle**<br/>
   Name of resource bundle that contains the module title, eg:
   ca.slashdev.MyAppResources

 * **name_resource\_id**<br/>
   The id of the resource that contains the module title, eg: 1234.

 * **midlet_class**<br/>

 * **icons** (alias **icon**)<br/>
   A list of icons to be display on the ribbon.

 * **focus_icons** (alias **focus_icon**)<br/>
   A list of icons that appear on hover/focus on the ribbon. Icon paths must be
   relative to the `output_file`.

 * **arguments** (alias **argument**)<br/>
   A list of arguments passed to the main method. Icon paths must be relative
   to the `output_file`.

###Rapc task
The Rapc task runs the BlackBerry compiler. It supports most commonly used
options of the compiler. The compiler task also simplifies building different
apps targeting different BlackBerry operating systems from the same code base.

Example:

    rapc :build => [:manifest, :asminfo] do |r|
        r.output     = "App"
        r.sdkversion = "6.0.0"
        r.createweb  = true
        r.nowarn     = true
        r.sources    = FileList["src/**/*.java", "res/**/*"]
    end

####Properties

 * **command**<br/>
   Path to the `rapc` executable. Usually automatically discovered.

 * **jdkhome**<br/>
   *Optional.* Path to the Java SDK installation. If not specified, the
   `JDK_HOME` or `JAVA_HOME` environment variables will be used.

 * **jdehome**<br/>
   *Optional.* Path to the RIM BlackBerry JDE installation. If not specified,
   the `JDE_HOME` environment variable will be used.

 * **exepath**<br/>
   *Optional.* Explicitly define the directory containing the preverify command
   used by the rapc compiler. When not defined, the system's default `PATH` is
   used.

 * **destdir**<br/>
   *Optional.* Root output directory of .cod file. Standard output will be
   saved to `<destdir>/Standard/<sdkversion>`. Default is `deliverables`.

 * **quiet**<br/>
   *Optional.* Tells the rapc command to be less chatty, default is *false*.

 * **verbose**<br/>
   *Optional.* Turn on verbose output of the rapc compiler, default is *false*.

 * **nodebug**<br/>
   *Optional.* Turn off generation of .debug files, default is *false*.

 * **nowarn**<br/>
   *Optional.* Disable warning messages, default is *false*.

 * **warnerror**<br/>
   *Optional.* Treat all warnings as errors, default is *false*.

 * **noconvert**<br/>
   *Optional.* Don't convert images to png, default is *false*.

 * **nopreverify**<br/>
   *Optional.* Do not call the preverify executable, default is *false*.

 * **createweb**<br/>
   *Optional.* If true, extracts OTA compatible cods to
   `<destdir>/Standard/<sdkversion>`, default is *false*.

 * **rapcfile**<br/>
   *Optional.* The rapc spec file (optional). Default is
   `<destdir>/output.rapc`

 * **output**<br/>
   *Required.* The name out the output files.

 * **sdkversion**<br/>
   *Required.* The version of the SDK that is targeted. It is used for
   calculating paths and declaring tags. eg: "5.0.0".

 * **source** (Alias **sources**)<br/>
   *Required.* A list of all source files and resource files. Required.

 * **imports**<br/>
   *Optional.* A list of `.jar` files to import. The appropriate
   `net_rim_api.jar` for the sdkversion will be imported by default.

 * **tags**<br/>
   *Optional.* List of preprocessor tags. By default the `PREPROCESSOR` tag and
   a tag named `BlackBerrySDK<sdkversion>` will be declared. Set to nil to
   disable all tags.

###SigTool task
The SigTool task runs the RIM sigtool and signs the cod files generated by the Rapc.

Example:

    sigtool :sign_cod => [:build] do |s|
        s.codfiles    = FileList[ "deliverables/**/*.cod" ]
    end

####Properties

 * **command**<br/>
   Path to the `java` executable. Usually automatically discovered.

 * **jdkhome**<br/>
   *Optional.* Path to the Java or Java SDK installation. If not specified, the
   `JDK_HOME` or `JAVA_HOME` environment variables will be used.

 * **jdehome**<br/>
   *Optional.* Path to the RIM BlackBerry JDE installation. If not specified,
   the `JDE_HOME` environment variable will be used.

 * **sigtooljar**<br/>
   *Optional.* The path to the signature tool jar file. If not set, the
   `JDE_HOME` environment variable will be used to find the sigtool.

 * **forceclose**<br/>
   *Optional.* Close signature tool even if signature failed. Default is
   *false*.

 * **close**<br/>
   *Optional.* Close after signature request if no errors have occurred.
   Default is *true*.

 * **request**<br/>
   *Optional.* Request signature when application launches. Default is *true*.

 * **password**<br/>
   *Optional.* The password for the signing keys. Removes the interactive
   password prompt. This implicitly sets "close" and "request" to *true*.

 * **codfile** (Alias **codfiles**)<br/>
   *Required.* Cod files to sign.

###JavaAsmInfo task
Creates a Java class with some compile-time information. This file can be used
in manner similar to the .NET AssemblyInfo file. There is no built-in support
for using the variables in the generated class. You must manually use the properties
of the generated class instead of using string literals.

Example:

    desc "Create a java class with information about the app"
    javaassemblyinfo :asminfo do |j|
        j.title        = "App"
        j.description  = "A Cool App"
        j.version      = "1.0.0"
        j.copyright    = "(C) 2011 BB Rake Team"
        j.class_name   = "AssemblyInfo"
        j.package_name = "com.bbrake"
        j.output_dir   = File.expand_path("src/com/bbrake")
    end

####Properties

 * **title**<br/>
   The title of the app.

 * **description**<br/>
   The description of the app.

 * **copyright**<br/>
   Copyright notice.

 * **version**<br/>
   Version

 * **output_dir**<br/>
   The folder to save the output. You should organize by the package name. (eg:
   src/com/google/)

 * **class_name**<br/>
   The name of the class. Default is "AssemblyInfo".

 * **package_name**<br/>
   The name of the package. If not specified, package keyword will be omitted.

