using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour, IBlockSupplier {
	[SerializeField]
	Block[] blockPrefabs;
	[SerializeField]
	Slot[] receivers;
	Block preparedBlock;

	void Awake() {
		preparedBlock = GenerateBlock ();
		foreach (Slot s in receivers) {
			s.OnContentEmpty.AddListener (() => CheckReceivers());
		}
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube (transform.position, Vector3.one);
	}

	public bool CheckReceivers() {
		bool generatedBlocks = false;
		foreach (Slot s in receivers) {
			if (s.state == Slot.State.Empty) {
				GiveBlock(s);
				Debug.Log ("give");
				generatedBlocks = true;	
			}
		}
		return generatedBlocks;
	}

	public void GiveBlock(Slot receiver) {
		Block blockToGive = preparedBlock;
		preparedBlock = GenerateBlock ();
		receiver.content = blockToGive;
	}

	Block GenerateBlock() {
		int randomBlockIndex = Random.Range (0, blockPrefabs.Length);
		Block block = Instantiate (blockPrefabs[randomBlockIndex], transform, false);
		return block;
	}





}
