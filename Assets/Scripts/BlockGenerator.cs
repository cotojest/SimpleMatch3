using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : BlockSupplier {
	[SerializeField]
	Block[] blockPrefabs;

	Slot[] receivers;
	Block preparedBlock;

	void Start() {
		hasBlock = true;
		preparedBlock = GenerateBlock ();
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube (transform.position, Vector3.one);
	}

	public override Block GiveBlock() {
		Block toReturn = preparedBlock;
		preparedBlock = GenerateBlock ();
		return toReturn;
	}

	Block GenerateBlock() {
		int randomBlockIndex = Random.Range (0, blockPrefabs.Length);
		Block block = Instantiate (blockPrefabs[randomBlockIndex], transform, false);
		return block;
	}





}
