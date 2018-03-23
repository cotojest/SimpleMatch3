using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match3Manager : MonoBehaviour {
	[SerializeField]
	Board board;
	[SerializeField]
	Text pointsText;

	Slot userSelection;
	int points;

	void Start () {
		points = 0;
		UpdatePointsText ();
		PointsCounter.OnPointsGiven.AddListener (AddPoints);
		RegisterToSlotClicks ();
		InitGame ();
	}
	
	void InitGame() {
		board.Fill ();
	}

	void RegisterToSlotClicks () {
		foreach (Slot s in board.slots) {
			s.OnMoveEnd.AddListener (OnSlotPointerUp);
			s.OnMoveBegin.AddListener (OnSlotPointerDown);
		}
	}
		
	void OnSlotPointerDown(Slot slot) {
		userSelection = slot;
	}

	void OnSlotPointerUp(Slot slot) {
		if (userSelection != null) {
			board.Move (userSelection, slot);
			userSelection = null;
		}
	}
		
	bool IsNeighbourWithSelected (Slot slot) {
		return userSelection.down == slot || userSelection.up == slot ||
		userSelection.right == slot || userSelection.left == slot;
	}

	void AddPoints(int addedPoints) {
		points += (addedPoints * board.combo);
		UpdatePointsText ();
	}

	void UpdatePointsText() {
		pointsText.text = points.ToString ("D6");
	}
}
