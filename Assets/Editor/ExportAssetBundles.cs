using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using JSFTCacheManager = Jemast.LocalCache.CacheManager;
using JSFTShared = Jemast.LocalCache.Shared;
using JSFTPlatform = Jemast.LocalCache.Platform;

[InitializeOnLoad]
public class ExportAssetBundles
{
		// Store current texture format for the TextureProcessor.
		public static TextureImporterFormat textureFormat = TextureImporterFormat.PVRTC_RGBA4;
		private static BuildTarget buildTarget;
		private static Dictionary<int, AndroidBuildSubtarget> androidSubtargetDictionary;
		private static Dictionary<int, TextureImporterFormat> androidTextureImporterFormatDictionary;
		public static Dictionary<AndroidBuildSubtarget, int> bundleVersionList = new Dictionary<AndroidBuildSubtarget, int> ();
	
		static ExportAssetBundles ()
		{
				JSFTPlatform.PlatformChange += ExportSD_Asset;

				bundleVersionList [AndroidBuildSubtarget.ETC2] = 1;
				bundleVersionList [AndroidBuildSubtarget.DXT] = 2;
				bundleVersionList [AndroidBuildSubtarget.PVRTC] = 3;
				bundleVersionList [AndroidBuildSubtarget.ATC] = 4;

				androidSubtargetDictionary = new Dictionary<int, AndroidBuildSubtarget> ();
				androidSubtargetDictionary.Add (bundleVersionList [AndroidBuildSubtarget.ETC2], AndroidBuildSubtarget.ETC2);
				androidSubtargetDictionary.Add (bundleVersionList [AndroidBuildSubtarget.DXT], AndroidBuildSubtarget.DXT);
				androidSubtargetDictionary.Add (bundleVersionList [AndroidBuildSubtarget.PVRTC], AndroidBuildSubtarget.PVRTC);
				androidSubtargetDictionary.Add (bundleVersionList [AndroidBuildSubtarget.ATC], AndroidBuildSubtarget.ATC);

				androidTextureImporterFormatDictionary = new Dictionary<int, TextureImporterFormat> ();
				androidTextureImporterFormatDictionary.Add (bundleVersionList [AndroidBuildSubtarget.ETC2], TextureImporterFormat.ETC2_RGBA8);
				androidTextureImporterFormatDictionary.Add (bundleVersionList [AndroidBuildSubtarget.DXT], TextureImporterFormat.DXT5);
				androidTextureImporterFormatDictionary.Add (bundleVersionList [AndroidBuildSubtarget.PVRTC], TextureImporterFormat.PVRTC_RGBA4);
				androidTextureImporterFormatDictionary.Add (bundleVersionList [AndroidBuildSubtarget.ATC], TextureImporterFormat.ATC_RGBA8);
		}

		static string savepath {
				get {
						return EditorPrefs.HasKey ("savepath") ? EditorPrefs.GetString ("savepath") : "";
				}
				set {
						EditorPrefs.SetString ("savepath", value);
				}
		}
	
		static bool buildAllSubtarget {
				get {
						return EditorPrefs.HasKey ("buildAllSubtarget") ? EditorPrefs.GetBool ("buildAllSubtarget") : false;
				}
				set {
						EditorPrefs.SetBool ("buildAllSubtarget", value);
				}
		}
	
		static BuildTarget? currentBuildTarget {
				get {
						return EditorPrefs.HasKey ("currentBuildTarget") ? (BuildTarget?)EditorPrefs.GetInt ("currentBuildTarget") : null;
				}
				set {
						if (value == null)
								EditorPrefs.DeleteKey ("currentBuildTarget");
						else
								EditorPrefs.SetInt ("currentBuildTarget", (int)value);
				}
		}
	
		static AndroidBuildSubtarget? currentBuildSubTarget {
				get {
						return EditorPrefs.HasKey ("currentBuildSubTarget") ? (AndroidBuildSubtarget?)EditorPrefs.GetInt ("currentBuildSubTarget") : null;
				}
				set {
						if (value == null)
								EditorPrefs.DeleteKey ("currentBuildSubTarget");
						else
								EditorPrefs.SetInt ("currentBuildSubTarget", (int)value);
				}
		}
	
		static AndroidBuildSubtarget? nextBuildSubTarget {
				get {
						return EditorPrefs.HasKey ("nextBuildSubTarget") ? (AndroidBuildSubtarget?)EditorPrefs.GetInt ("nextBuildSubTarget") : null;
				}
				set {
						if (value == null)
								EditorPrefs.DeleteKey ("nextBuildSubTarget");
						else
								EditorPrefs.SetInt ("nextBuildSubTarget", (int)value);
				}
		}
	
		#region Unity menu integration
	
		[MenuItem("Tools/Build/Sound/iOS")]
		static void ExportSound_IOS ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.iPhone;
		
				List<string> buildDirList = new List<string> ();
				buildDirList.Add ("Assets/AssetBundles/Sound");
		
