// Builds an asset bundle from the selected objects in the project view,
// and changes the texture format using an AssetPostprocessor.
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ExportAssetBundles
{
		public static string savepath = "";
		private static bool multipleExport = false;

		// Store current texture format for the TextureProcessor.
		public static TextureImporterFormat textureFormat = TextureImporterFormat.PVRTC_RGBA4;
		private static BuildTarget buildTarget;

		#region Unity menu integration
	
		//Android All
		[MenuItem("Tools/Build/Android/All")]
		static void ExportAll_Android_Assets ()
		{
				multipleExport = true;
				ExportAll_Android_SD_DXT5_Assets ();
				ExportAll_Android_SD_PVRTC_RGBA4_Assets ();
				ExportAll_Android_SD_ATC_RGBA8_Assets ();
				ExportAll_Android_SD_ETC2_RGBA8_Assets ();
				multipleExport = false;
		}

		//Android SD
		[MenuItem("Tools/Build/Android/SD/DXT5")]
		static void ExportAll_Android_SD_DXT5_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.DXT5;
				ExportSD_Asset ();
		}

		[MenuItem("Tools/Build/Android/SD/PVRTC_RGBA4")]
		static void ExportAll_Android_SD_PVRTC_RGBA4_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.PVRTC_RGBA4;
				ExportSD_Asset ();
		}

		[MenuItem("Tools/Build/Android/SD/ATC_RGBA8")]
		static void ExportAll_Android_SD_ATC_RGBA8_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.ATC_RGBA8;
				ExportSD_Asset ();
		}

		[MenuItem("Tools/Build/Android/SD/ETC2_RGBA8")]
		static void ExportAll_Android_SD_ETC2_RGBA8_Assets ()
		{
				buildTarget = BuildTarget.Android;
				textureFormat = TextureImporterFormat.ETC2_RGBA8;
				ExportSD_Asset ();
		}

		static void ExportSD_Asset ()
		{
				List<string> buildDirList = new List<string> ();

				buildDirList.Add ("Assets/AssetBundles/Story/Scene01");
				buildDirList.Add ("Assets/AssetBundles/Story/Scene02");

				ExportAssetResources (buildDirList);
		}

		#endregion

		[MenuItem("Assets/Show Dependencies")]
		static void ExportDependencies ()
		{
				ShowDependencies ();       
		}

		[MenuItem("Assets/Build DXT5")]
		static void ExportDXT5 ()
		{
				buildTarget = EditorUserBuildSettings.activeBuildTarget;
				textureFormat = TextureImporterFormat.DXT5;
				ExportSelectResource ();    
		}

		[MenuItem("Assets/Build PVRTC_RGBA4")]
		static void ExportPVRTC_RGBA4 ()
		{
				buildTarget = EditorUserBuildSettings.activeBuildTarget;
				textureFormat = TextureImporterFormat.PVRTC_RGBA4;
				ExportSelectResource ();       
		}

		[MenuItem("Assets/Build ATC_RGBA8")]
		static void ExportATC_RGBA8 ()
		{
				buildTarget = EditorUserBuildSettings.activeBuildTarget;
				textureFormat = TextureImporterFormat.ATC_RGBA8;
				ExportSelectResource ();
		}

		[MenuItem("Assets/Build ETC2_RGBA8")]
		static void ExportETC2_RGBA8 ()
		{
				buildTarget = EditorUserBuildSettings.activeBuildTarget;
				textureFormat = TextureImporterFormat.ETC2_RGBA8;
				ExportSelectResource ();
		}

		private static bool isSelectDependencies = true;

		static void ExportResource ()
		{
				// Bring up save panel.
				if (isSelectDependencies) {
						savepath = EditorUtility.SaveFilePanel ("Save Resource", "", EditorUserBuildSettings.activeBuildTarget.ToString () + "_EN_HD", "unity3d");
				} else {
						savepath = EditorUtility.SaveFilePanel ("Save Resource", "", Selection.activeObject.ToString () + "_EN_HD", "unity3d");
				}
				
				string path = savepath;	

				string saveName = Path.GetFileNameWithoutExtension (path);
				path = path.Replace (saveName, saveName + "_" + textureFormat);
				Debug.Log ("ExportResource at path = " + path);

				if (path.Length != 0) {
						if (isSelectDependencies) {
								Debug.Log ("build with dependency");
								// Build the resource file from the active selection.
								Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			
								int i = 0;
								foreach (object asset in selection) {
										EditorUtility.DisplayProgressBar ("Asset Bundle Process", "Processing asset : " + asset.ToString (), (float)++i / selection.Length);
										string assetPath = AssetDatabase.GetAssetPath ((UnityEngine.Object)asset);
										Debug.Log ("assetPath = " + assetPath);
										if (asset is Texture2D) {
												// Force reimport thru TextureProcessor.
												AssetDatabase.ImportAsset (assetPath);
												Debug.Log ("asset is Texture2D");
										}
								}

								BuildPipeline.BuildAssetBundle (Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);				
								Selection.objects = selection;
						} else {
								Debug.Log ("build with no-dependency");
								BuildPipeline.BuildAssetBundle (Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);
						}
				}
		}

		static void ExportSelectResource ()
		{
				// Bring up save panel.
				string selectpath = AssetDatabase.GetAssetPath (Selection.activeObject);
		
				string savepath = EditorUtility.SaveFilePanel ("Save Resource", "", "", "unity3d");

				BuildAssetInDirectory (savepath, selectpath);
		}

		static void ExportAssetResources (List<string> buildDirList, bool isNeedCompression = true)
		{
				// Bring up save panel.
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

				Debug.Log ("buildToPath = " + buildToPath);

				string[] fileEntries = Directory.GetFiles (buildDir, "*.*", SearchOption.AllDirectories);
				Debug.Log ("fileEntries = " + fileEntries.Length);
				string[] dependencies = AssetDatabase.GetDependencies (fileEntries);
				Debug.Log ("dependencies at path length = " + dependencies.Length);

				//need to build asset from here
				ArrayList assetList = new ArrayList ();
				int i = 0;

				foreach (string filePath in dependencies) {
						Debug.Log ("filename : " + filePath);
						Object asset = AssetDatabase.LoadAssetAtPath (filePath, typeof(Object));

						EditorUtility.DisplayProgressBar ("Asset Bundle Process", "Processing asset : " + asset.ToString (), (float)++i / dependencies.Length);
						if (asset != null) {
								Debug.Log (asset.name);
								if (asset is Texture2D) {
										// Force reimport thru TextureProcessor.
										AssetDatabase.ImportAsset (filePath);
										// Debug.Log ("asset is Texture2D");
								}
						}
						assetList.Add (asset);
				}
				
				Object[] assets = (Object[])assetList.ToArray (typeof(Object));
				Selection.objects = assets;

				BuildPipeline.BuildAssetBundle (Selection.activeObject, assets, buildToPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
		}

		static void ShowDependencies ()
		{
				Debug.Log ("build with dependency");
				// Build the resource file from the active selection.
				Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
				Selection.objects = EditorUtility.CollectDependencies (selection);

				string assetList = "";

				foreach (Object asset in Selection.objects) {
						assetList += asset.name + "\n";
				}

				Debug.Log (assetList);
		}
}
