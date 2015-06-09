//
//  Shared.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jemast.LocalCache
{
    public static class Shared
    {
        public static readonly string ProjectPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);

        public static readonly string SettingsFilePath = ProjectPath +
                                                         "JLocalCacheProjectSettings.xml";

        private static string _utilsPaths;

        public static string UtilsPaths
        {
            get
            {
                if (_utilsPaths != null)
                    return _utilsPaths;

                // Recursively parse assets to look for JLocalCacheWindow.cs
                FileInfo[] fileList = new DirectoryInfo(ProjectPath + "Assets").GetFiles("lz4.exe",
                    SearchOption.AllDirectories);
                if (fileList.Length == 1)
                    _utilsPaths =
                        fileList[0].DirectoryName.Substring(ProjectPath.Length,
                            fileList[0].DirectoryName.Length - ProjectPath.Length).Replace('\\', '/') + '/';
                else
                    _utilsPaths = "Assets/Jemast/Shared/Editor/Utils/";

                return _utilsPaths;
            }
        }

        private static string _editorAssetsPath;

        public static string EditorAssetsPath
        {
            get
            {
                if (_editorAssetsPath != null)
                    return _editorAssetsPath;

                // Recursively parse assets to look for JLocalCacheWindow.cs
                FileInfo[] fileList = new DirectoryInfo(ProjectPath + "Assets").GetFiles("JLocalCacheWindow.cs",
                    SearchOption.AllDirectories);
                if (fileList.Length == 1)
                    _editorAssetsPath =
                        fileList[0].DirectoryName.Substring(ProjectPath.Length,
                            fileList[0].DirectoryName.Length - ProjectPath.Length).Replace('\\', '/') + '/';
                else
                    _editorAssetsPath = "Assets/Jemast/LocalCache/Editor/";

                return _editorAssetsPath;
            }
        }

        private static string _sharedEditorAssetsPath;

        public static string SharedEditorAssetsPath
        {
            get
            {
                if (_sharedEditorAssetsPath != null)
                    return _sharedEditorAssetsPath;

                // Recursively parse assets to look for JCF.cs
                FileInfo[] fileList = new DirectoryInfo(ProjectPath + "Assets").GetFiles("JCF.cs",
                    SearchOption.AllDirectories);
                if (fileList.Length == 1)
                    _sharedEditorAssetsPath =
                        fileList[0].DirectoryName.Substring(ProjectPath.Length,
                            fileList[0].DirectoryName.Length - ProjectPath.Length).Replace('\\', '/') + '/';
                else
                    _sharedEditorAssetsPath = "Assets/Jemast/Shared/Editor/";

                return _sharedEditorAssetsPath;
            }
        }

        public enum CacheTarget
        {
            WebPlayer,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
            WebGL,
#endif
            Standalone,
            iOS,
            Android,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            BlackBerry,
            Metro,
            WP8,
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			NaCl,
	#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			Flash,
	#endif
            PS3,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            PS4,
            VITA,
            PSM,
#endif
            X360,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            XBONE,
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			Wii,
#endif
            Count
        };

        public enum CacheSubtarget
        {
            Android_First,
            Android_GENERIC,
            Android_DXT,
            Android_PVRTC,
            Android_ATC,
            Android_ETC,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            Android_ETC2,
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            Android_ASTC,
#endif
            Android_Last,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            BlackBerry_First,
            BlackBerry_GENERIC,
            BlackBerry_PVRTC,
            BlackBerry_ATC,
            BlackBerry_ETC,
            BlackBerry_Last,
#endif
            Count
        }

        public static readonly string[] CacheTargetPrefixes =
        {
            "WEBPLAYER_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
            "WEBGL_",
#endif
            "STANDALONE_",
            "IOS_",
            "ANDROID_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            "BB10_",
            "METRO_",
            "WP8_",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			"NACL_",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			"FLASH_",
#endif
            "PS3_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            "PS4_",
            "VITA_",
            "PSM_",
#endif
            "X360_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            "XBONE_"
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			"WII_"
#endif
        };

        public static readonly string[] CacheSubtargetPrefixes =
        {
            "Android_First_",
            "GENERIC_",
            "DXT_",
            "PVRTC_",
            "ATC_",
            "ETC1_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            "ETC2_",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            "ASTC_",
#endif
            "Android_Last_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            "BlackBerry_First_",
            "GENERIC_",
            "PVRTC_",
            "ATC_",
            "ETC1_",
            "BlackBerry_Last_"
#endif
        };

        public static CacheTarget? CacheTargetForBuildTarget(BuildTarget? target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return CacheTarget.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3)
			case BuildTarget.BB10:
				return LocalCache.Shared.CacheTarget.BlackBerry;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case BuildTarget.BlackBerry:
                    return CacheTarget.BlackBerry;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			case BuildTarget.FlashPlayer:
				return LocalCache.Shared.CacheTarget.Flash;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 // UNITY 4.6-
                case BuildTarget.iPhone:
                    return CacheTarget.iOS;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
                case BuildTarget.MetroPlayer:
                    return CacheTarget.Metro;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			case BuildTarget.NaCl:
				return LocalCache.Shared.CacheTarget.NaCl;
#endif
                case BuildTarget.PS3:
                    return CacheTarget.PS3;
#if !UNITY_3_4 && !UNITY_3_5
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                case BuildTarget.StandaloneOSXIntel:
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
#endif
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return CacheTarget.Standalone;
                case BuildTarget.WebPlayer:
                case BuildTarget.WebPlayerStreamed:
                    return CacheTarget.WebPlayer;
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			case BuildTarget.Wii:
				return LocalCache.Shared.CacheTarget.Wii;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case BuildTarget.WP8Player:
                    return CacheTarget.WP8;
#endif
                case BuildTarget.XBOX360:
                    return CacheTarget.X360;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case BuildTarget.PS4:
                    return CacheTarget.PS4;
                case BuildTarget.PSP2:
                    return CacheTarget.VITA;
                case BuildTarget.PSM:
                    return CacheTarget.PSM;
                case BuildTarget.XboxOne:
                    return CacheTarget.XBONE;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
                case BuildTarget.iOS:
                    return CacheTarget.iOS;
                case BuildTarget.WSAPlayer:
                    return CacheTarget.Metro;
                case BuildTarget.WebGL:
                    return CacheTarget.WebGL;
#endif
                default:
                    return null;
            }
        }

        public static BuildTarget? BuildTargetForCacheTarget(CacheTarget? option)
        {
            switch (option)
            {
                case CacheTarget.Android:
                    return BuildTarget.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3)
			case LocalCache.Shared.CacheTarget.BlackBerry:
				return BuildTarget.BB10;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case CacheTarget.BlackBerry:
                    return BuildTarget.BlackBerry;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			case LocalCache.Shared.CacheTarget.Flash:
				return BuildTarget.FlashPlayer;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 // UNITY 4.6-
                case CacheTarget.iOS:
                    return BuildTarget.iPhone;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
                case CacheTarget.Metro:
                    return BuildTarget.MetroPlayer;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			case LocalCache.Shared.CacheTarget.NaCl:
				return BuildTarget.NaCl;
#endif
                case CacheTarget.PS3:
                    return BuildTarget.PS3;
                case CacheTarget.Standalone:
                    switch (Preferences.DefaultStandaloneBuildTargetOption)
                    {
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                        case 0:
                            return BuildTarget.StandaloneWindows;
                        case 1:
                            return BuildTarget.StandaloneWindows64;
                        case 2:
                            return BuildTarget.StandaloneOSXIntel;
                        case 3:
                            return BuildTarget.StandaloneOSXIntel64;
                        case 4:
                            return BuildTarget.StandaloneOSXUniversal;
                        case 5:
                            return BuildTarget.StandaloneLinux;
                        case 6:
                            return BuildTarget.StandaloneLinux64;
                        case 7:
                            return BuildTarget.StandaloneLinuxUniversal;
#elif !UNITY_3_4 && !UNITY_3_5
                        case 0:
                            return BuildTarget.StandaloneWindows;
                        case 1:
                            return BuildTarget.StandaloneWindows64;
                        case 2:
                            return BuildTarget.StandaloneOSXIntel;
                        case 3:
                            return BuildTarget.StandaloneLinux;
                        case 4:
                            return BuildTarget.StandaloneLinux64;
                        case 5:
                            return BuildTarget.StandaloneLinuxUniversal;
#else
                        case 0:
                            return BuildTarget.StandaloneWindows;
                        case 1:
                            return BuildTarget.StandaloneWindows64;
                        case 2:
                            return BuildTarget.StandaloneOSXIntel;
#endif
                        default:
                            return BuildTarget.StandaloneWindows;
                    }
                case CacheTarget.WebPlayer:
                    switch (Preferences.DefaultWebPlayerBuildTargetOption)
                    {
                        case 0:
                            return BuildTarget.WebPlayer;
                        case 1:
                            return BuildTarget.WebPlayerStreamed;
                        default:
                            return BuildTarget.WebPlayer;
                    }
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			case LocalCache.Shared.CacheTarget.Wii:
				return BuildTarget.Wii;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case CacheTarget.WP8:
                    return BuildTarget.WP8Player;
#endif
                case CacheTarget.X360:
                    return BuildTarget.XBOX360;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case CacheTarget.PS4:
                    return BuildTarget.PS4;
                case CacheTarget.VITA:
                    return BuildTarget.PSP2;
                case CacheTarget.PSM:
                    return BuildTarget.PSM;
                case CacheTarget.XBONE:
                    return BuildTarget.XboxOne;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
                case CacheTarget.iOS:
                    return BuildTarget.iOS;
                case CacheTarget.Metro:
                    return BuildTarget.WSAPlayer;
                case CacheTarget.WebGL:
                    return BuildTarget.WebGL;
#endif
                default:
                    return null;
            }
        }

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
        public static CacheSubtarget? CacheSubtargetForAndroidBuildSubtarget(MobileTextureSubtarget? target)
        {
            switch (target)
            {
                case MobileTextureSubtarget.ATC:
                    return CacheSubtarget.Android_ATC;
                case MobileTextureSubtarget.DXT:
                    return CacheSubtarget.Android_DXT;
                case MobileTextureSubtarget.Generic:
                    return CacheSubtarget.Android_GENERIC;
                case MobileTextureSubtarget.ETC:
                    return CacheSubtarget.Android_ETC;
                case MobileTextureSubtarget.ETC2:
                    return CacheSubtarget.Android_ETC2;
                case MobileTextureSubtarget.PVRTC:
                    return CacheSubtarget.Android_PVRTC;
                case MobileTextureSubtarget.ASTC:
                    return CacheSubtarget.Android_ASTC;
                default:
                    return null;
            }
        }

        public static MobileTextureSubtarget? AndroidBuildSubtargetForCacheSubtarget(CacheSubtarget? target)
        {
            switch (target)
            {
                case CacheSubtarget.Android_GENERIC:
                    return MobileTextureSubtarget.Generic;
                case CacheSubtarget.Android_ATC:
                    return MobileTextureSubtarget.ATC;
                case CacheSubtarget.Android_DXT:
                    return MobileTextureSubtarget.DXT;
                case CacheSubtarget.Android_ETC:
                    return MobileTextureSubtarget.ETC;
                case CacheSubtarget.Android_ETC2:
                    return MobileTextureSubtarget.ETC2;
                case CacheSubtarget.Android_PVRTC:
                    return MobileTextureSubtarget.PVRTC;
                case CacheSubtarget.Android_ASTC:
                    return MobileTextureSubtarget.ASTC;
                default:
                    return null;
            }
        }
