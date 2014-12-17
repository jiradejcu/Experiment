using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour
{
		void Start ()
		{
				GameObject.DontDestroyOnLoad (gameObject);
		}
}
