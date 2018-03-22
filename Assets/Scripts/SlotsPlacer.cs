using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsPlacer : MonoBehaviour {
	[SerializeField]
	Slot slotPrefab;
	[SerializeField]
	int squareSize;
	[SerializeField]
	float slotSize;
	Slot[] slots;

	[ContextMenu("Place")]
	void Place () {
		slots = CreateSlots ();
		for (int i = 0; i < squareSize; i++) {
			for (int j = 0; j < squareSize; j++) {
				int current = i * squareSize + j;
				slots[current].transform.localPosition = new Vector3 (i * slotSize, j * slotSize);
				SetNeighbours (current, i, j);
				SetReceivers (current, i, j);
			}
		}
	}

	Slot[] CreateSlots() {
		Slot[] toReturn = new Slot[squareSize*squareSize];
		for (int i = 0; i < toReturn.Length; i++) {
			toReturn [i] = Instantiate (slotPrefab, transform);
			toReturn [i].name = string.Format ("Slot{0}", i);
		}
		return toReturn;
	}
		
	void SetNeighbours(int current, int i, int j) {
		if (i > 0) {
			slots [current].down = slots [(i - 1) * squareSize + j];
		}
		if (i < squareSize - 1) {
			slots [current].up = slots [(i + 1) * squareSize + j];
		}
		if (j > 0) {
			slots [current].left = slots [current - 1];
		}
		if (j < squareSize - 1) {
			slots [current].right = slots [current + 1];
		}
	}

	void SetReceivers(int current, int i, int j) {
		if (j > 0) {
			slots [current].receivers = new Slot[] {slots [current - 1]};
		}
	}
}
