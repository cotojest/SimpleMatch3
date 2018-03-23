using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour {
	public Slot[] slots { get; private set;}
	public State state { get; private set;}

	private BlockGenerator[] generators;
	private HashSet<Slot> markedMatches;
	private int combo;
	private Slot[] lastMove;

	public void Awake() {
		state = State.BlocksFalling;
		combo = 0;
		lastMove = new Slot[2];
		slots = GetComponentsInChildren<Slot> ();
		generators = GetComponentsInChildren<BlockGenerator> ();
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

	public void Move(Slot s1, Slot s2) {
		if (state == State.WaitingForMove) {
			state = State.Moving;
			Block toSwap = s2.content;
			s2.content = s1.content;
			s1.content = toSwap;
		}
		lastMove [0] = s1;
		lastMove [1] = s2;
	}

	public void RevertLastMove() {
		Block toSwap = lastMove [0].content;
		lastMove [0].content = lastMove [1].content;
		lastMove [1].content = toSwap;
	}

	public void CheckForMatches() {
		combo++;
		state = State.CheckingMatches;
		markedMatches.Clear ();
		foreach (Slot s in slots) {
			HashSet<Slot> chain = new HashSet<Slot> ();
			CheckDownChain (ref chain, s);
			if (chain.Count >= 3) {
				markedMatches.UnionWith (chain);
			}
			chain.Clear ();
			CheckRightChain (ref chain, s);
			if (chain.Count >= 3) {
				markedMatches.UnionWith (chain);
			}
		}
		if (markedMatches.Count > 0) {
			state = State.BlocksFalling;
			DestroyMarkedMatches ();
		} else {
			combo = 0;
			state = State.WaitingForMove;
		}
	}


	private void DestroyMarkedMatches() {
		foreach (Slot s in markedMatches) {
			s.DestroyContent ();
		}
	}

	private void CheckDownChain(ref HashSet<Slot> chain, Slot s) {
		chain.Add (s);
		if (s.down && s.down.content && s.down.content.id == s.content.id) {
			CheckDownChain (ref chain, s.down);
		};
	}

	private void CheckRightChain(ref HashSet<Slot> chain, Slot s) {
		chain.Add (s);
		if (s.right && s.right.content && s.right.content.id == s.content.id) {
			CheckRightChain (ref chain, s.right);
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

	public enum State {BlocksFalling, CheckingMatches, WaitingForMove, Moving};
		
}
