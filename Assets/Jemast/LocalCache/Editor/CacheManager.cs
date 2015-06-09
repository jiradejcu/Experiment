//
//  CacheManager.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Jemast.LocalCache
{
    public static class CacheManager
    {
        private static string _progressTitle;
        private static string _progressDescription;
        private static float _progress;

        private static bool? _platformRefreshInProgress;

        private static bool? _switchOperationInProgress;

        private static bool? _switchOperationIsApi;


        private static string _platformRefreshCurrentScene;

        private static bool? _platformRefreshShouldBustCache;

        public static bool ShouldSerializeAssets;
        public static bool ShouldSwitchPlatform;

        public static bool HasCheckedHardLinkStatus;
        public static bool ShouldPerformCleanup;

        private static Shared.CacheTarget? _newCacheTarget;
        private static Shared.CacheSubtarget? _newCacheSubtarget;

        public static Shared.CacheTarget? CurrentCacheTarget
        {
            get { return Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget); }
        }

        public static Shared.CacheSubtarget? CurrentCacheSubtarget
        {
            get
            {
                if (CurrentCacheTarget == Shared.CacheTarget.Android)
                    return Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                if (CurrentCacheTarget == Shared.CacheTarget.BlackBerry)
                    return
                        Shared.CacheSubtargetForBlackBerryBuildSubtarget(
                            EditorUserBuildSettings.blackberryBuildSubtarget);
#endif
                return null;
            }
        }

        public static bool PlatformRefreshInProgress
        {
            get
            {
                if (_platformRefreshInProgress == null)
                {
                    _platformRefreshInProgress = File.Exists(Preferences.CachePath + "PlatformRefreshInProgress");
                }

                return _platformRefreshInProgress.Value;
            }
            set
            {
                if (value)
                {
                    FileStream s = File.Create(Preferences.CachePath + "PlatformRefreshInProgress");
                    s.Dispose();
                }
                else
                {
                    File.Delete(Preferences.CachePath + "PlatformRefreshInProgress");
                }

                _platformRefreshInProgress = value;
            }
        }

        public static bool SwitchOperationInProgress
        {
            get
            {
                if (_switchOperationInProgress == null)
                {
                    _switchOperationInProgress = File.Exists(Preferences.CachePath + "SwitchOperationInProgress");
                }

                return _switchOperationInProgress.Value;
            }
            set
            {
                if (value)
                {
                    FileStream s = File.Create(Preferences.CachePath + "SwitchOperationInProgress");
                    s.Dispose();
                }
                else
                {
                    File.Delete(Preferences.CachePath + "SwitchOperationInProgress");
                }

                _switchOperationInProgress = value;
            }
        }

        public static bool SwitchOperationIsApi
        {
            get
            {
                if (_switchOperationIsApi == null)
                {
                    _switchOperationIsApi = File.Exists(Preferences.CachePath + "SwitchOperationIsAPI");
                }

                return _switchOperationIsApi.Value;
            }
            set
            {
                if (value)
                {
                    FileStream s = File.Create(Preferences.CachePath + "SwitchOperationIsAPI");
                    s.Dispose();
                }
                else
                {
                    File.Delete(Preferences.CachePath + "SwitchOperationIsAPI");
                }

                _switchOperationIsApi = value;
            }
        }

        public static string PlatformRefreshCurrentScene
        {
            get
            {
                if (_platformRefreshCurrentScene != null)
                    return _platformRefreshCurrentScene;

                string filePath = Preferences.CachePath + "PlatformRefreshCurrentScene";
                if (File.Exists(filePath))
                {
                    _platformRefreshCurrentScene = File.ReadAllText(filePath);
                    return _platformRefreshCurrentScene;
                }

                _platformRefreshCurrentScene = "";
                return _platformRefreshCurrentScene;
            }
            set
            {
                string filePath = Preferences.CachePath + "PlatformRefreshCurrentScene";

                if (string.IsNullOrEmpty(value))
                {
                    File.Delete(filePath);
                }
                else
                {
                    File.WriteAllText(filePath, value);
                }
            }
        }

        public static bool? PlatformRefreshShouldBustCache
        {
            get
            {
                if (_platformRefreshShouldBustCache != null)
                    return _platformRefreshShouldBustCache;

                string filePath = Preferences.CachePath + "PlatformRefreshShouldBustCache";
                if (File.Exists(filePath))
                {
                    _platformRefreshShouldBustCache = true;
                    return _platformRefreshShouldBustCache;
                }

                _platformRefreshShouldBustCache = false;
                return _platformRefreshShouldBustCache;
            }
            set
            {
                if (value == true)
                {
                    FileStream s = File.Create(Preferences.CachePath + "PlatformRefreshShouldBustCache");
                    s.Dispose();
                }
                else
                {
                    File.Delete(Preferences.CachePath + "PlatformRefreshShouldBustCache");
                }

                _platformRefreshShouldBustCache = value;
            }
        }

        public static bool BackgroundCacheCompressionInProgress
        {
            get { return File.Exists(Preferences.CachePath + "Background.txt"); }
        }

        public static void SwitchPlatform(Shared.CacheTarget? cacheTarget, Shared.CacheSubtarget? cacheSubtarget,
            bool apiSwitch)
        {
            if (cacheTarget == CurrentCacheTarget && cacheSubtarget == CurrentCacheSubtarget) return;

            BuildTarget? newBuildTarget = Shared.BuildTargetForCacheTarget(cacheTarget);

            if (CurrentCacheTarget == null || newBuildTarget == null || cacheTarget == null ||
                (newBuildTarget == EditorUserBuildSettings.activeBuildTarget && CurrentCacheSubtarget == cacheSubtarget))
                return;

            // Make sure we have a cache subtarget for platforms that require theme
            if (cacheTarget == Shared.CacheTarget.Android && cacheSubtarget == null)
            {
                cacheSubtarget = Shared.CacheSubtarget.Android_GENERIC;
            }
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            else if (cacheTarget == Shared.CacheTarget.BlackBerry && cacheSubtarget == null)
            {
                cacheSubtarget = Shared.CacheSubtarget.BlackBerry_GENERIC;
            }
#endif

            if (apiSwitch == false)
            {
                int option =
                    EditorUtility.DisplayDialogComplex(
                        "Do you want to save the changes you made in the scene " +
                        (string.IsNullOrEmpty(EditorApplication.currentScene)
                            ? "Untitled"
                            : EditorApplication.currentScene) + "?",
                        "Platform switching requires closing the current scene to process cache. Your changes will be lost if you don't save them.",
                        "Save", "Don't Save", "Cancel");
                switch (option)
                {
                    case 0:
                        EditorApplication.SaveScene();
                        break;
                    case 1:
                        break;
                    case 2:
                        return;
                    default:
                        return;
                }
            }

            LogUtility.LogImmediate("-------------------------------------------------------");
            LogUtility.LogImmediate("Switching from {0} ({1}) to {2} ({3})", CurrentCacheTarget.ToString(),
                CurrentCacheSubtarget.HasValue ? CurrentCacheSubtarget.Value.ToString() : "Base", cacheTarget,
                cacheSubtarget.HasValue ? cacheSubtarget.Value.ToString() : "Base");

            EditorUtility.DisplayProgressBar("Hold on", "Persisting assets and import settings to disk...", 0.5f);

            PlatformRefreshCurrentScene = EditorApplication.currentScene;
            EditorApplication.NewScene();

            _newCacheTarget = cacheTarget;
            _newCacheSubtarget = cacheSubtarget;

            SwitchOperationInProgress = true;
            SwitchOperationIsApi = apiSwitch;
            PlatformRefreshInProgress = true;
            ShouldSerializeAssets = true;
        }

        public static void SerializeAssetsOperation()
        {
            ShouldSerializeAssets = false;

            // Save assets
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            // Save all import manager timestamps
            Directory.CreateDirectory(Preferences.CachePath);

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            LogUtility.LogImmediate("Writing import settings to disk and fetching assets timestamps");

            var timestamps = new Dictionary<string, ulong>();

            foreach (string path in assetPaths)
            {
                if (Path.IsPathRooted(path)) continue;

                AssetDatabase.WriteImportSettingsIfDirty(path);

                AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                if (assetImporter != null)
                {
                    timestamps.Add(path, assetImporter.assetTimeStamp);
                }
            }

            LogUtility.LogImmediate("Serializing asset timestamps to disk");

            string currentTargetMetadataTimestampsPath = Preferences.CachePath +
                                                         Shared.CacheTargetPrefixes[(int) CurrentCacheTarget] +
                                                         (CurrentCacheSubtarget.HasValue
                                                             ? Shared.CacheSubtargetPrefixes[(int) CurrentCacheSubtarget
                                                                 ]
                                                             : "") + "timestamps";
            using (FileStream destFileStream = File.Create(currentTargetMetadataTimestampsPath))
            {
                SerializeTimestamps(timestamps, destFileStream);
            }

            ShouldSwitchPlatform = true;
        }

        public static void SwitchPlatformOperation()
        {
            ShouldSwitchPlatform = false;
            //ShouldRefreshUI = true;

            BuildTarget? newBuildTarget = Shared.BuildTargetForCacheTarget(_newCacheTarget);
            BuildTargetGroup? newBuildTargetGroup = Shared.BuildTargetGroupForCacheTarget(_newCacheTarget);

            EditorApplication.NewScene();

            PlatformRefreshShouldBustCache = (_newCacheSubtarget != null && CurrentCacheTarget == _newCacheTarget && CurrentCacheSubtarget != _newCacheSubtarget);

            string assetsPath = Application.dataPath;

            string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";

            Directory.CreateDirectory(Preferences.CachePath);

            string metadataPath = libraryPath + "/metadata";

            string currentTargetMetadataDirectoryPath = Preferences.CachePath +
                                                        Shared.CacheTargetPrefixes[(int) CurrentCacheTarget] +
                                                        (CurrentCacheSubtarget.HasValue
                                                            ? Shared.CacheSubtargetPrefixes[(int) CurrentCacheSubtarget]
                                                            : "") + "metadata";
            string newTargetMetadataDirectoryPath = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) _newCacheTarget] +
                                                    (_newCacheSubtarget.HasValue
                                                        ? Shared.CacheSubtargetPrefixes[(int) _newCacheSubtarget]
                                                        : "") + "metadata";

            string currentTargetMetadataTimestampsPath = Preferences.CachePath +
                                                         Shared.CacheTargetPrefixes[(int) CurrentCacheTarget] +
                                                         (CurrentCacheSubtarget.HasValue
                                                             ? Shared.CacheSubtargetPrefixes[(int) CurrentCacheSubtarget
                                                                 ]
                                                             : "") + "timestamps";
            string newTargetMetadataTimestampsPath = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) _newCacheTarget] +
                                                     (_newCacheSubtarget.HasValue
                                                         ? Shared.CacheSubtargetPrefixes[(int) _newCacheSubtarget]
                                                         : "") + "timestamps";

            LogUtility.LogImmediate("Attempting to perform switch operation");

            try
            {
                // Preemptively get a list of assets
                string[] assetPaths = AssetDatabase.GetAllAssetPaths();

                LogUtility.LogImmediate("Performing decompression of current build target if required");
                CompressionManager.PerformDecompression(currentTargetMetadataDirectoryPath);

                LogUtility.LogImmediate("Performing decompression of new build target if required");
                CompressionManager.PerformDecompression(newTargetMetadataDirectoryPath);

                if (Preferences.EnableHardLinks)
                {
                    EditorUtility.DisplayProgressBar("Hold on", "Swapping hard links...", 0.5f);
                    LogUtility.LogImmediate("Swapping hard links");

                    // If platform is not cached copy data from current platform
                    if (Directory.Exists(newTargetMetadataDirectoryPath) == false)
                    {
                        Shared.DirectoryCopy(metadataPath, newTargetMetadataDirectoryPath, true);
                    }

                    // Swap hard link
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        // Make hardlink process executable
                        var process = new Process
                        {
                            StartInfo =
                            {
                                FileName = "chmod",
                                Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths + "hardlink\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Delete current hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                Arguments = "-u \"" + metadataPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Make new hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                Arguments = "\"" + newTargetMetadataDirectoryPath + "\" \"" + metadataPath +
                                            "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();
                    }
                    else
                    {
                        // Delete current hard link
                        var process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                Arguments = "/accepteula -d \"" + metadataPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Make new hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                Arguments = "/accepteula \"" + metadataPath + "\" \"" +
                                            newTargetMetadataDirectoryPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();
                    }
                }
                else
                {
                    // Save current platform
                    LogUtility.LogImmediate("Merging current build target to cache");

                    _progressTitle = "Hold on";
                    _progressDescription = "Saving current platform to cache...";
                    _progress = 0f;
                    DirectoryMerge(metadataPath, currentTargetMetadataDirectoryPath);

                    LogUtility.LogImmediate("Performing decompression of new build target if required");

                    // Override with new platform
                    LogUtility.LogImmediate("Merging new build target to cache");

                    _progressTitle = "Hold on";
                    _progressDescription = "Attempting to load previous platform from cache...";
                    _progress = 0f;
                    DirectoryMerge(newTargetMetadataDirectoryPath, metadataPath);
                }

                // Chmod/Chown metadata
                LogUtility.LogImmediate("Setting correct permissions on metadata");
                FixPermissions();

                LogUtility.LogImmediate("Deleting excess metadata files");

                // Look for excess metadata files
                string[] metadataFiles = Directory.GetFiles(metadataPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in metadataFiles)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(Path.GetFileName(file));
                    if (assetPath.StartsWith("Assets") &&
                        (string.IsNullOrEmpty(assetPath) ||
                         (!File.Exists(Shared.ProjectPath + assetPath) &&
                          !Directory.Exists(Shared.ProjectPath + assetPath))))
                    {
                        File.Delete(file);
                    }
                }

                LogUtility.LogImmediate("Looking for invalid/expired timestamps");

                // Look for invalid timestamps
                if (File.Exists(currentTargetMetadataTimestampsPath) && File.Exists(newTargetMetadataTimestampsPath))
                {
                    Dictionary<string, ulong> currentTimestamps;
                    Dictionary<string, ulong> newTimestamps;

                    using (FileStream stream = File.OpenRead(currentTargetMetadataTimestampsPath))
                    {
                        currentTimestamps = DeserializeTimestamps(stream);
                    }

                    using (FileStream stream = File.OpenRead(newTargetMetadataTimestampsPath))
                    {
                        newTimestamps = DeserializeTimestamps(stream);
                    }

                    foreach (var kv in newTimestamps)
                    {
                        if (currentTimestamps.ContainsKey(kv.Key) && (currentTimestamps[kv.Key] != kv.Value))
                        {
                            string guid = AssetDatabase.AssetPathToGUID(kv.Key);
                            File.Delete(metadataPath + "/" + guid.Substring(0, 2) + "/" + guid);
                        }
                    }
                }

                LogUtility.LogImmediate("Perform actual switch");

                // Perform switch
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
                if (_newCacheTarget == Shared.CacheTarget.Android)
                {
                    MobileTextureSubtarget? androidSubtarget =
                        Shared.AndroidBuildSubtargetForCacheSubtarget(_newCacheSubtarget);
                    if (androidSubtarget.HasValue)
                        EditorUserBuildSettings.androidBuildSubtarget = androidSubtarget.Value;
                }
                else if (_newCacheTarget == Shared.CacheTarget.BlackBerry)
                {
                    MobileTextureSubtarget? blackberrySubtarget =
                        Shared.BlackBerryBuildSubtargetForCacheSubtarget(_newCacheSubtarget);
                    if (blackberrySubtarget.HasValue)
                        EditorUserBuildSettings.blackberryBuildSubtarget = blackberrySubtarget.Value;
                }
#else
                if (_newCacheTarget == Shared.CacheTarget.Android)
                {
                    AndroidBuildSubtarget? androidSubtarget =
                        Shared.AndroidBuildSubtargetForCacheSubtarget(_newCacheSubtarget);
                    if (androidSubtarget.HasValue)
                        EditorUserBuildSettings.androidBuildSubtarget = androidSubtarget.Value;
                }
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                else if (_newCacheTarget == Shared.CacheTarget.BlackBerry)
                {
                    BlackBerryBuildSubtarget? blackberrySubtarget =
                        Shared.BlackBerryBuildSubtargetForCacheSubtarget(_newCacheSubtarget);
                    if (blackberrySubtarget.HasValue)
                        EditorUserBuildSettings.blackberryBuildSubtarget = blackberrySubtarget.Value;
                }
#endif
#endif

                EditorUserBuildSettings.SwitchActiveBuildTarget(newBuildTarget.Value);

                if (EditorUserBuildSettings.activeBuildTarget != newBuildTarget.Value)
                    throw new Exception();

                // Make as selected build target group
                if (newBuildTargetGroup.HasValue)
                    EditorUserBuildSettings.selectedBuildTargetGroup = newBuildTargetGroup.Value;

                switch (_newCacheTarget.Value)
                {
                    case Shared.CacheTarget.Standalone:
                        EditorUserBuildSettings.selectedStandaloneTarget = newBuildTarget.Value;
                        break;
                    case Shared.CacheTarget.WebPlayer:
                        EditorUserBuildSettings.webPlayerStreamed = newBuildTarget.Value == BuildTarget.WebPlayerStreamed;
                        break;
                }

                // Platform refresh
                SwitchOperationInProgress = true;
                PlatformRefreshInProgress = true;

                LogUtility.LogImmediate("Looking for missing metadata files");

                // Look for missing metadata files
                int currentAsset = 0;
                int assetCount = assetPaths.Length;

                AssetDatabase.StartAssetEditing();

                foreach (string path in assetPaths)
                {
                    if (Path.IsPathRooted(path)) continue;

                    EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets...",
                        currentAsset/(float) assetCount);

                    string filename = Path.GetFileName(path);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    if (path.StartsWith("Assets") && !string.IsNullOrEmpty(guid) &&
                        !File.Exists(metadataPath + "/" + guid.Substring(0, 2) + "/" + guid))
                    {
                        EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets... (" + filename + ")",
                            currentAsset/(float) assetCount);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                    currentAsset++;
                }

                EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets... (processing batch)", 1.0f);
                AssetDatabase.StopAssetEditing();

                EditorUtility.ClearProgressBar();

                // Refresh asset database
                AssetDatabase.Refresh();

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                LogUtility.LogImmediate("Switch operation exception: ", e.Message);

                Debug.LogError(e.Message);

                EditorUtility.ClearProgressBar();

                SwitchOperationInProgress = false;

                if (Preferences.EnableHardLinks)
                {
                    // Check if platform switch happened -- Get current target
                    Shared.CacheTarget? currentCacheTarget =
                        Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
                    Shared.CacheSubtarget? currentCacheSubtarget = null;

                    if (currentCacheTarget == Shared.CacheTarget.Android)
                        currentCacheSubtarget =
                            Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                    else if (currentCacheTarget == Shared.CacheTarget.BlackBerry)
                        currentCacheSubtarget =
                            Shared.CacheSubtargetForBlackBerryBuildSubtarget(
                                EditorUserBuildSettings.blackberryBuildSubtarget);
#endif

                    // Revert hard links
                    if (currentCacheTarget != _newCacheTarget || currentCacheSubtarget != _newCacheSubtarget)
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            // Make hardlink process executable
                            var process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = "chmod",
                                    Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths +
                                                "hardlink\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            process.Start();
                            process.WaitForExit();
                            process.Dispose();

                            // Delete current hard link
                            process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                    Arguments = "-u \"" + metadataPath + "\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            process.Start();
                            process.WaitForExit();
                            process.Dispose();

                            // Make new hard link
                            process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                    Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" +
                                                metadataPath + "\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            process.Start();
                            process.WaitForExit();
                            process.Dispose();
                        }
                        else
                        {
                            // Delete current hard link
                            var process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                    Arguments = "/accepteula -d \"" + metadataPath + "\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            process.Start();
                            process.WaitForExit();
                            process.Dispose();

                            // Make new hard link
                            process = new Process
                            {
                                StartInfo =
                                {
                                    FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                    Arguments = "/accepteula \"" + metadataPath + "\" \"" +
                                                currentTargetMetadataDirectoryPath + "\"",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                }
                            };
                            process.Start();
                            process.WaitForExit();
                            process.Dispose();
                        }

                        // Mark for cleanup
                        File.Create(newTargetMetadataDirectoryPath + ".cleanup");
                    }
                }

                return;
            }

            SwitchOperationInProgress = false;
            LogUtility.LogImmediate("Switch operation successful");
        }

        public static bool GetCacheStatus(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget)
        {
            if (!target.HasValue)
                return false;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";
            string metadataCacheLz4File = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                          (subtarget.HasValue
                                              ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                              : "") + "metadata.jcf.lz4";

            return Directory.Exists(metadataCacheFolder) || File.Exists(metadataCacheLz4File);
        }

        public static bool GetCacheCompression(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget)
        {
            if (!target.HasValue)
                return false;

            string metadataCacheLz4File = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                          (subtarget.HasValue
                                              ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                              : "") + "metadata.jcf.lz4";

            return File.Exists(metadataCacheLz4File);
        }

        public static DateTime GetCacheDate(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget)
        {
            if (!target.HasValue)
                return DateTime.Now;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";
            string metadataCacheLz4File = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                          (subtarget.HasValue
                                              ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                              : "") + "metadata.jcf.lz4";

            return File.Exists(metadataCacheLz4File)
                ? File.GetLastWriteTime(metadataCacheLz4File)
                : Directory.GetLastWriteTime(metadataCacheFolder);
        }

        public static long GetCacheSize(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget)
        {
            if (!target.HasValue)
                return -1;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";
            string metadataCacheLz4File = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                          (subtarget.HasValue
                                              ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                              : "") + "metadata.jcf.lz4";

            return File.Exists(metadataCacheLz4File)
                ? (new FileInfo(metadataCacheLz4File)).Length
                : CalculateFolderSize(metadataCacheFolder);
        }

        public static void ClearCache(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget)
        {
            if (!target.HasValue)
                return;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";
            string metadataCacheLz4File = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                          (subtarget.HasValue
                                              ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                              : "") + "metadata.jcf.lz4";
            string metadataCacheTimestampsFile = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                                 (subtarget.HasValue
                                                     ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value]
                                                     : "") + "timestamps";

            if (Directory.Exists(metadataCacheFolder))
                Shared.DeleteDirectory(metadataCacheFolder);
            if (File.Exists(metadataCacheLz4File))
                File.Delete(metadataCacheLz4File);
            if (File.Exists(metadataCacheTimestampsFile))
                File.Delete(metadataCacheTimestampsFile);
        }

        public static void ClearAllCache()
        {
            Directory.CreateDirectory(Preferences.CachePath);

            // Get current target
            Shared.CacheTarget? currentCacheTarget =
                Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            Shared.CacheSubtarget? currentCacheSubtarget = null;

            if (currentCacheTarget == Shared.CacheTarget.Android)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            else if (currentCacheTarget == Shared.CacheTarget.BlackBerry)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif

            string currentTargetMetadataDirectoryPath = Preferences.CachePath +
                                                        Shared.CacheTargetPrefixes[(int) currentCacheTarget] +
                                                        (currentCacheSubtarget.HasValue
                                                            ? Shared.CacheSubtargetPrefixes[(int) currentCacheSubtarget]
                                                            : "") + "metadata";

            foreach (string directory in Directory.GetDirectories(Preferences.CachePath))
            {
                if (Preferences.EnableHardLinks &&
                    directory.Replace("\\", "/").Equals(currentTargetMetadataDirectoryPath))
                    continue;

                Shared.DeleteDirectory(directory);
            }

            foreach (string file in Directory.GetFiles(Preferences.CachePath))
            {
                File.Delete(file);
            }
        }

        public static void CompressCache(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget,
            bool silent = false)
        {
            if (!target.HasValue)
                return;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";

            if (Directory.Exists(metadataCacheFolder))
                CompressionManager.PerformCompression(metadataCacheFolder, silent);
        }

        public static void DecompressCache(Shared.CacheTarget? target, Shared.CacheSubtarget? subtarget,
            bool silent = false)
        {
            if (!target.HasValue)
                return;

            string metadataCacheFolder = Preferences.CachePath + Shared.CacheTargetPrefixes[(int) target.Value] +
                                         (subtarget.HasValue ? Shared.CacheSubtargetPrefixes[(int) subtarget.Value] : "") +
                                         "metadata";

            CompressionManager.PerformDecompression(metadataCacheFolder, silent);
        }

        public static void CompressAllCache(bool background)
        {
            // Get current target
            Shared.CacheTarget? currentCacheTarget =
                Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            Shared.CacheSubtarget? currentCacheSubtarget = null;

            if (currentCacheTarget == Shared.CacheTarget.Android)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            else if (currentCacheTarget == Shared.CacheTarget.BlackBerry)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif

            if (background)
            {
                Directory.CreateDirectory(Preferences.CachePath);

                string backgroundLockFilePath = Preferences.CachePath + "Background.txt";
                FileStream stream = File.Create(backgroundLockFilePath);
                stream.Dispose();

                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        CompressAllCacheOperation(true, currentCacheTarget, currentCacheSubtarget);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);

                        // Cleanup
                        foreach (string file in Directory.GetFiles(Preferences.CachePath))
                        {
                            if (file.EndsWith(".jcf"))
                                File.Delete(file);
                        }
                    }
                    finally
                    {
                        File.Delete(backgroundLockFilePath);
                        //ShouldRefreshUI = true;
                    }
                });
            }
            else
            {
                CompressAllCacheOperation(false, currentCacheTarget, currentCacheSubtarget);
            }
        }

        private static void CompressAllCacheOperation(bool background, Shared.CacheTarget? currentCacheTarget,
            Shared.CacheSubtarget? currentCacheSubtarget)
        {
            // Process all targets & subtargets
            for (int i = 0; i < (int) Shared.CacheTarget.Count; i++)
            {
                var target = (Shared.CacheTarget) i;

                if (target == Shared.CacheTarget.Android)
                {
                    for (int j = (int) Shared.CacheSubtarget.Android_First + 1;
                        j < (int) Shared.CacheSubtarget.Android_Last;
                        j++)
                    {
                        var subtarget = (Shared.CacheSubtarget) j;
                        if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget) &&
                            !(Preferences.EnableHardLinks && target == currentCacheTarget &&
                              subtarget == currentCacheSubtarget))
                            CompressCache(target, subtarget, background);
                    }
                }
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                else if (target == Shared.CacheTarget.BlackBerry)
                {
                    for (int j = (int) Shared.CacheSubtarget.BlackBerry_First + 1;
                        j < (int) Shared.CacheSubtarget.BlackBerry_Last;
                        j++)
                    {
                        var subtarget = (Shared.CacheSubtarget) j;
                        if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget) &&
                            !(Preferences.EnableHardLinks && target == currentCacheTarget &&
                              subtarget == currentCacheSubtarget))
                            CompressCache(target, subtarget, background);
                    }
                }
