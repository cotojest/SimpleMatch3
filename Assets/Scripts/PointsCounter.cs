using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> {
}

public static class PointsCounter  {

	public static IntEvent OnPointsGiven = new IntEvent();

	public static void GivePoints(int points) {
		OnPointsGiven.Invoke (points);
	}
}
