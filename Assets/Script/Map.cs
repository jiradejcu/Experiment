using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{

		void Start ()
		{
				for (int i=1; i<=5; i++) {
						GameObject bigObject = GameObject.Instantiate (Resources.Load ("Prefab/Big")) as GameObject;
						bigObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Image/Big" + i);
				}
		}

		public void Back ()
		{
				Application.LoadLevelAsync ("Main");
		}

}
