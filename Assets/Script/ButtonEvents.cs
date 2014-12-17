using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonEvents : MonoBehaviour
{
		private dfButton _button;

		public void Start ()
		{
				this._button = GetComponent<dfButton> ();
		}

		public void OnClick (dfControl control, dfMouseEventArgs mouseEvent)
		{
				GameObject.Destroy (transform.root.gameObject);
				Application.LoadLevel ("Main");
		}
}