#endif
                else
                {
                    if (GetCacheStatus(target, null) && !GetCacheCompression(target, null) &&
                        !(Preferences.EnableHardLinks && target == currentCacheTarget && null == currentCacheSubtarget))
                        CompressCache(target, null, background);
                }
            }
        }

        public static void DecompressAllCache()
        {
            // Process all targets & subtargets
            for (int i = 0; i < (int) Shared.CacheTarget.Count; i++)
            {
                var target = (Shared.CacheTarget) i;

                if (target == Shared.CacheTarget.Android)
                {
                    for (int j = (int) Shared.CacheSubtarget.Android_First + 1;
                        j < (int) Shared.CacheSubtarget.Android_Last;
                        j++)
                    {
                        var subtarget = (Shared.CacheSubtarget) j;
                        if (GetCacheStatus(target, subtarget) && GetCacheCompression(target, subtarget))
                            DecompressCache(target, subtarget);
                    }
                }
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                else if (target == Shared.CacheTarget.BlackBerry)
                {
                    for (int j = (int) Shared.CacheSubtarget.BlackBerry_First + 1;
                        j < (int) Shared.CacheSubtarget.BlackBerry_Last;
                        j++)
                    {
                        var subtarget = (Shared.CacheSubtarget) j;
                        if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget))
                            CompressCache(target, subtarget);
                    }
                }