#else
        public static CacheSubtarget? CacheSubtargetForAndroidBuildSubtarget(AndroidBuildSubtarget? target)
        {
            switch (target)
            {
                case AndroidBuildSubtarget.ATC:
                    return CacheSubtarget.Android_ATC;
                case AndroidBuildSubtarget.DXT:
                    return CacheSubtarget.Android_DXT;
                case AndroidBuildSubtarget.Generic:
                    return CacheSubtarget.Android_GENERIC;
                case AndroidBuildSubtarget.ETC:
                    return CacheSubtarget.Android_ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case AndroidBuildSubtarget.ETC2:
                    return CacheSubtarget.Android_ETC2;
#endif
                case AndroidBuildSubtarget.PVRTC:
                    return CacheSubtarget.Android_PVRTC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
                case AndroidBuildSubtarget.ASTC:
                    return CacheSubtarget.Android_ASTC;
#endif
                default:
                    return null;
            }
        }

        public static AndroidBuildSubtarget? AndroidBuildSubtargetForCacheSubtarget(CacheSubtarget? target)
        {
            switch (target)
            {
                case CacheSubtarget.Android_GENERIC:
                    return AndroidBuildSubtarget.Generic;
                case CacheSubtarget.Android_ATC:
                    return AndroidBuildSubtarget.ATC;
                case CacheSubtarget.Android_DXT:
                    return AndroidBuildSubtarget.DXT;
                case CacheSubtarget.Android_ETC:
                    return AndroidBuildSubtarget.ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case CacheSubtarget.Android_ETC2:
                    return AndroidBuildSubtarget.ETC2;
#endif
                case CacheSubtarget.Android_PVRTC:
                    return AndroidBuildSubtarget.PVRTC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
                case CacheSubtarget.Android_ASTC:
                    return AndroidBuildSubtarget.ASTC;
#endif
                default:
                    return null;
            }
        }
