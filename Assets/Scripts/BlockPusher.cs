using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPusher : BlockSupplier {
	Slot slot;

	public void Awake() {
		slot = GetComponent<Slot> ();
		hasBlock = false;
	}

	public override Block GiveBlock() {
		hasBlock = false;
		Block toReturn = slot.content;
		slot.content = null;
		return toReturn;
	}

	public void OnBlockArrived() {
		hasBlock = true;
	}
}
