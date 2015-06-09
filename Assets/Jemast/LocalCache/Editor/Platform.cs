using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Jemast.LocalCache
{
    [InitializeOnLoad]
    public static class Platform
    {
        public delegate void CallbackFunction();

        public static CallbackFunction PlatformChange;

        private static bool? _wantsSwitchOperation;

		private static bool _hasSkippedFrame;

        static Platform()
        {
            EditorApplication.update += Update;

            // Local Cache Version
            if (Preferences.LocalCacheVersion < 2)
            {
                Preferences.LocalCacheVersion = 2;
                EditorUtility.DisplayDialog("Fast Platform Switch - IMPORTANT",
                    "This update features an updated and improved cache manager. Due to breaking changes, we need you to CLEAR ALL YOUR CACHE and REIMPORT ALL once on all your projects using Fast Platform Switch.\n\nTo do so, go to Fast Platform Switch settings tab and hit the \"Clear all cache and Reimport everything button\" on each of your projects (note: this will disable hard links). We also recommend you backup your project and/or commit/push to version control for better security.\n\nSorry for the inconvience caused and thanks for your understanding!",
                    "OK, I understand");
            }

            // Perform pending cleanup
			if (CacheManager.PlatformRefreshInProgress != true)
				CacheManager.ShouldPerformCleanup = true;
        }

        private static bool WantsSwitchOperation
        {
            get
            {
                if (_wantsSwitchOperation == null)
                {
                    _wantsSwitchOperation = File.Exists(Preferences.CachePath + "WantsSwitchOperation");
                }

                return _wantsSwitchOperation.Value;
            }
            set
            {
                if (value)
                {
                    FileStream s = File.Create(Preferences.CachePath + "WantsSwitchOperation");
                    s.Dispose();
                }
                else
                {
                    File.Delete(Preferences.CachePath + "WantsSwitchOperation");
                }

                _wantsSwitchOperation = value;
            }
        }

        private static Shared.CacheTarget? WantedCacheTarget
        {
            get
            {
                string filePath = Preferences.CachePath + "WantedCacheTarget";
                if (File.Exists(filePath))
                {
                    return (Shared.CacheTarget?) int.Parse(File.ReadAllText(filePath));
                }

                return null;
            }
            set
            {
                string filePath = Preferences.CachePath + "WantedCacheTarget";

                if (value == null)
                {
                    File.Delete(filePath);
                }
                else
                {
                    File.WriteAllText(filePath, ((int) value.Value).ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private static Shared.CacheSubtarget? WantedCacheSubtarget
        {
            get
            {
                string filePath = Preferences.CachePath + "WantedCacheSubtarget";
                if (File.Exists(filePath))
                {
                    return (Shared.CacheSubtarget?) int.Parse(File.ReadAllText(filePath));
                }

                return null;
            }
            set
            {
                string filePath = Preferences.CachePath + "WantedCacheSubtarget";

                if (value == null)
                {
                    File.Delete(filePath);
                }
                else
                {
                    File.WriteAllText(filePath, ((int) value.Value).ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public static bool SwitchPlatform(Shared.CacheTarget target, Shared.CacheSubtarget? subtarget = null)
        {
            if (CacheManager.SwitchOperationInProgress)
            {
                Debug.LogWarning("You can only perform one switch operation at a time.");
                return false;
            }

            // Check we're in editor and not running
            if (Application.isEditor == false || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Platform switch can only happen in editor and not during play mode.");
                return false;
            }

            // Check external version control for Unity < 4.3
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
				Debug.LogWarning ("External version control (.meta files) needs to be enabled in your editor settings.");
				return false;
			}
#endif

            // Check that we're trying to switch to a new target
            if (target == CacheManager.CurrentCacheTarget && subtarget == CacheManager.CurrentCacheSubtarget)
            {
                Debug.LogWarning("Switch operation not triggered because target platform is already active.");
                return false;
            }

            // Refresh Asset Database
            AssetDatabase.Refresh();

            WantsSwitchOperation = true;
            WantedCacheTarget = target;
            WantedCacheSubtarget = subtarget;

            return true;
        }

        private static void Update()
        {
            if (CacheManager.HasCheckedHardLinkStatus == false && !CacheManager.PlatformRefreshInProgress)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling ||
                    EditorApplication.isUpdating) return;

				if (_hasSkippedFrame) {
	                try
	                {
	                    CacheManager.CheckHardLinkStatus();
	                    CacheManager.HasCheckedHardLinkStatus = true;

	                    // Auto compress?
	                    if (Preferences.AutoCompress)
	                        CacheManager.CompressAllCache(true);
	                }
	                catch
	                {
	                }
					_hasSkippedFrame = false;
				} else {
					_hasSkippedFrame = true;
				}
            }
            else
            {
                if (CacheManager.ShouldPerformCleanup)
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling ||
                        EditorApplication.isUpdating) return;

					if (_hasSkippedFrame) {
	                    try
                        {
	                        CacheManager.Cleanup();
	                        CacheManager.ShouldPerformCleanup = false;
	                    }
	                    catch
	                    {
	                    }
						_hasSkippedFrame = false;
					} else {
						_hasSkippedFrame = true;
					}
                }
                else if (WantsSwitchOperation && WantedCacheTarget != null &&
                         CacheManager.SwitchOperationInProgress != true &&
                         !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                         !EditorApplication.isUpdating)
                {
                    CacheManager.SwitchPlatform(WantedCacheTarget, WantedCacheSubtarget, true);
                    WantsSwitchOperation = false;
                    WantedCacheTarget = null;
                    WantedCacheSubtarget = null;
                }
                else if (CacheManager.ShouldSerializeAssets && !EditorApplication.isPlayingOrWillChangePlaymode &&
                         !EditorApplication.isCompiling && !EditorApplication.isUpdating)
                {
					if (_hasSkippedFrame) {
                        CacheManager.SerializeAssetsOperation();
						_hasSkippedFrame = false;
					} else {
						_hasSkippedFrame = true;
					}
                }
                else if (CacheManager.ShouldSwitchPlatform && !EditorApplication.isPlayingOrWillChangePlaymode &&
                         !EditorApplication.isCompiling && !EditorApplication.isUpdating)
                {
                	CacheManager.SwitchPlatformOperation();
                }
                else if (CacheManager.PlatformRefreshInProgress &&
                         !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                         !EditorApplication.isUpdating && !CacheManager.SwitchOperationInProgress)
                {
                	ActiveBuildTargetChanged();
                }
            }
        }

        private static void ActiveBuildTargetChanged()
        {
            LogUtility.LogImmediate("Applying trick to refresh subtargets");

            // Only required with subtarget-only switches
            if (CacheManager.PlatformRefreshShouldBustCache == true)
            {
                EditorUtility.DisplayProgressBar("Hold on", "Forcing synchronization with empty asset bundle build...",
                    0.5f);

                // We'd rather avoid that but since the BuildStreamedSceneAssetBundle method says it requires pro while docs says no...
                if (InternalEditorUtility.HasPro())
                {
                    BuildPipeline.BuildStreamedSceneAssetBundle(
                        new[] {Shared.EditorAssetsPath + "SubtargetCacheBust.unity"},
                        "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget);
                }
                else
                {
                    try
                    {
                        MethodInfo minfo = typeof (BuildPipeline).GetMethod("BuildPlayerInternalNoCheck",
                            BindingFlags.Static | BindingFlags.NonPublic);
                        if (minfo.GetParameters().Length == 5)
                        {
                            minfo.Invoke(null, new object[]
                            {
                                new[] {Shared.EditorAssetsPath + "SubtargetCacheBust.unity"},
                                "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget,
                                BuildOptions.BuildAdditionalStreamedScenes, EditorApplication.isCompiling
                            });
                        }
                        else
                        {
                            minfo.Invoke(null, new object[]
                            {
                                new[] {Shared.EditorAssetsPath + "SubtargetCacheBust.unity"},
                                "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget,
                                BuildOptions.BuildAdditionalStreamedScenes, EditorApplication.isCompiling, null
                            });
                        }
                    }
                    catch
                    {
                    }
                }

                File.Delete(Shared.ProjectPath + "/JLocalCachePluginCacheBuster.unity3d");
                CacheManager.PlatformRefreshShouldBustCache = false;
            }

            LogUtility.LogImmediate("Platform refresh is all done");

            CacheManager.SwitchOperationInProgress = false;
            CacheManager.PlatformRefreshInProgress = false;

            // Cleanup
            CacheManager.ShouldPerformCleanup = true;
            WantsSwitchOperation = false;
            WantedCacheTarget = null;
            WantedCacheSubtarget = null;
            AssetDatabase.Refresh();

            LogUtility.LogImmediate("Reopening previous scene if any");

            if (!string.IsNullOrEmpty(CacheManager.PlatformRefreshCurrentScene))
            {
                EditorUtility.DisplayProgressBar("Hold on", "Reopening previous scene...", 0.5f);
                EditorApplication.OpenScene(CacheManager.PlatformRefreshCurrentScene);
                CacheManager.PlatformRefreshCurrentScene = "";
            }

            EditorUtility.ClearProgressBar();

            if (CacheManager.SwitchOperationIsApi && PlatformChange != null)
            {
                CacheManager.SwitchOperationIsApi = false;
                PlatformChange();
            }
            else
            {
                CacheManager.SwitchOperationIsApi = false;
            }
        }
    }
}