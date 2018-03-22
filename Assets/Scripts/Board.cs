using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	[SerializeField]
	Slot[] slots;
	[SerializeField]
	BlockGenerator[] generators;

	public void Awake() {
		slots = GetComponentsInChildren<Slot> ();
	}

	public void Fill() {
		foreach(BlockGenerator bg in generators) {
			bg.CheckReceivers ();
		}
	}

}