#endif


#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
        public static CacheSubtarget? CacheSubtargetForBlackBerryBuildSubtarget(MobileTextureSubtarget? target)
        {
            switch (target)
            {
                case MobileTextureSubtarget.Generic:
                    return CacheSubtarget.BlackBerry_GENERIC;
                case MobileTextureSubtarget.PVRTC:
                    return CacheSubtarget.BlackBerry_PVRTC;
                case MobileTextureSubtarget.ATC:
                    return CacheSubtarget.BlackBerry_ATC;
                case MobileTextureSubtarget.ETC:
                    return CacheSubtarget.BlackBerry_ETC;
                default:
                    return null;
            }
        }

        public static MobileTextureSubtarget? BlackBerryBuildSubtargetForCacheSubtarget(CacheSubtarget? target)
        {
            switch (target)
            {
                case CacheSubtarget.BlackBerry_GENERIC:
                    return MobileTextureSubtarget.Generic;
                case CacheSubtarget.BlackBerry_PVRTC:
                    return MobileTextureSubtarget.PVRTC;
                case CacheSubtarget.BlackBerry_ATC:
                    return MobileTextureSubtarget.ATC;
                case CacheSubtarget.BlackBerry_ETC:
                    return MobileTextureSubtarget.ETC;
                default:
                    return null;
            }
        }
