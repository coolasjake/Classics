using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
	public float StartTime;
	public bool Started = false;

	void Start () {
		StartTime = Time.time;
		GetComponent<Text> ().text = "0";
	}

	// Update is called once per frame
	void Update () {
		if (Started)
			GetComponent<Text> ().text = "" + Mathf.FloorToInt(Time.time - StartTime);
	}

	public void StartTimer () {
		Started = true;
		StartTime = Time.time;
	}

	public void StopTimer () {
		Started = false;
	}
}
