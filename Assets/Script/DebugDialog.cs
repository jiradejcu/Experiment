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
						dfMaterialCache.Clear ();
						Resources.UnloadUnusedAssets ();
				}
		}

}