#endif
                else
                {
                    if (GetCacheStatus(target, null) && GetCacheCompression(target, null))
                        DecompressCache(target, null);
                }
            }
        }

        public static void FixIssues()
        {
            // Disable hard links
            Preferences.EnableHardLinks = false;
            CheckHardLinkStatus();

            // Delete local cache folder
            if (Directory.Exists(Preferences.CachePath))
                Shared.DeleteDirectory(Preferences.CachePath);

            // Delete metadata folder
            string assetsPath = Application.dataPath;
            string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
            if (Directory.Exists(libraryPath + "/metadata"))
                Shared.DeleteDirectory(libraryPath + "/metadata");

            // Reimport all
            EditorApplication.ExecuteMenuItem("Assets/Reimport All");
        }

        private static long CalculateFolderSize(string folder)
        {
            long folderSize = 0;
            try
            {
                if (!Directory.Exists(folder))
                    return -1;
                try
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (File.Exists(file))
                        {
                            var finfo = new FileInfo(file);
                            folderSize += finfo.Length;
                        }
                    }

                    foreach (string dir in Directory.GetDirectories(folder))
                        folderSize += CalculateFolderSize(dir);
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Unable to calculate folder size: {0}", e.Message));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Unable to calculate folder size: {0}", e.Message));
            }

            return folderSize;
        }

        private static void DirectoryMerge(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(sourceDirName))
                return;

            EditorUtility.DisplayProgressBar(_progressTitle, _progressDescription, _progress);

            // Check for dest directory
            bool destDirectoryExists = Directory.Exists(destDirName);
            if (destDirectoryExists)
            {
                // First delete files from dest not in source
                string[] destFiles = Directory.GetFiles(destDirName, "*.*", SearchOption.AllDirectories);
                foreach (string file in destFiles)
                {
                    if (!File.Exists(sourceDirName + file.Remove(0, destDirName.Length)))
                        File.Delete(file);

                    _progress += 1.0f/(destFiles.Length*3.0f);
                    EditorUtility.DisplayProgressBar(_progressTitle, _progressDescription, _progress);
                }
            }
            else
            {
                Directory.CreateDirectory(destDirName);
            }

            // Then copy changed and new files from source to dest
            string[] sourceFiles = Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories);
            foreach (string file in sourceFiles)
            {
                string destFile = destDirName + file.Remove(0, sourceDirName.Length);

                // Check if exists, same write time, same size
                if (File.Exists(destFile) && File.GetLastWriteTimeUtc(file) == File.GetLastWriteTimeUtc(destFile) &&
                    new FileInfo(file).Length == new FileInfo(destFile).Length)
                    continue;

                // Create subdirs if needed
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                // Copy file
                //File.Delete(destFile);
                File.Copy(file, destFile, true);
                File.SetCreationTime(destFile, File.GetCreationTime(file));
                File.SetLastAccessTime(destFile, File.GetLastAccessTime(file));
                File.SetLastWriteTime(destFile, File.GetLastWriteTime(file));

                _progress += 1.0f/(sourceFiles.Length*(destDirectoryExists ? 3.0f : 1.0f));
                EditorUtility.DisplayProgressBar(_progressTitle, _progressDescription, _progress);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void SerializeTimestamps(Dictionary<string, ulong> dictionary, Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
            writer.Flush();
        }

        private static Dictionary<string, ulong> DeserializeTimestamps(Stream stream)
        {
            var reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            var dictionary = new Dictionary<string, ulong>(count);
            for (int n = 0; n < count; n++)
            {
                string key = reader.ReadString();
                ulong value = reader.ReadUInt64();
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        public static void CheckHardLinkStatus()
        {
            bool metadataIsHardLink = false;
            bool hardLinkIsValid = false;
            string hardLinkDirectory = null;

            string assetsPath = Application.dataPath;

            string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
            Directory.CreateDirectory(Preferences.CachePath);

            string metadataPath = libraryPath + "/metadata";


            // Get current target
            Shared.CacheTarget? currentCacheTarget =
                Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            Shared.CacheSubtarget? currentCacheSubtarget = null;

            if (currentCacheTarget == Shared.CacheTarget.Android)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            else if (currentCacheTarget == Shared.CacheTarget.BlackBerry)
                currentCacheSubtarget =
                    Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif

            string currentTargetMetadataDirectoryPath = Preferences.CachePath +
                                                        Shared.CacheTargetPrefixes[(int) currentCacheTarget] +
                                                        (currentCacheSubtarget.HasValue
                                                            ? Shared.CacheSubtargetPrefixes[(int) currentCacheSubtarget]
                                                            : "") + "metadata";


            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // Get inum value
                string inum;

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "ls",
                        Arguments = "-id \"" + metadataPath + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    inum = result.Split(' ')[0];
                }

                process.Dispose();

                // Find by inum in cache
                process = new Process
                {
                    StartInfo =
                    {
                        FileName = "find",
						Arguments = "\"" + Preferences.CachePath.TrimEnd(new [] { '/' }) + "\" -inum " + inum,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(result))
                    {
                        metadataIsHardLink = true;
                        hardLinkDirectory = result;
                        hardLinkIsValid = result.Equals(currentTargetMetadataDirectoryPath);
                    }
                }

                process.Dispose();
            }
            else
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                        Arguments = "/accepteula \"" + metadataPath + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.GetEncoding("ISO-8859-1")
                    }
                };
                process.Start();
                process.WaitForExit();

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    int indexOfSubstituteName = result.IndexOf("Substitute Name: ", StringComparison.Ordinal);
                    if (indexOfSubstituteName != -1)
                    {
                        metadataIsHardLink = true;
                        hardLinkDirectory =
                            result.Substring(indexOfSubstituteName + 17).Split('\n')[0].Trim().Replace("\\", "/");
                        hardLinkIsValid = hardLinkDirectory.Equals(currentTargetMetadataDirectoryPath);
                    }
                }

                process.Dispose();
            }

            if (Preferences.EnableHardLinks)
            {
                if (metadataIsHardLink == false)
                {
                    EditorUtility.DisplayProgressBar("Hold on", "Enabling hard links...", 0.5f);

                    // Cleanup
                    if (File.Exists(currentTargetMetadataDirectoryPath + ".jcf"))
                        File.Delete(currentTargetMetadataDirectoryPath + ".jcf");

                    if (File.Exists(currentTargetMetadataDirectoryPath + ".jcf.lz4"))
                        File.Delete(currentTargetMetadataDirectoryPath + ".jcf.lz4");

                    if (Directory.Exists(currentTargetMetadataDirectoryPath))
                        Shared.DeleteDirectory(currentTargetMetadataDirectoryPath);

                    FixPermissions();

                    Shared.DirectoryCopy(metadataPath, currentTargetMetadataDirectoryPath, true);
                    Shared.DeleteDirectory(metadataPath);

                    var process = new Process();
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        // Make hardlink process executable
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = "chmod",
                                Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths + "hardlink\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" + metadataPath +
                                            "\""
                            }
                        };
                    }
                    else
                    {
                        process.StartInfo.FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe";
                        process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\" \"" +
                                                      currentTargetMetadataDirectoryPath + "\"";
                    }

                    // Wait for process to end
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    process.Dispose();

                    EditorUtility.ClearProgressBar();
                }
                else if (hardLinkIsValid == false)
                {
                    // Cleanup current target directories & files
                    if (Directory.Exists(currentTargetMetadataDirectoryPath))
                        Shared.DeleteDirectory(currentTargetMetadataDirectoryPath);
                    if (File.Exists(currentTargetMetadataDirectoryPath + ".jcf"))
                        File.Delete(currentTargetMetadataDirectoryPath + ".jcf");
                    if (File.Exists(currentTargetMetadataDirectoryPath + ".jcf.lz4"))
                        File.Delete(currentTargetMetadataDirectoryPath + ".jcf.lz4");
                    if (
                        File.Exists(
                            currentTargetMetadataDirectoryPath.Substring(0,
                                currentTargetMetadataDirectoryPath.Length - 9) + "_timestamps"))
                        File.Delete(
                            currentTargetMetadataDirectoryPath.Substring(0,
                                currentTargetMetadataDirectoryPath.Length - 9) + "_timestamps");

                    // Copy current data to new metadata path
                    Shared.DirectoryCopy(hardLinkDirectory, currentTargetMetadataDirectoryPath, true);

                    Process process;
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        // Make hardlink process executable
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = "chmod",
                                Arguments = "+x \"" + Shared.ProjectPath + Shared.UtilsPaths + "hardlink\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Delete current hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                Arguments = "-u \"" + metadataPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Make new hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink",
                                Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" + metadataPath +
                                            "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();
                    }
                    else
                    {
                        // Delete current hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                Arguments = "/accepteula -d \"" + metadataPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();

                        // Make new hard link
                        process = new Process
                        {
                            StartInfo =
                            {
                                FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe",
                                Arguments = "/accepteula \"" + metadataPath + "\" \"" +
                                            currentTargetMetadataDirectoryPath + "\"",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit();
                        process.Dispose();
                    }

                    // Cleanup previous target directories & files
                    if (Directory.Exists(hardLinkDirectory))
                        Shared.DeleteDirectory(hardLinkDirectory);
                    if (File.Exists(hardLinkDirectory + ".jcf"))
                        File.Delete(hardLinkDirectory + ".jcf");
                    if (File.Exists(hardLinkDirectory + ".jcf.lz4"))
                        File.Delete(hardLinkDirectory + ".jcf.lz4");
                    if (File.Exists(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps"))
                        File.Delete(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps");
                }
            }
            else
            {
                if (metadataIsHardLink)
                {
                    EditorUtility.DisplayProgressBar("Hold on", "Disabling hard links...", 0.5f);

                    var process = new Process();
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        process.StartInfo.FileName = Shared.ProjectPath + Shared.UtilsPaths + "hardlink";
                        process.StartInfo.Arguments = "-u \"" + metadataPath + "\"";
                    }
                    else
                    {
                        process.StartInfo.FileName = Shared.ProjectPath + Shared.UtilsPaths + "junction.exe";
                        process.StartInfo.Arguments = "/accepteula -d \"" + metadataPath + "\"";
                    }

                    // Wait for process to end
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    process.Dispose();

                    FixPermissions();

                    if (Directory.Exists(metadataPath))
                        Shared.DeleteDirectory(metadataPath);

                    Shared.DirectoryCopy(hardLinkDirectory, metadataPath, true);

                    // Cleanup previous target directories & files if hard link was invalid
                    if (hardLinkIsValid == false)
                    {
                        if (Directory.Exists(hardLinkDirectory))
                            Shared.DeleteDirectory(hardLinkDirectory);
                        if (File.Exists(hardLinkDirectory + ".jcf"))
                            File.Delete(hardLinkDirectory + ".jcf");
                        if (File.Exists(hardLinkDirectory + ".jcf.lz4"))
                            File.Delete(hardLinkDirectory + ".jcf.lz4");
                        if (File.Exists(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps"))
                            File.Delete(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps");
                    }

                    EditorUtility.ClearProgressBar();
                }
            }
        }

        private static void FixPermissions()
        {
            string assetsPath = Application.dataPath;
            string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
            string metadataPath = libraryPath + "/metadata";
            Directory.CreateDirectory(Preferences.CachePath);

            if (Application.platform != RuntimePlatform.OSXEditor) return;

            // Chmod/Chown metadata
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "whoami",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();

            string whoami = process.StandardOutput.ReadLine();

            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "find",
                    Arguments = "\"" + metadataPath + "\" -type d -exec chmod 755 {} +",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "find",
                    Arguments = "\"" + metadataPath + "\" -type f -exec chmod 644 {} +",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "chown",
                    Arguments = "-RH \"" + whoami + ":staff\" \"" + metadataPath + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "find",
					Arguments = "\"" + Preferences.CachePath.TrimEnd(new [] { '/' }) + "\" -type d -exec chmod 755 {} +",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "find",
					Arguments = "\"" + Preferences.CachePath.TrimEnd(new [] { '/' }) + "\" -type f -exec chmod 644 {} +",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();

            process = new Process
            {
                StartInfo =
                {
                    FileName = "chown",
					Arguments = "-RH \"" + whoami + ":staff\" \"" + Preferences.CachePath.TrimEnd(new [] { '/' }) + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        public static void Cleanup()
        {
            Directory.CreateDirectory(Preferences.CachePath);

            string[] cleanupFiles = Directory.GetFiles(Preferences.CachePath, "*.cleanup", SearchOption.TopDirectoryOnly);

            foreach (string file in cleanupFiles)
            {
                string directory = file.Remove(file.Length - 8, 8);
                if (Directory.Exists(directory))
                    Shared.DeleteDirectory(directory);
                if (File.Exists(directory + ".jcf"))
                    File.Delete(directory + ".jcf");
                if (File.Exists(directory + ".jcf.lz4"))
                    File.Delete(directory + ".jcf.lz4");
                if (File.Exists(directory.Substring(0, directory.Length - 9) + "_timestamps"))
                    File.Delete(directory.Substring(0, directory.Length - 9) + "_timestamps");

                File.Delete(file);
            }
        }
    }
}