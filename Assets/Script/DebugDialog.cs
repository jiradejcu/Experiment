using UnityEngine;
using System.Collections;

public class DebugDialog : MonoBehaviour
{
		void Start ()
		{
				GameObject.DontDestroyOnLoad (gameObject);
		}

		void OnGUI ()
		{
				ClearAsset ();
		}

		void ClearAsset ()
		{
				if (GUI.Button (new Rect (10f, 10f, 100f, 50f), "Clear Asset")) {
						dfAtlas atlas = GameObject.FindObjectOfType<dfAtlas> ();
						if (atlas != null)
								GameObject.DestroyImmediate (atlas.gameObject);
						dfMaterialCache.Clear ();
//						Resources.UnloadUnusedAssets ();
						Texture2D[] textureList = Resources.FindObjectsOfTypeAll<Texture2D> ();
						foreach (Texture2D tex in textureList) {
								if (tex.name.Equals ("mainscreen")) {
										Debug.Log ("found");
										Resources.UnloadAsset (tex);
								}
						}
				}
		}

}
