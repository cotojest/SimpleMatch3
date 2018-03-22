using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour {
	[SerializeField]
	Slot[] slots;
	[SerializeField]
	BlockGenerator[] generators;

	private HashSet<Slot> markedMatches;


	public void Awake() {
		slots = GetComponentsInChildren<Slot> ();
		foreach (Slot s in slots) {
			s.OnReady.AddListener (OnSlotReady);
		}
		markedMatches = new HashSet<Slot> ();
	}

	public void Fill() {
		foreach(BlockGenerator bg in generators) {
			bg.CheckReceivers ();
		}
	}

	public void CheckForMatches() {
		markedMatches.Clear ();
		foreach (Slot s in slots) {
			HashSet<Slot> chain = new HashSet<Slot> ();
			CheckLeftChain (ref chain, s);
			if (chain.Count >= 3) {
				markedMatches.UnionWith (chain);
			}
			chain.Clear ();
			CheckUpChain (ref chain, s);
			if (chain.Count >= 3) {
				markedMatches.UnionWith (chain);
			}
		}
		DestroyMarkedMatches ();
	}


	private void DestroyMarkedMatches() {
		foreach (Slot s in markedMatches) {
			s.DestroyContent ();
		}
	}

	private void CheckLeftChain(ref HashSet<Slot> chain, Slot s) {
		chain.Add (s);
		if (s.left && s.left.content && s.left.content.id == s.content.id) {
			CheckLeftChain (ref chain, s.left);
		};
	}

	private void CheckUpChain(ref HashSet<Slot> chain, Slot s) {
		chain.Add (s);
		if (s.up && s.up.content && s.up.content.id == s.content.id) {
			CheckUpChain (ref chain, s.up);
		};
	}

	private void OnSlotReady() {
		foreach (Slot s in slots) {
			if (s.state != Slot.State.BlockReady) {
				return;
			}
		}
		CheckForMatches ();
	}
		
}
