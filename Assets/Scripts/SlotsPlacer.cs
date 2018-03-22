using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsPlacer : MonoBehaviour {
	[SerializeField]
	Slot slotPrefab;
	[SerializeField]
	BlockGenerator generatorPrefab;
	[SerializeField]
	Transform generatorParent;
	[SerializeField]
	Vector3 generatorOffset = new Vector3(0, 1);
	[SerializeField]
	int squareSize;
	[SerializeField]
	float slotSize;
	Slot[] slots;

	[ContextMenu("Place")]
	void Place () {
		int squareSize = (int)Mathf.Sqrt (slots.Length);
		slots = CreateSlots ();
		for (int i = 0; i < squareSize; i++) {
			for (int j = 0; j < squareSize; j++) {
				int current = i * squareSize + j;
				slots[current].transform.localPosition = new Vector3 (i * slotSize, j * slotSize);
				SetNeighbours (current, i, j);
				SetSuppliers (current, i, j);
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

	BlockGenerator CreateGenerator(int current) {
		BlockGenerator toReturn = Instantiate (generatorPrefab, generatorParent);
		toReturn.transform.position = slots [current].transform.position + generatorOffset;
		toReturn.name = string.Format ("Generator{0}", current);
		return toReturn;
	}

	void SetNeighbours(int current, int i, int j) {
		if (i > 0 && i < squareSize - 1) {
			slots [current].horizontalNeighbours = new Slot[] { slots [(i + 1) * squareSize + j],
																slots [(i - 1) * squareSize + j]};
		} else if (i == 0) {
			slots [current].horizontalNeighbours = new Slot[] { slots [(i + 1) * squareSize + j] };
		} else {
			slots [current].horizontalNeighbours = new Slot[] { slots [(i - 1) * squareSize + j] };
		}

		if (j > 0 && j < squareSize - 1) {
			slots [current].verticalNeighbours = new Slot[] { slots [current + 1], slots [current - 1]};
		} else if (j == 0) {
			slots [current].verticalNeighbours = new Slot[] { slots [current + 1] };
		} else {
			slots [current].verticalNeighbours = new Slot[] { slots [current - 1] };
		}
	}

	void SetSuppliers(int current, int i, int j) {
		if (j < squareSize - 1) {
			slots [current].blockSuppliers = 
				new BlockSupplier[] { slots [current + 1].gameObject.AddComponent<BlockPusher>() };
		} else {
			slots [current].blockSuppliers = new BlockSupplier[] { CreateGenerator(current) };
		}
	}

}
