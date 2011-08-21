Notes On Building Wireless Music Sync for BlackBerry&reg; App
=============================================================
The App supports both BlackBerry OS 5 and 6.

The app can be built using [BlackBerry Eclipse JDE 1.3](http://us.blackberry.com/developers/javaappdev/javaplugin.jsp)

Recommended Settings
--------------------
 1. Disable automatic build.
    1. Uncheck on **Project->Build Automatically**
 2. Pay no attention to the check-as-you type errors in Eclipse. Check the 
    **Problems** panel for actual errors from the `rapc` compiler.

Changing Target JRE
-------------------
 1. Right click on **JRE System Library** in the Package Explorer.
 2. Click on **Properties**.
 3. From the *Alternate JRE* drop down, select **BlackBerry JRE x.x.x**, where
    x.x.x is the target version.

org.json.me Issues
------------------
SDK 6.0.0 includes `org.json.me` package and the source files must be excuded
from the build for the compiling to work.

### For OS 6.0.0 Builds
 1. Right click on **org.json.me** package in the Package Explorer.
 2. Click on **Build Path->Exclude** to exclude it from build.

### For OS 5.0.0 Builds
 1. Right click on *any item* in the Package Explorer.
 2. Click on **Build Path->Configure Build Path...**.
 3. In the *Source* tab Under the *.../src* folder, find the entry named
   `Excluded: org/json/me` and remove it. It should now read `Excluded: (None)`.
 4. Click **OK**.

The changes are saved in the file `.classpath` at the project root.
