using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Manager : MonoBehaviour {
	[SerializeField]
	Board board;
	// Use this for initialization
	void Awake() {
		LeanTween.init (800);
	}

	void Start () {
		InitGame ();
	}
	
	void InitGame() {
		board.Fill ();
	}
}
