using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using JSFTCacheManager = Jemast.LocalCache.CacheManager;
using JSFTShared = Jemast.LocalCache.Shared;
using JSFTPlatform = Jemast.LocalCache.Platform;

public class ExportAssetBundles
{
		public static string savepath = "";
		private static bool multipleExport = false;

		// Store current texture format for the TextureProcessor.
		public static TextureImporterFormat textureFormat = TextureImporterFormat.PVRTC_RGBA4;
		private static BuildTarget buildTarget;
		private static Queue<string> buildQueue;
		private static Dictionary<string, string> subtargetDictionary;

		#region Unity menu integration
	
		//Android All
		[MenuItem("Tools/Build/Android/All")]
		static void ExportAll_Android_Assets ()
		{
				subtargetDictionary = new Dictionary<string, string> ();
				subtargetDictionary.Add (TextureImporterFormat.PVRTC_RGBA4.ToString (), JSFTShared.CacheSubtarget.Android_PVRTC.ToString ());
				subtargetDictionary.Add (TextureImporterFormat.ATC_RGBA8.ToString (), JSFTShared.CacheSubtarget.Android_ATC.ToString ());
				
				multipleExport = true;
				JSFTPlatform.PlatformChange = new JSFTPlatform.CallbackFunction (ExportSD_Asset);
				buildQueue = new Queue<string> ();
				buildQueue.Enqueue (TextureImporterFormat.PVRTC_RGBA4.ToString ());
				buildQueue.Enqueue (TextureImporterFormat.ATC_RGBA8.ToString ());
				ExportAll_Android_Assets_Queue ();
				multipleExport = false;
		}

		static void ExportAll_Android_Assets_Queue ()
		{
				if (buildQueue.Count > 0) {
						string target = buildQueue.Dequeue ();
						if (target == TextureImporterFormat.PVRTC_RGBA4.ToString ()) {
								ExportAll_Android_SD_PVRTC_RGBA4_Assets ();
						} else if (target == TextureImporterFormat.ATC_RGBA8.ToString ()) {
								ExportAll_Android_SD_ATC_RGBA8_Assets ();
						}
				
						if (buildTarget.ToString () == JSFTCacheManager.CurrentCacheTarget.ToString () && subtargetDictionary [textureFormat.ToString ()] == JSFTCacheManager.CurrentCacheSubtarget.ToString ())
								ExportSD_Asset ();
				}
		}

		//Android SD
		[MenuItem("Tools/Build/Android/SD/DXT5")]
		static void ExportAll_Android_SD_DXT5_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.DXT5;
				JSFTCacheManager.SwitchPlatform (JSFTShared.CacheTarget.Android, JSFTShared.CacheSubtarget.Android_DXT, true);
		}

		[MenuItem("Tools/Build/Android/SD/PVRTC_RGBA4")]
		static void ExportAll_Android_SD_PVRTC_RGBA4_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.PVRTC_RGBA4;
				JSFTCacheManager.SwitchPlatform (JSFTShared.CacheTarget.Android, JSFTShared.CacheSubtarget.Android_PVRTC, true);
		}

		[MenuItem("Tools/Build/Android/SD/ATC_RGBA8")]
		static void ExportAll_Android_SD_ATC_RGBA8_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.ATC_RGBA8;
				JSFTCacheManager.SwitchPlatform (JSFTShared.CacheTarget.Android, JSFTShared.CacheSubtarget.Android_ATC, true);
		}

		[MenuItem("Tools/Build/Android/SD/ETC2_RGBA8")]
		static void ExportAll_Android_SD_ETC2_RGBA8_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.ETC2_RGBA8;
				JSFTCacheManager.SwitchPlatform (JSFTShared.CacheTarget.Android, JSFTShared.CacheSubtarget.Android_ETC2, true);
		}

		static void ExportSD_Asset ()
		{
				List<string> buildDirList = new List<string> ();

				buildDirList.Add ("Assets/AssetBundles/Story/Scene01");
				buildDirList.Add ("Assets/AssetBundles/Story/Scene02");

				ExportAssetResources (buildDirList);
				ExportAll_Android_Assets_Queue ();
		}

		#endregion

		static void ExportAssetResources (List<string> buildDirList, bool isNeedCompression = true)
		{
				if (string.IsNullOrEmpty (savepath) || !multipleExport)
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
