using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuessCard : MonoBehaviour {

	public CardTypes Type;
	public List<string> Options = new List<string> ();
	public int CurrentIndex = 0;
	public string Current = "Press Arrows";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string Text {
		set {
			GetComponentInChildren<Text> ().text = value;
		}
	}

	public void ToggleCardUp () {
		CurrentIndex += 1;
		if (CurrentIndex >= Options.Count)
			CurrentIndex = 0;
		if (CurrentIndex >= Options.Count)
			Current = "Press Arrows";
		else
			Current = Options [CurrentIndex];
		GetComponentInChildren<Text> ().text = Current;
	}

	public void ToggleCardDown () {
		CurrentIndex -= 1;
		if (CurrentIndex < 0)
			CurrentIndex = Options.Count - 1;
		if (CurrentIndex >= Options.Count || CurrentIndex < 0)
			Current = "Press Arrows";
		else
			Current = Options [CurrentIndex];
		GetComponentInChildren<Text> ().text = Current;
	}
}
