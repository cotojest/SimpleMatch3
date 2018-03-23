using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Board : MonoBehaviour {
	public Slot[] slots { get; private set;}
	public State state { get; private set;}
	public int combo { 
		get {
			return _combo;
		}
		private set { 
			_combo = value;
			onComboChange.Invoke (_combo);
		}
	}
	private int _combo;
	public IntEvent onComboChange;
	private BlockGenerator[] generators;
	private HashSet<Slot> markedMatches;
	private Slot[] lastMove;
	private bool moveToCheck = false;

	public void Awake() {
		state = State.BlocksFalling;
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
			moveToCheck = true;
			state = State.Moving;
			Block toSwap = s2.content;
			s2.content = s1.content;
			s1.content = toSwap;
			lastMove [0] = s1;
			lastMove [1] = s2;
		}
	}

	public void RevertLastMove() {
		moveToCheck = false;
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
		HandleMatches ();
	}

	private void HandleMatches() {
		if (markedMatches.Count > 0) {
			moveToCheck = false;
			state = State.BlocksFalling;
			DestroyMarkedMatches ();
		} else {
			combo = 0;
			if (moveToCheck) {
				RevertLastMove ();
			} else if (HasPossibleMoves ()) {
				state = State.WaitingForMove;
			} else {
				Shuffle ();
			}
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

	private bool HasPossibleMoves() {
		foreach (Slot s in slots) {
			if (HasSlotPossibleMoves (s)) {
				return true;
			}
		}
		return false;
	}

	private bool HasSlotPossibleMoves(Slot s) {
		if (s.content) {
			if (HasSlotPossibleHorizontalMatches(s) || HasSlotPossibleVerticalMatches(s)) {
				return true;
			}
		}
		return false;
	}

	private bool HasSlotPossibleHorizontalMatches(Slot s) {
		int match = s.content.id;
		if (s.right) {
			if (HasMatchingContent(s.right, match)) {
				if (s.right.right && HasTwoMatchingContentInNeighbours(s.right.right, match)) {
					return true;
				}
				if (s.left && HasTwoMatchingContentInNeighbours(s.left, match)) {
					return true;
				}
			} else if (HasMatchingContent(s.right.right, match)) {
				if (HasMatchingContent (s.right.up, match) ||
					HasMatchingContent (s.right.down, match)) {
					return true;
				}
			}
		}
		return false;
	}

	private bool HasSlotPossibleVerticalMatches(Slot s) {
		int match = s.content.id;
		if (s.down) {
			if (HasMatchingContent(s.down, match)) {
				if (s.down.down && HasTwoMatchingContentInNeighbours(s.down.down, match)) {
					return true;
				}
				if (s.up && HasTwoMatchingContentInNeighbours(s.up, match)) {
					return true;
				}
			} else if (HasMatchingContent(s.down.down, match)) {
				if (HasMatchingContent (s.down.right, match) ||
					HasMatchingContent (s.down.left, match)) {
					return true;
				}
			}
		}
		return false;
	}

	private bool HasTwoMatchingContentInNeighbours(Slot s, int match) {
		int matchCount = 0;
		if (HasMatchingContent (s.left, match)) {
			matchCount++;
		}
		if (HasMatchingContent (s.right, match)) {
			matchCount++;
		}
		if (HasMatchingContent (s.up, match)) {
			matchCount++;
		}
		if (HasMatchingContent (s.down, match)) {
			matchCount++;
		}
		return matchCount > 1;
	}

	private bool HasMatchingContent(Slot s, int match) {
		return (s && s.content && s.content.id == match);
	}

	private void Shuffle() {
		List<Block> contentToShuffle = slots.Select (x => x.content).ToList();
		foreach (Slot s in slots) {
			int randomContentId = Random.Range (0, contentToShuffle.Count);
			s.content = contentToShuffle [randomContentId];
			contentToShuffle.RemoveAt (randomContentId);
		}
	}

	public enum State {BlocksFalling, CheckingMatches, WaitingForMove, Moving};
		
}
