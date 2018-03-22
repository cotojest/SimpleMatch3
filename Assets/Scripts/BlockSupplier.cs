using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockSupplier : MonoBehaviour {
	public bool hasBlock;
	public abstract Block GiveBlock ();
}
