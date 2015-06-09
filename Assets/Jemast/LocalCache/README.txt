------------------------------------------
Fast Platform Switch
v1.7.1
Codename: JLocalCachePlugin
Product Page: http://jemast.com/unity/fast-platform-switch/
Copyright (c) 2013-2015 jemast software.
------------------------------------------


------------------------------------------
Quick Start
------------------------------------------

After importing Fast Platform Switch from the Asset Store, open the Window menu and click Fast Platform Switch.

You must now exclusively switch platforms using the Fast Platform Switch interface and not rely on the Build Settings platform list.

Pick a platform and hit the Switch Platform button. That's it. The plugin will do the rest.

------------------------------------------
About Fast Platform Switch
------------------------------------------

Fast Platform Switch is not meant to replace Unity's Cache Server. It's meant as a personal caching utility for individuals and small teams. If you're already using Unity's Cache Server, you should not use Fast Platform Switch as this means you'll cache your data twice and will probably loose time and disk space.

Because Fast Platform Switch caches data for each platform at switch time, this means it takes up valuable disk space which can become quite large depending on your project size. We've included a Status panel to check the status of your cache and the size it takes up.

We've also included a compression mechanism to save space. While this mechanism will slow down a bit the switch time due to decompression time, the savings in terms of disk space are generally around 50% which is also significant. Compression happens in the background after you perform a platform switch. While this allows you to continue working as soon as the switch happened, it may incur slowdowns as it eats up CPU resources.

Finally, because it creates and operates the cache folder in your project directory, we've also included convenient methods to ignore this folder for popular version control mechanisms (Git and Mercurial). We've also added instructions for other version control mechanisms.


------------------------------------------
Help & Support
------------------------------------------

Check out our documentation at http://jemast.com/unity/fast-platform-switch/documentation/

If you need any additional help, please review our product page at http://jemast.com/unity/fast-platform-switch/

Feel free to contact us at contact@jemast.com for personal support


-------------------------------------------
Release Notes
-------------------------------------------

-------------------
v1.7.1 (02/12/2015)
-------------------

- ABOUT: When updating a project to Unity 5, it is recommended you clear all cache
- FIX: Fixed hangs in Unity 5 happening randomly after switching due to eager UI refresh
- FIX: Fixed trying to persist changes on files that are not project assets (no more warnings in Unity 5)

-------------------
v1.7 (11/09/2014)
-------------------

- NEW: Settings can now be instantiated per project (system-wide remains default)
- NEW: Cache directory can now be changed and placed anywhere (using per project settings)
- NEW: Preliminary Unity 5.0 beta support (WebGL platform support and scripting fixes)
- FIX: Better namespace isolation to prevent collisions with other plugins
- FIX: More leeway for the editor to update in the overall process should prevent rare issues
- ABOUT: Unity 4.6 RC is already fully supported

-------------------
v1.6 (08/09/2014)
-------------------

- NEW: You can now set Architecture (e.g. Windows / OS X) when switching from UI (has no influence on cache, productivity boost only)
- FIX: Removed the last progress bar waiting for compilation as it could hang on OS X until Unity's window lost focus
- UPDATE: Updated compression utility
- UPDATE: Full code review, better adherence to C# standards and best practices

-------------------
v1.5.4 (07/22/2014)
-------------------

- FIX: Better recovery from failed switches (should prevent the rare blank window with errors bug)

-------------------
v1.5.3 (07/03/2014)
-------------------

- FIX: Fixed API switch not behaving correctly on platforms with texture compression options when no option is passed
- NEW: Double-click on a platform in the list to perform the switch

-------------------
v1.5.2 (06/01/2014)
-------------------