#else
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        public static CacheSubtarget? CacheSubtargetForBlackBerryBuildSubtarget(BlackBerryBuildSubtarget? target)
        {
            switch (target)
            {
                case BlackBerryBuildSubtarget.Generic:
                    return CacheSubtarget.BlackBerry_GENERIC;
                case BlackBerryBuildSubtarget.PVRTC:
                    return CacheSubtarget.BlackBerry_PVRTC;
                case BlackBerryBuildSubtarget.ATC:
                    return CacheSubtarget.BlackBerry_ATC;
                case BlackBerryBuildSubtarget.ETC:
                    return CacheSubtarget.BlackBerry_ETC;
                default:
                    return null;
            }
        }
#endif

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        public static BlackBerryBuildSubtarget? BlackBerryBuildSubtargetForCacheSubtarget(CacheSubtarget? target)
        {
            switch (target)
            {
                case CacheSubtarget.BlackBerry_GENERIC:
                    return BlackBerryBuildSubtarget.Generic;
                case CacheSubtarget.BlackBerry_PVRTC:
                    return BlackBerryBuildSubtarget.PVRTC;
                case CacheSubtarget.BlackBerry_ATC:
                    return BlackBerryBuildSubtarget.ATC;
                case CacheSubtarget.BlackBerry_ETC:
                    return BlackBerryBuildSubtarget.ETC;
                default:
                    return null;
            }
        }
#endif
#endif

        public static BuildTargetGroup? BuildTargetGroupForCacheTarget(CacheTarget? option)
        {
            switch (option)
            {
                case CacheTarget.Android:
                    return BuildTargetGroup.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3)
			case LocalCache.Shared.CacheTarget.BlackBerry:
				return BuildTargetGroup.BB10;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case CacheTarget.BlackBerry:
                    return BuildTargetGroup.BlackBerry;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			case LocalCache.Shared.CacheTarget.Flash:
				return BuildTargetGroup.FlashPlayer;
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 // UNITY 4.6-
                case CacheTarget.iOS:
                    return BuildTargetGroup.iPhone;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && (UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
                case CacheTarget.Metro:
                    return BuildTargetGroup.Metro;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			case LocalCache.Shared.CacheTarget.NaCl:
				return BuildTargetGroup.NaCl;
#endif
                case CacheTarget.PS3:
                    return BuildTargetGroup.PS3;
                case CacheTarget.Standalone:
                    return BuildTargetGroup.Standalone;
                case CacheTarget.WebPlayer:
                    return BuildTargetGroup.WebPlayer;
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			case LocalCache.Shared.CacheTarget.Wii:
				return BuildTargetGroup.Wii;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                case CacheTarget.WP8:
                    return BuildTargetGroup.WP8;
#endif
                case CacheTarget.X360:
                    return BuildTargetGroup.XBOX360;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
                case CacheTarget.PS4:
                    return BuildTargetGroup.PS4;
                case CacheTarget.VITA:
                    return BuildTargetGroup.PSP2;
                case CacheTarget.PSM:
                    return BuildTargetGroup.PSM;
                case CacheTarget.XBONE:
                    return BuildTargetGroup.XboxOne;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
                case CacheTarget.iOS:
                    return BuildTargetGroup.iOS;
                case CacheTarget.Metro:
                    return BuildTargetGroup.WSA;
                case CacheTarget.WebGL:
                    return BuildTargetGroup.WebGL;
#endif
                default:
                    return null;
            }
        }

        public static readonly string[] CompressionAlgorithmOptions =
        {
            "LZ4"
        };

        public static readonly string[] CompressionQualityLz4Options =
        {
            "Fast Compression",
            "High Compression (Slow)"
        };

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
        }

        public static void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public static bool DirectoryIsEmpty(string targetDir)
        {
            return (Directory.GetFiles(targetDir).Length + Directory.GetDirectories(targetDir).Length) == 0;
        }
    }
}