				ExportAssetResources (buildDirList, false);
		}
	
		[MenuItem("Tools/Build/Sound/Android")]
		static void ExportSound_Android ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
		
				List<string> buildDirList = new List<string> ();
				buildDirList.Add ("Assets/AssetBundles/Sound");
		
				ExportAssetResources (buildDirList, false);
		}

		//Android All
		[MenuItem("Tools/Build/iOS/All")]
		static void ExportAll_iOS_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.iPhone;
				nextBuildSubTarget = AndroidBuildSubtarget.PVRTC;
				buildAllSubtarget = false;
		
				Export_Assets_Queue ();
		}

		//Android All
		[MenuItem("Tools/Build/Android/All")]
		static void ExportAll_Android_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
				nextBuildSubTarget = AndroidBuildSubtarget.ETC2;
				buildAllSubtarget = true;

				Export_Assets_Queue ();
		}

		static void Export_Assets_Queue ()
		{
				if (currentBuildTarget.HasValue && nextBuildSubTarget.HasValue) {
						currentBuildSubTarget = nextBuildSubTarget;
						nextBuildSubTarget = null;

						if (currentBuildTarget.Equals (BuildTarget.Android)) {
								if (buildAllSubtarget && androidSubtargetDictionary.ContainsKey (bundleVersionList [currentBuildSubTarget.Value] + 1))
										nextBuildSubTarget = androidSubtargetDictionary [bundleVersionList [currentBuildSubTarget.Value] + 1];
			                                                       
								JSFTPlatform.SwitchPlatform (JSFTShared.CacheTargetForBuildTarget (currentBuildTarget).Value, JSFTShared.CacheSubtargetForAndroidBuildSubtarget (currentBuildSubTarget).Value);

								if (EditorUserBuildSettings.activeBuildTarget.Equals (currentBuildTarget) && EditorUserBuildSettings.androidBuildSubtarget.Equals (currentBuildSubTarget)) {
										ExportSD_Asset ();
								}
						} else {
								JSFTPlatform.SwitchPlatform (JSFTShared.CacheTargetForBuildTarget (currentBuildTarget).Value);
				
								if (EditorUserBuildSettings.activeBuildTarget.Equals (currentBuildTarget)) {
										ExportSD_Asset ();
								}
						}
				}
		}
	
		//Android SD
		[MenuItem("Tools/Build/Android/SD/DXT5")]
		static void ExportAll_Android_SD_DXT5_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
				nextBuildSubTarget = androidSubtargetDictionary [bundleVersionList [AndroidBuildSubtarget.DXT]];
				buildAllSubtarget = false;
		
				Export_Assets_Queue ();
		}
	
		[MenuItem("Tools/Build/Android/SD/PVRTC_RGBA4")]
		static void ExportAll_Android_SD_PVRTC_RGBA4_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
				nextBuildSubTarget = androidSubtargetDictionary [bundleVersionList [AndroidBuildSubtarget.PVRTC]];
				buildAllSubtarget = false;
		
				Export_Assets_Queue ();
		}
	
		[MenuItem("Tools/Build/Android/SD/ATC_RGBA8")]
		static void ExportAll_Android_SD_ATC_RGBA8_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
				nextBuildSubTarget = androidSubtargetDictionary [bundleVersionList [AndroidBuildSubtarget.ATC]];
				buildAllSubtarget = false;
		
				Export_Assets_Queue ();
		}
	
		[MenuItem("Tools/Build/Android/SD/ETC2_RGBA8")]
		static void ExportAll_Android_SD_ETC2_RGBA8_Assets ()
		{
				savepath = "";
				currentBuildTarget = BuildTarget.Android;
				nextBuildSubTarget = androidSubtargetDictionary [bundleVersionList [AndroidBuildSubtarget.ETC2]];
				buildAllSubtarget = false;
		
				Export_Assets_Queue ();
		}
	
		static void ExportSD_Asset ()
		{
				List<string> buildDirList = new List<string> ();

				buildDirList.Add ("Assets/AssetBundles/Story/Scene01");
				buildDirList.Add ("Assets/AssetBundles/Story/Scene02");

				ExportAssetResources (buildDirList);
				Export_Assets_Queue ();
		}

		#endregion

		static void ExportAssetResources (List<string> buildDirList, bool isNeedCompression = true)
		{
				buildTarget = currentBuildTarget.Value;
				if (currentBuildSubTarget.HasValue)
						textureFormat = androidTextureImporterFormatDictionary [bundleVersionList [currentBuildSubTarget.Value]];

				if (string.IsNullOrEmpty (savepath))
						savepath = EditorUtility.SaveFilePanel ("Save Resource", "", "", "unity3d");

				foreach (string buildDir in buildDirList) {
						BuildAssetInDirectory (savepath, buildDir, isNeedCompression);
				}
		}

		static void BuildAssetInDirectory (string savepath, string buildDir, bool isNeedCompression = true)
		{
				string buildToPath = buildDir.Replace ("Assets/AssetBundles/", "");
				buildToPath = buildToPath.Replace ("/", "_");

				if (isNeedCompression) {
						buildToPath = Path.GetDirectoryName (savepath) + "/" + buildToPath + "_" + textureFormat + "_" + buildTarget + Path.GetExtension (savepath);
				} else {
						buildToPath = Path.GetDirectoryName (savepath) + "/" + buildToPath + "_" + buildTarget + Path.GetExtension (savepath);
				}

				string[] fileEntries = Directory.GetFiles (buildDir, "*.*", SearchOption.AllDirectories);
				string[] dependencies = AssetDatabase.GetDependencies (fileEntries);

				List<Object> assetList = new List<Object> ();
				int i = 0;

				foreach (string filePath in dependencies) {
						Object asset = AssetDatabase.LoadAssetAtPath (filePath, typeof(Object));
						EditorUtility.DisplayProgressBar ("Asset Bundle Process", "Processing asset : " + asset.ToString (), (float)++i / dependencies.Length);
						assetList.Add (asset);
				}
				
				Object[] assets = assetList.ToArray ();
				Selection.objects = assets;

				BuildPipeline.BuildAssetBundle (Selection.activeObject, assets, buildToPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
		}
}
