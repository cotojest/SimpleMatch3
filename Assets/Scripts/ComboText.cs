using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ComboText : MonoBehaviour {
	private Text t;

	void Start () {
		t = GetComponent<Text> ();
		t.text = "";
	}

	public void UpdateText(int comboValue) {
		if (comboValue > 1) {
			t.text = string.Format ("COMBO x{0}", comboValue);
		} else {
			t.text = "";
		}
	}
}
