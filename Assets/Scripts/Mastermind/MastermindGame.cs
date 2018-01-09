using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastermindGame : MonoBehaviour {

	private List<string> Possibilities = new List<string> ();
	public List<Clue> Clues = new List<Clue> ();
	public Clue LastClue;
	public int RW = 0;
	public int RR = 0;
	public string Answer;


	// Use this for initialization
	void Start () {



		string Text = "Hello 1 World";
		//Debug.Log(Text.IndexOf ("1"));


		string Number = "652754217868";
		//Debug.Log(Number.IndexOf ("1"));

		//Debug.Log(Number.Contains ("1"));
		//Debug.Log(Number.Contains ("9"));

		Clue FirstClue = new Clue ("12345", 2, 2);
		Clue SecondClue = new Clue ("12345", 2, 3);

		//Debug.Log (FirstClue.CheckPossible ("12345"));

		EndlessPossibilities ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			List<string> ToRemove = new List<string> ();
			LastClue.Results = LastClue.FindHint (Answer);
			Debug.Log ("RR: " + LastClue.Results.RR + ", RW: " + LastClue.Results.RW);
			//LastClue.Results.RW = RW;
			//LastClue.Results.RR = RR;
			Debug.Log ("Finding Invalids");
			foreach (string P in Possibilities) {
				if (!LastClue.CheckPossible (P))
					ToRemove.Add (P);
			}
			Debug.Log ("Removing");
			foreach (string R in ToRemove)
				Possibilities.Remove (R);
			if (Possibilities.Count == 1) {
				Debug.Log ("The answer is " + Possibilities [0]);
			} else if (Possibilities.Count < 1) {
				Debug.Log ("Something went wrong man.");
			} else {
				Debug.Log ("There are " + Possibilities.Count + " Possibilities left.");
				Debug.Log ("Choosing Guess");
				Clues.Add (LastClue);
				LastClue = new Clue (Possibilities [Random.Range (0, Possibilities.Count - 1)]);
				Debug.Log (LastClue.Combination);
			}
		}
		if (Input.GetKeyDown (KeyCode.Tab)) {
			EndlessPossibilities ();
		}
	}

	public void EndlessPossibilities () {
		int Length = Answer.Length;
		int NumColours = 0;
		for (int i = 0; i < 10; ++i) {
			if (Answer.Contains (i.ToString ()))
				NumColours = i + 1;
		}

		Debug.Log ("Creating " + NumColours + " ^ " + Length + " combinations.");
		CreatePossibilities (Length, NumColours);

		LastClue = new Clue (Possibilities [Random.Range (0, Possibilities.Count - 1)]);
		Debug.Log (LastClue.Combination);
	}

	public void CreatePossibilities (int Length, int NumColours) {
		Possibilities = new List<string> ();
		//float StartTime = Time.time;
		int Counter = 0;
		int Max = (int)Mathf.Pow(NumColours, Length);

		string Combination = "";
		int TempNumber = 0;
		int Digit = 0;

		string TempPoss = "";
		for (int i = 0; i < (int)Mathf.Pow (10, Length); ++i) {
			TempPoss = "" + i;
			bool IsValid = true;
			for (int j = NumColours; j < 10; ++j) {
				if (TempPoss.Contains (j.ToString ())) {
					IsValid = false;
					break;
				}
			}
			if (i < (int)Mathf.Pow (10, Length - 1)) {
				while (TempPoss.Length < Length) 
					TempPoss = "0" + TempPoss;
			}
			if (IsValid)
				Possibilities.Add (TempPoss);
		}
		Debug.Log (Max + " combinations created.");
		//Debug.Log ((Time.time - StartTime) + " seconds");
		//foreach (string P in Possibilities) {
		//	Debug.Log (P);
		//}

	}
}

public class Hints {
	public int RW = -1;
	public int RR = -1;

	public Hints () {
	}

	public Hints (int rw, int rr) {
		RW = rw;
		RR = rr;
	}

	public bool Equals (Hints Other) {
		return (Other.RR == RR && Other.RW == RW);
	}
}

public class Clue {
	public string Combination;
	public Hints Results = new Hints();

	public Clue (string Guess) {
		Combination = Guess;
	}

	public Clue (string Guess, int rr, int rw) {
		Combination = Guess;
		Results = new Hints (rr, rw);
	}

	public bool CheckPossible (string GuessCombo) {
		//Should change to 'used elements' instead of char arrays.
		char[] GuessChars = GuessCombo.ToCharArray();
		string GuessStr = GuessCombo;
		Hints VirtualResults = new Hints (0, 0);
		int i = 0;
		foreach (char C in Combination.ToCharArray()) {
			if (GuessStr.Contains (C.ToString ())) {
				if (GuessChars [i] == C) {
					VirtualResults.RR += 1;
					GuessChars [i] = char.Parse("Z");
				} else {
					VirtualResults.RW += 1;
					int j = GuessStr.IndexOf (C);
					GuessChars [j] = char.Parse("Z");
				}
			}
			GuessStr = new string(GuessChars);
			i += 1;
		}
		return (Results.Equals (VirtualResults));
	}

	public Hints FindHint (string GuessCombo) {
		char[] GuessChars = GuessCombo.ToCharArray();
		string GuessStr = GuessCombo;
		Hints VirtualResults = new Hints (0, 0);
		int i = 0;
		foreach (char C in Combination.ToCharArray()) {
			if (GuessStr.Contains (C.ToString ())) {
				if (GuessChars [i] == C) {
					VirtualResults.RR += 1;
					GuessChars [i] = char.Parse("Z");
				} else {
					VirtualResults.RW += 1;
					int j = GuessStr.IndexOf (C);
					GuessChars [j] = char.Parse("Z");
				}
			}
			GuessStr = new string(GuessChars);
			i += 1;
		}
		return VirtualResults;
	}
}
