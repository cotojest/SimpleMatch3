using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	public int id;

	public void DestroyAnimation(System.Action onComplete = null) {
		LeanTween.scale (gameObject, Vector3.zero, 0.2f).setEaseInBounce ()
			.setOnComplete (() => { 
				if (onComplete != null) {
					onComplete.Invoke ();
				}
				Destroy (gameObject);
		});
	}

}
