using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	public int id;
	[SerializeField]
	int pointValue;

	public void DestroyAnimation(System.Action onComplete = null) {
		LeanTween.scale (gameObject, Vector3.zero, 0.4f).setEaseInBack ()
		.setOnComplete (() => { 
			if (onComplete != null) {
				onComplete.Invoke ();
			}
			Destroy (gameObject);
		});
	}

	void OnDestroy() {
		PointsCounter.GivePoints (pointValue);
	}
}
