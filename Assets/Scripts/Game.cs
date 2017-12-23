﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public List<GameObject> Deck = new List<GameObject> ();
	public List<GameObject> Play = new List<GameObject> ();
	public List<GameObject> Sets = new List<GameObject> ();
	public List<GameObject> SelectedCards = new List<GameObject> ();

	private int[] PlayerScores = new int[4];
	public int Timer = 0;
	public int TurnTime = 300;
	public int CurrentTurn = -1;

	public Canvas MainCanvas;

	public int PlaySize = 25;

	private Vector3 ScreenCenter;

	// Use this for initialization
	void Start () {
		ScreenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2));
		MainCanvas = FindObjectOfType<Canvas> ();

		for (int i = 0; i < 4; ++i) {
			PlayerScores [i] = 0;
			MainCanvas.GetComponentsInChildren<Text> () [i].text = "" + PlayerScores [i];
		}

		Vector2 Offset = new Vector2(ScreenCenter.x - 8, ScreenCenter.y - 4);
		GameObject CardPrefab = Resources.Load<GameObject> ("Prefabs/Card");
		GameObject NewCard;
		for (int i = 0; i < 81; ++i) {
			NewCard = Instantiate (CardPrefab, transform);
			NewCard.GetComponent<Card>().TargetPos = new Vector3 (Offset.x + Mathf.Floor (i / 9f) * 2, Offset.y + (i % 9f) * 1, 1);
			NewCard.transform.localPosition = new Vector3 (-3, -3, 1);
			NewCard.GetComponent<Card> ().SetDesign (i);
			Deck.Add (NewCard);
		}
	}

	void FixedUpdate () {
		if (CurrentTurn != -1) {
			if (Timer < TurnTime)
				Timer += 1;
			else if (Timer >= TurnTime)
				TurnEnded (CurrentTurn + 1, true);
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			StartGame ();
		}
			
		if (Input.GetKeyDown (KeyCode.C))
			Clue ();

		if (Input.GetMouseButtonUp (0) && CurrentTurn != -1) {
			//Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			bool ChoseCard = false;
			bool FoundSet = false;
			foreach (GameObject C in Play) {
				Bounds B = new Bounds (C.transform.position, new Vector3(2, 1, 60));
				if (B.Contains(Camera.main.ScreenToWorldPoint (Input.mousePosition))) {
					ChoseCard = true;
					if (SelectedCards.Count < 3) {
						if (!SelectedCards.Contains (C)) {
							SelectedCards.Add (C);
							C.transform.localScale = new Vector3 (1.05f, 1.05f, 1);
						}
						if (SelectedCards.Count >= 3) {
							if (CheckSet (SelectedCards [0].GetComponent<Card>(), SelectedCards [1].GetComponent<Card>(), SelectedCards [2].GetComponent<Card>())) {
								//Remove cards, replace them, and add a 'point'.
								Debug.Log("Set!");
								FoundSet = true;
								PlayerScores [CurrentTurn] += 1;
								MainCanvas.GetComponentsInChildren<Text> () [CurrentTurn].text = "" + PlayerScores [CurrentTurn];						Timer = 0;
								TurnEnded (CurrentTurn + 1, false);
							} else {
								//Deselect all cards, give a warning ("Not a set") or lose a point.
								foreach (GameObject aCard in SelectedCards)
									aCard.transform.localScale = new Vector3 (1, 1, 1);
								SelectedCards = new List<GameObject> ();
							}
						}
					}
				}
			}
			if (FoundSet) {
				RemoveAndReplaceSelected ();
				Debug.Log(FindNumberOfSets () + " sets left.");
			}
			if (ChoseCard == false) {
				foreach (GameObject aCard in SelectedCards)
					aCard.transform.localScale = new Vector3 (1, 1, 1);
				SelectedCards = new List<GameObject> ();
			}
		}
	}

	public void StartGame() {
		foreach (Timer T in MainCanvas.GetComponentsInChildren<Timer>())
			T.StartTimer();
		foreach (GameObject aCard in SelectedCards)
			aCard.transform.localScale = new Vector3 (1, 1, 1);
		SelectedCards = new List<GameObject> ();
		DrawNewCards ();
		MainCanvas.GetComponentInChildren<StartButton> ().enabled = false;
	}

	public void DrawNewCards() {
		Deck.AddRange (Play);
		Play = new List<GameObject> ();
		Shuffle<GameObject> (Deck);
		Play = Deck.GetRange (0, PlaySize);
		Deck.RemoveRange (0, PlaySize);
		ArrangePlay ();
		Debug.Log (FindNumberOfSets ());
	}

	public void RemoveAndReplaceSelected() {
		foreach (GameObject C in SelectedCards) {
			Sets.Add (C);
			if (Deck.Count > 0) {
				Play [Play.IndexOf (C)] = Deck [0];
				Deck.RemoveAt (0);
			} else {
				Play.Remove (C);
			}
			C.GetComponent<Card> ().TargetPos = new Vector3 (10, 10, 0);
		}
		SelectedCards = new List<GameObject> ();

		//Play.AddRange(Deck.GetRange (0, 3));
		//Deck.RemoveRange (0, 3);

		ArrangePlay ();

		if (Deck.Count == 0) {
			if (FindNumberOfSets () == 0) {
				MultiplayerGameFinished ();
			}
		}
	}

	public void MultiplayerGameFinished () {
		int HighestScore = 0;
		int Winner = 0;
		for (int i = 0; i < 4; ++i) {
			if (PlayerScores [i] > HighestScore) {
				HighestScore = PlayerScores [i];
				Winner = i;
			}
		}

		for (int i = 0; i < 4; ++i) {
			if (i == Winner)
				MainCanvas.GetComponentsInChildren<Text> () [i].text = "Well done. You've accomplished so much.";
			else
				MainCanvas.GetComponentsInChildren<Text> () [i].text = "At least you're not a nerd.";
		}

		foreach (Timer T in MainCanvas.GetComponentsInChildren<Timer>())
			T.StopTimer ();


	}

	public void ArrangePlay () {
		float Size = Mathf.Ceil (Mathf.Sqrt (Play.Count));
		Vector2 Offset = new Vector2(ScreenCenter.x - Size + 1, ScreenCenter.y - Size/2 + 0.5f);
		Instantiate (new GameObject (), Offset, new Quaternion());
		int i = 0;
		foreach (GameObject C in Play) {
			C.GetComponent<Card>().TargetPos = new Vector3 (Offset.x + Mathf.Floor (i / Size) * 2, Offset.y + (i % Size) * 1, 1);
			++i;
		}
		foreach (GameObject C in Deck)
			C.GetComponent<Card> ().TargetPos = new Vector3 (-13, -4, 1);
	}

	public void PressedButton(int Player) {
		if (CurrentTurn == -1) {
			CurrentTurn = Player - 1;
			int i = 0;
			foreach (Button B in MainCanvas.GetComponentsInChildren<Button>()) {
				i += 1;
				if (Player != i) {
					B.interactable = false;
					//ColorBlock CB = new ColorBlock ();
					//CB.normalColor = Color.red;
					//B.colors = CB;
				}
			}
		}
	}

	public void TurnEnded (int Player, bool TimedOut) {
		Timer = 0;
		CurrentTurn = -1;
		int i = 0;
		foreach (Button B in MainCanvas.GetComponentsInChildren<Button>()) {
			i += 1;
			if (Player == i) {
				if (TimedOut)
					B.interactable = false;
			} else
				B.interactable = true;
		}
	}

	public void Clue() {
		//Deselect
		foreach (GameObject aCard in SelectedCards)
			aCard.transform.localScale = new Vector3 (1, 1, 1);
		SelectedCards = new List<GameObject> ();

		foreach (GameObject A in Play) {
			foreach (GameObject B in Play) {
				if (B != A) {
					foreach (GameObject C in Play) {
						if (C != A && C != B) {
							if (CheckSet (A.GetComponent<Card> (), B.GetComponent<Card> (), C.GetComponent<Card> ())) {
								A.transform.localScale = new Vector3 (1.05f, 1.05f, 1);
								SelectedCards.Add (A);
								return;
							}
						}
					}
				}
			}
		}
	}

	public int FindNumberOfSets() {
		int Total = 0;
		//Rechecks a lot of sets, change to not check previous items in the list.
		foreach (GameObject A in Play) {
			foreach (GameObject B in Play) {
				if (B != A) {
					foreach (GameObject C in Play) {
						if (C != A && C != B) {
							if (CheckSet (A.GetComponent<Card> (), B.GetComponent<Card> (), C.GetComponent<Card> ())) {
								Total += 1;
							}
						}
					}
				}
			}
		}
		return Total / 6;
	}

	public bool CheckSet(Card One, Card Two, Card Three) {
		if ((One.Colour == Two.Colour && One.Colour == Three.Colour) ||
			(One.Colour != Two.Colour && One.Colour != Three.Colour && Two.Colour != Three.Colour))
		{
			if ((One.Shade == Two.Shade && One.Shade == Three.Shade) ||
				(One.Shade != Two.Shade && One.Shade != Three.Shade && Two.Shade != Three.Shade))
			{
				if ((One.Shape == Two.Shape && One.Shape == Three.Shape) ||
					(One.Shape != Two.Shape && One.Shape != Three.Shape && Two.Shape != Three.Shape))
				{
					if ((One.Number == Two.Number && One.Number == Three.Number) ||
					    (One.Number != Two.Number && One.Number != Three.Number && Two.Number != Three.Number)) {
						//Debug.Log ("" + One.Number + One.Colour + One.Shade + One.Shape + "'s + " + Two.Number + Two.Colour + Two.Shade + Two.Shape + "'s + ");
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void Shuffle<T>(List<T> list) {
		int n = list.Count;
		while (n > 1) {
			--n;
			int k = Random.Range (0, list.Count);
			T Value = list [k];
			list [k] = list [n];
			list [n] = Value;
		}
	}
}