- NEW: Added generic texture compression option (aka Don't override) for Android and BlackBerry
- FIX: Workaround for an issue in Unity Free to force texture reimport in build subtargets (texture compression options)

-------------------
v1.5.1 (05/29/2014)
-------------------

- FIX: API switch requests are now properly persisted and will go through after calling BuildPipeline.BuildPlayer
- FIX: When a platform switch fails, cleanup will now happen immediately (cache being in an unknown state, it has to be cleared for that platform)
- UPDATE: Tightened the overall checks regarding the current editor state

-------------------
v1.5 (05/18/2014)
-------------------

- IMPORTANT: Please Clear all cache and do a Reimport all on your projects due to a breaking change in cache manager
- IMPORTANT: Dropped support for Unity 3.5.7 (Unity 4.0 is the minimal requirement now)

- UPDATE: Improved cache manager that should prevent small issues reported by some users
- UPDATE: Now using StartAssetEditing and StopAssetEditing to batch reimports
- UPDATE: Tweaked the UI a bit for readability and clarity

-------------------
v1.4.5 (04/18/2014)
-------------------

- NEW: Support for Unity 4.5 new platforms (removed deprecated ones as well)

-------------------
v1.4.4 (04/14/2014)
-------------------

- FIX: Fixed the Mercurial rule for preventing the cache to go into version control
- FIX: Removed Flash platform for Unity 4.3 (not available anymore)
- FIX: Support for ISO-8859-1 paths with hard links on Windows

-------------------
v1.4.3 (03/31/2014)
-------------------

- FIX: Fixed a critical regression in v1.4.1 with directory asset reimport

-------------------
v1.4.2 (03/28/2014)
-------------------

- FIX: Prevent issues when accidentaly switching with default Build settings and hard links are enabled

-------------------
v1.4.1 (03/27/2014)
-------------------

- FIX: Do not attempt to import directories as assets
- FIX: Fixed directory delete issues and related cleanup tasks
- FIX: No more issues if you cancel a platform change
- FIX: Proper disposal of external processes after they exit
- FIX: Better indication of platform switch start (progress bar when persisting assets to disk)

-------------------
v1.4 (03/21/2014)
-------------------

- NEW: Experimental hard links support! No more moving the cache around, only folder links are changed. Feature is marked as experimental because hard links can fail under certain conditions (network shares, unsupported file systems, ...).
- FIX: When reimporting assets, progress bar will output asset file names.
- FIX: Prevent import of files inside hidden folders (as Unity ignores them).
- FIX: Prevent import of Thumbs.db files (sorry about that).
- FIX: Fixed path issues with compression. We are also now logging failed compressions and doing some cleanup.

-------------------
v1.3.3 (03/10/2014)
-------------------

- FIX: Ignore hidden files when trying to persist asset settings (props to Glauber)

-------------------
v1.3.2 (03/07/2014)
-------------------

- FIX: Fixed namespaces issues (props to Larry)

-------------------
v1.3.1 (01/19/2014)
-------------------

- FIX: Fixed texture refresh not being triggered when changing texture compression in Android or BB10 on Unity 4.3+
- FIX: Textures from Fast Platform Switch window do not get reimported after each platform switch
- FIX: Paths to required assets and tools are now relative and you should be able to move the Jemast folder around

-------------------
v1.3 (01/13/2014)
-------------------

- NEW: API to integrate Fast Platform Switch into your custom pipeline

-------------------
v1.2.5 (11/13/2013)
-------------------

- NEW: Full Unity 4.3 support
- NEW: Added support for Unity 4.3+ hidden meta files
- NEW: Added support for ASTC texture compression for Android (Unity 4.3+)
- FIX: Fixed incorrect automatic Mercurial rule for version control
- FIX: Attempted to fix UI lockup where switch is not happening

-------------------
v1.2 (11/04/2013)
-------------------

- NEW: Support for Blackberry texture compression options (ETC1, ATC, PVRTC)
- NEW: Verbose mode to help debugging issues (log file)
- NEW: Option to unlock the plugin UI should switch or compression operation go wrong and UI is stuck
- FIX: New method for forcing texture refresh on subtarget change (texture compression for Android & BB10)
- FIX: Fixed plugin remaining locked if there are script compilation errors after switching platform
- FIX: Timestamps now get correctly deleted when deleting a specific platform cache
- FIX: Fixed some minor UI quirks

-------------------
v1.1 (10/13/2013)
-------------------

- CRITICAL FIX: You are now prompted to enable external version control in your project as this is required
- FIX: Asset importers are correctly saved to disk before switching
- FIX: Asset timestamps are now written to cache in order to fix issues
- FIX: Fixed an innocuous error in Unity 3 branch after switch
- As a result of those fixes, cache is deleted once upon update
- Please restart Unity after applying this update

-------------------
v1.0.2 (10/12/2013)
-------------------

- FIX: Cancel now working when asking to save scene before switching
- UI: Clearer dialog box for saving scene before switching

-------------------
v1.0.1 (10/09/2013)
-------------------

- FIX: New platform is now correctly selected for build after switch
- FIX: Removed Native Client as an independent platform for Unity 3 branch
- Please restart Unity after applying this update

-------------------
v1.0 (09/15/2013)
-------------------

- Initial release.



jemast software



-------------------------------------------
Third-party acknowledgments
-------------------------------------------

lz4 Command line utility
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Fast compression algorithm
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Copyright (c) 2013 Yann Collet.

-------------------------------------------

Fast Platform Switch logo uses various icons from The Noun Project

Computer by The Noun Project (Designed by Edward Boatman)
http://thenounproject.com/noun/computer/#icon-No115

iPhone by The Noun Project (Designed by Edward Boatman)
http://thenounproject.com/noun/iphone/#icon-No414