using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CluedoGame : MonoBehaviour {

	//References to UI Objects:
	public Text Turn;
	public Button Submit;
	public Button Undo;
	public Button NoGuess;
	public Button Shown;
	public Button NotShown;
	public GuessCard WeaponCard;
	public GuessCard RoomCard;
	public GuessCard SuspectCard;
	public GameObject ScrollView;

	public TemporaryMessage SolutionGrid;

	//Controller Variables
	public CPhases Phase = CPhases.Names;
	public string ListToUndo = "Already used undo";
	public int MultiCounter = 0;

	//---Game Data---
	//All Cards:
	public int NumWeapons = 6;
	public int NumRooms = 9;
	public int NumSuspects = 6;

	public int MinimumPlayers = 3;

	public List<string> Weapons = new List<string> ();
	public List<string> Rooms = new List<string> ();
	public List<string> Suspects = new List<string> ();

	public List<string> All = new List<string> ();

	//Card Locations:
	public List<string> Unknown = new List<string> ();
	public List<string> Known = new List<string> (); //Duplicate data, but should reduce calculation time.
	public string[] Solution = new string[3];

	//Players (Contain card lists):
	//Player 0 is the user/computers player.
	public List<Player> Players = new List<Player> ();
	public int CurrentPlayer = 0;
	public int ShowingPlayer = 0;

	//Guesses:
	public Guess NewGuess;
	public List<Guess> UnSolvedGuesses = new List<Guess>();
	public List<CompletedGuess> SolvedGuesses = new List<CompletedGuess> ();


	// Use this for initialization
	void Start () {

		if (Weapons.Count < NumWeapons) {
			Weapons = new List<string> ();
			Weapons.Add ("Dagger");
			Weapons.Add ("Candlestick");
			Weapons.Add ("Revolver");
			Weapons.Add ("Rope");
			Weapons.Add ("Lead Pipe");
			Weapons.Add ("Wrench");
			WeaponCard.Options = Weapons;
			WeaponCard.Text = ""; //"Press Arrows";
		}

		if (Rooms.Count < NumRooms) {
			Rooms = new List<string> ();
			Rooms.Add 	("Hall");
			Rooms.Add 	("Lounge");
			Rooms.Add 	("Dining Room");
			Rooms.Add 	("Kitchen");
			Rooms.Add 	("Ballroom");
			Rooms.Add 	("Conservatory");
			Rooms.Add 	("Billiard Room");
			Rooms.Add 	("Library");
			Rooms.Add 	("Study");
			RoomCard.Options = Rooms;
			RoomCard.Text = ""; //"Press Arrows";
		}

		if (Suspects.Count < NumSuspects) {
			Suspects = new List<string> ();
			Suspects.Add ("Colonel Mustard");
			Suspects.Add ("Professor Plum");
			Suspects.Add ("Reverend Green");
			Suspects.Add ("Mrs Peacock");
			Suspects.Add ("Miss Scarlett");
			Suspects.Add ("Mrs White");
			SuspectCard.Options = Suspects;
			SuspectCard.Text = ""; //"Press Arrows";
		}

		All.AddRange (Weapons);
		All.AddRange (Rooms);
		All.AddRange (Suspects);
		Unknown.AddRange(All);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void AskForFirstPlayer () {
		string PlayerNames = "Who is going first? ";
		PlayerNames += "\n";
		foreach (Player P in Players) {
			PlayerNames += P.Name + ", ";
		}
		//ADD NEW LINE HERE IF POSSIBLE.
		Turn.text = PlayerNames;
	}

	public void StartGuessRound () {
		if (Phase == CPhases.Guess) {
			Submit.interactable = false;
			NewGuess = new Guess (WeaponCard.Current, RoomCard.Current, SuspectCard.Current, CurrentPlayer);
			Phase = CPhases.ResultsOfGuess;

			ShowingPlayer = (CurrentPlayer + 1) % Players.Count;
			if (CurrentPlayer == 0) {
				EnableAllCards ();
				Turn.text = "If " + Players[ShowingPlayer].Name + " showed a card select it, else press 'No'.";
			} else {
				Shown.interactable = true;
				Turn.text = "Did " + Players[ShowingPlayer].Name + " show a card?";
			}
			NotShown.interactable = true;
		}
	}

	public void SkipGuessRound () {
		CurrentPlayer = (CurrentPlayer + 1) % Players.Count;
		Turn.text = "Enter " + Players [CurrentPlayer].Name + "'s guess then press Submit, or press No Guess";
		if (CurrentPlayer == 0)
			Turn.text = "Enter your guess then press Submit, or press No Guess";
	}

	public void UndoPressed () {

	}

	public void ShownPressed () {
		NewGuess.ShownBy = ShowingPlayer;
		EndShowing ();
	}

	public void NotShownPressed () {
		Players [ShowingPlayer].NotHave.Add (NewGuess.Weapon);
		Players [ShowingPlayer].NotHave.Add (NewGuess.Room);
		Players [ShowingPlayer].NotHave.Add (NewGuess.Suspect);
		ShowingPlayer += 1;
		ShowingPlayer = ShowingPlayer % Players.Count;

		if (ShowingPlayer == CurrentPlayer) {
			EndShowing ();
		} else {
			if (CurrentPlayer == 0) {
				EnableAllCards ();
				Turn.text = "If " + Players[ShowingPlayer].Name + " showed a card select it, else press 'No'.";
			} else {
				Shown.interactable = true;
				Turn.text = "Did " + Players[ShowingPlayer].Name + " show a card?";
			}
		}
	}

	public void EndShowing () {
		//Disable buttons etc
		DisableAllCards();
		NotShown.interactable = false;
		Shown.interactable = false;
		NoGuess.interactable = false;

		UnSolvedGuesses.Add (NewGuess);

		Phase = CPhases.Calculate;
		Calculate ();
	}

	public void SubmitName (GameObject TextObject) {
		if (Phase == CPhases.Names) {
			if (TextObject.GetComponentsInChildren<Text> () [1].text == "") {
				if (Players.Count >= MinimumPlayers) {
					Phase = CPhases.FirstPlayer;
					AskForFirstPlayer ();
				} else {
					Turn.text = "You need at least " + MinimumPlayers + "\nplayers.";
				}
			} else {
				Players.Add (new Player (TextObject.GetComponentsInChildren<Text> () [1].text, MultiCounter));
				MultiCounter += 1;
				Debug.Log ("Added: " + TextObject.GetComponentsInChildren<Text> () [1].text + " as player " + (Players.Count));
				TextObject.GetComponent<InputField> ().text = ""; //Clear text in input field.
				if (Players.Count >= MinimumPlayers)
					Turn.text = "Enter player " + (Players.Count + 1) + "'s name:\nOr submit nothing if last player.";
				else
					Turn.text = "Enter player " + (Players.Count + 1) + "'s name:";
			}
		} else if (Phase == CPhases.FirstPlayer) {
			int i = 0;
			foreach (Player P in Players) {
				if (P.Name == TextObject.GetComponentsInChildren<Text> () [1].text) {
					CurrentPlayer = i;
				}
				i += 1;
			}
			Phase = CPhases.NumCards;
			//Calculate the number of cards for each player here.
			int RemainingCards = AllCardsCount - 3;
			int Counter = 0;
			for (int C = 0; C < Players.Count; ++C) {
				Players[((C + CurrentPlayer) % Players.Count)].NumberOfCards = Mathf.FloorToInt (RemainingCards / (Players.Count - Counter));
				RemainingCards -= Mathf.FloorToInt (RemainingCards / (Players.Count - Counter));
				Debug.Log (Players[((C + CurrentPlayer) % Players.Count)].NumberOfCards + " cards for " + Players[((C + CurrentPlayer) % Players.Count)].Name);
				Counter += 1;
			}
			/*
			foreach (Player P in Players) {
				P.NumberOfCards = Mathf.FloorToInt (RemainingCards / (Players.Count - Counter));
				RemainingCards -= Mathf.FloorToInt (RemainingCards / (Players.Count - Counter));
				Counter += 1;
				Debug.Log (P.NumberOfCards);
			}
			*/
			Destroy (TextObject);
			Turn.text = "Select the cards in your hand.\n(Click a card to submit it)";
			WeaponCard.Text = "Press Arrows";
			RoomCard.Text = "Press Arrows";
			SuspectCard.Text = "Press Arrows";
			WeaponCard.Current = "ZZZZZ";
			RoomCard.Current = "ZZZZZ";
			SuspectCard.Current = "ZZZZZ";
			WeaponCard.GetComponent<Button> ().interactable = true;
			RoomCard.GetComponent<Button> ().interactable = true;
			SuspectCard.GetComponent<Button> ().interactable = true;
			Phase = CPhases.StartingCards;
		}
	}

	private void CreatePlayerVisuals (int Player) {
		GameObject TM = Resources.Load<GameObject> ("Prefabs/Cluedo/CluedoMessage");
		//Player 1
		GameObject GO = Instantiate (TM, ScrollView.transform);
		GO.transform.localPosition = new Vector3 ();
		GO.transform.Translate(330 + (Player * 80), -80, 0);
		GO.transform.Rotate (0, 0, -20);
		GO.GetComponent<TemporaryMessage>().SetText(Players[Player].Name);
		GO.GetComponent<TemporaryMessage>().Delay = -1;
		//If a player has the card or not
		GO = Instantiate (TM, ScrollView.transform);
		GO.transform.localPosition = new Vector3 ();
		GO.transform.Translate(380 + (Player * 80), -300, 0);
		GO.GetComponent<Text> ().fontSize = 20;
		string LongString = "";
		foreach (string s in All) {
			if (Players[Player].DoesHave.Contains(s))
				LongString += " X \n";
			else if (Players[Player].NotHave.Contains(s))
				LongString += "   \n";
			else 
				LongString += " ? \n";

		}
		GO.GetComponent<TemporaryMessage>().SetText(LongString);
		GO.GetComponent<TemporaryMessage>().Delay = -1;
		Players [Player].Visuals = GO.GetComponent<TemporaryMessage> ();
	}

	private void DisableAllCards () {
		WeaponCard.GetComponent<Button> ().interactable = false;
		RoomCard.GetComponent<Button> ().interactable = false;
		SuspectCard.GetComponent<Button> ().interactable = false;
	}

	private void EnableAllCards () {
		WeaponCard.GetComponent<Button> ().interactable = true;
		RoomCard.GetComponent<Button> ().interactable = true;
		SuspectCard.GetComponent<Button> ().interactable = true;
	}

	public void WeaponPressed () {
		if (WeaponCard.Current == "ZZZZZ")
			return;
		if (Phase == CPhases.StartingCards) {
			Debug.Log ("Weapon Card submited: " + WeaponCard.Current);
			if (!Players [0].DoesHave.Contains (WeaponCard.Current))
				Players [0].DoesHave.Add (WeaponCard.Current);
		}

		CardPressed (0);
	}

	public void RoomPressed () {
		if (RoomCard.Current == "ZZZZZ")
			return;
		if (Phase == CPhases.StartingCards) {
			Debug.Log ("Room Card submited: " + RoomCard.Current);
			if (!Players [0].DoesHave.Contains (RoomCard.Current))
				Players [0].DoesHave.Add (RoomCard.Current);
		}

		CardPressed (1);
	}

	public void SuspectPressed () {
		if (SuspectCard.Current == "ZZZZZ")
			return;
		if (Phase == CPhases.StartingCards) {
			Debug.Log ("Suspect Card submited: " + SuspectCard.Current);
			if (!Players [0].DoesHave.Contains (SuspectCard.Current))
				Players [0].DoesHave.Add (SuspectCard.Current);
		}
		
		CardPressed (2);
	}

	public void CardPressed (int Category) {
		if (Phase == CPhases.StartingCards) {
			if (Players [0].DoesHave.Count == Players [0].NumberOfCards) {
				DisableAllCards ();

				foreach (string s in All) {
					if (!Players [0].DoesHave.Contains (s))
						Players [0].NotHave.Add (s);
				}

				DiscoveredCardsToNotHave ();
				UpdateKnown ();

				GameObject TM = Resources.Load<GameObject> ("Prefabs/Cluedo/CluedoMessage");
				ScrollView.GetComponent<RectTransform> ().sizeDelta += new Vector2 (60 * Players.Count - 90, 0);
				//Cards
				GameObject GO = Instantiate (TM, ScrollView.transform);
				GO.transform.localPosition = new Vector3 ();
				GO.transform.Translate (210, -300, 0);
				GO.GetComponent<Text> ().fontSize = 20;
				string AllItems = "";
				foreach (string s in All)
					AllItems += s + " - \n";
				GO.GetComponent<TemporaryMessage> ().SetText (AllItems);
				GO.GetComponent<TemporaryMessage> ().Delay = -1;
				for (int P = 0; P < Players.Count; ++P)
					CreatePlayerVisuals (P);
				//Solution
				GO = Instantiate (TM, ScrollView.transform);
				GO.transform.localPosition = new Vector3 ();
				GO.transform.Translate (350 + (Players.Count * 80), -80, 0);
				GO.transform.Rotate (0, 0, -20);
				GO.GetComponent<TemporaryMessage> ().SetText ("Solution");
				GO.GetComponent<TemporaryMessage> ().Delay = -1;
				//Solution Grid
				GO = Instantiate (TM, ScrollView.transform);
				GO.transform.localPosition = new Vector3 ();
				GO.transform.Translate (380 + (Players.Count * 80), -300, 0);
				GO.GetComponent<Text> ().fontSize = 20;
				//Generate grid for Solution
				string LongString = "";
				foreach (string s in All) {
					if (Solution [0] == s || Solution [1] == s || Solution [2] == s)
						LongString += " X \n";
					else if (Known.Contains (s)) {
						LongString += " <> \n"; //Know where it is
					} else {
						LongString += "   \n"; //DON'T know
					}

				}
				GO.GetComponent<TemporaryMessage> ().SetText (LongString);
				GO.GetComponent<TemporaryMessage> ().Delay = -1;
				SolutionGrid = GO.GetComponent<TemporaryMessage> ();
				Phase = CPhases.Guess;
				Turn.text = "Enter " + Players [CurrentPlayer].Name + "'s guess then press Submit, or press No Guess";
				if (CurrentPlayer == 0)
					Turn.text = "Enter your guess then press Submit, or press No Guess";
				Submit.interactable = true;
				NoGuess.interactable = true;
				//EnableAllCards ();
			}
		} else if (Phase == CPhases.ResultsOfGuess) {
			string DiscoveredCard = NewGuess.Cards [Category];
			Players [ShowingPlayer].DoesHave.Add (DiscoveredCard);
			Unknown.Remove (DiscoveredCard);
			Known.Add (DiscoveredCard);
			DiscoveredCardsToNotHave ();
			EndShowing ();
		}
	}

	/// <summary>
	/// Make sure that all of the players 'Not Have' lists contain all the cards that the other players DO have.
	/// </summary>
	private void DiscoveredCardsToNotHave () {
		foreach (Player P in Players) {
			foreach (Player OP in Players) {
				if (OP == P)
					continue;
				foreach (string s in P.DoesHave) {
					if (!OP.NotHave.Contains (s))
						OP.NotHave.Add (s);
				}
			}
		}
	}

	private void UpdateKnown () {
		List<string> ToMove = new List<string> ();
		foreach (string s in Unknown) {
			foreach (Player P in Players) {
				if (P.DoesHave.Contains (s))
					ToMove.Add (s);
			}
		}
		Known.AddRange (ToMove);
		foreach (string s in ToMove)
			Unknown.Remove (s);
	}

	private void AddDiscoveredToNotHaves (string Card, int DoesHaveIndex) {
		foreach (Player P in Players) {
			if (P.Index != DoesHaveIndex)
				P.NotHave.Add (Card);
		}
	}

	public void Calculate () {
		//Scenarios:
		//		> If we know ALL the cards a player DOESN'T have, add the rest to what they DO have
		//		> A player DOESN'T have 2 of the cards in a guess that they answered, so they must have the third
		//		> A player has at least 1 of the cards in a guess, so we remove that guess, as no extra information can be gained with it
		//		> If a player only has one slot left for a card they DO have, if 2 guesses (they answered) only have 1 possible answer in common, that is in the solution
		//		> If a player has a card, remove it from UNKNOWN
		//		> If all players DON'T have a card, add it to SOLUTION
		//		> If there is only 1 UNKNOWN left for a category (and it's not in the solution) add it to the solution
		//- Check any guesses that no-one answered, if we know all the cards of the asker, then add cards they don't own to the solution (and highlight these cards on the grid either way)
		//- If a player is still asking about a card we assume they have seen, assume that the player who showed them had 2 in that guess, and display (but do not apply) the other possibility


		foreach (Player P in Players) {
			//If we know all of this players cards, add the rest to 'Doesn't Have'.
			if (P.DoesHave.Count == P.NumberOfCards) {
				if (P.NotHave.Count < AllCardsCount - P.NumberOfCards) {
					foreach (string s in All) {
						if (!P.NotHave.Contains (s) && !P.DoesHave.Contains(s))
							P.NotHave.Add (s);
					}
				}
			}

			//If we know ALL the cards a player DOESN'T have, add the rest to what they DO have
			if (P.NotHave.Count == AllCardsCount - P.NumberOfCards) {
				if (P.DoesHave.Count < P.NumberOfCards) {
					foreach (string s in All) {
						if (!P.NotHave.Contains (s) && !P.DoesHave.Contains (s)) {
							P.DoesHave.Add (s);
							Unknown.Remove (s);
							Known.Add (s);
						}
					}
					DiscoveredCardsToNotHave ();
				}
			}			

			//Create a list of guesses that were answered by this player.
			List<Guess> RelevantGuesses = new List<Guess> ();
			foreach (Guess G in UnSolvedGuesses) {
				if (G.ShownBy == P.Index)
					RelevantGuesses.Add (G);
			}

			//Find guesses where the player could only have answered with one card.
			foreach (Guess G in RelevantGuesses) {
				//Don't have 2 of the cards in the guess
				if (P.CountNOT (G) == 2) {
					//Add the remaining card to that players 'Does Have' list, and the other players 'Not Have' lists, and update Known and Unknown
					string DiscoveredCard = P.AddOwnedCard (G);
					AddDiscoveredToNotHaves (DiscoveredCard, P.Index);
					//Unknown.Remove (DiscoveredCard);
					//Known.Add (DiscoveredCard);
				}
			}

			//Remove obsolete guesses
			foreach (Guess G in RelevantGuesses) {
				//If no obvious information can be gained from this guess (we know one of the owned cards)
				if (P.CountHAS (G) > 0) {
					//Remove it from UNsolved, add it to the cards the other player has seen, and add it to solved (AKA history)
					UnSolvedGuesses.Remove (G);
					int Card = P.FindOwnedCard (G);
					Players [G.ShownTo].HasSeenAll.Add (G.Cards [Card]);
					SolvedGuesses.Add(new CompletedGuess(G, Card));
				}
			}

			//NEW METHOD FOR \/:
			//Create a list of all the cards they COULD have.
			//For each guess, if it DOESN'T contain one of those cards, the must not have it, so REMOVE it from the list.
			//If the list contains only ONE card at the end, that must be the card they have.
			//If 0, show an error.
			List<string> CouldHave = new List<string> ();
			//Remove when debugging is finished.
			foreach (string u in Unknown) {
				if (P.NotHave.Contains (u) && P.DoesHave.Contains (u)) {
					Debug.LogError ("Unknown is not accurate");
				}
			}

			CouldHave.AddRange (Unknown);
			foreach (Guess G in UnSolvedGuesses) {
				List<string> ToRemove = new List<string> ();
				foreach (string s in CouldHave) {
					if (!G.Contains (s)) {
						ToRemove.Add (s);
					}
				}
				foreach (string s in ToRemove)
					CouldHave.Remove (s);
			}

			if (CouldHave.Count == P.NumberOfCards - P.DoesHave.Count) {
				P.DoesHave.AddRange (CouldHave);
				Known.AddRange (CouldHave);
				foreach (string s in CouldHave) {
					Unknown.Remove (s);
				}
				DiscoveredCardsToNotHave ();
			}

			//If a player has only one card that is unknown to us: if any two guesses have only one (possible) card in common, add it to DoesHave
			//Is inefficient, checks each guess combination twice (G2 should start at G1's index + 1).

			/*
			if (P.DoesHave.Count == P.NumberOfCards - 1) {
				foreach (Guess G1 in UnSolvedGuesses) {
					bool Found = false;
					foreach (Guess G2 in UnSolvedGuesses) {
						if (G1 == G2)
							continue;
						int Same = 0;
						string PossibleCard = new string ();
						for (int i = 0; i < 3; ++i) {
							if (G1.Cards [i] == G2.Cards [i]) {
								if (!P.NotHave.Contains (G1.Cards [i])) {
									Same += 1;
									PossibleCard = G1.Cards [i];
								}
							}
						}
						if (Same == 1) {
							P.DoesHave.Add (PossibleCard);
							Unknown.Remove (PossibleCard);
							DiscoveredCardsToNotHave ();
							Found = true;
							break;
						}
					}
					if (Found)
						break;
				}
			}
			//[Could do /\ for 2 cards - eg three guesses only have one in common - but it is exponentially more intensive]
			*/
		}
		//Check if ALL players don't have a card. Add to Solution if they don't.
		List<string> ToRemove2 = new List<string> ();
		foreach (string s in Unknown) {
			bool OwnedByNoOne = true;
			foreach (Player P in Players) {
				if (!P.NotHave.Contains (s)) {
					OwnedByNoOne = false;
					break;
				}
			}
			if (OwnedByNoOne) {
				ToRemove2.Add (s);
				Solution [CategoryIndex (s)] = s;
			}
		}
		Known.AddRange (ToRemove2);
		foreach (string s in ToRemove2)
			Unknown.Remove (s);

		//For each category, if the solution hasn't been found, and there is only 1 unknown left in that category, add it to the solution.
		for (int i = 0; i < 3; ++i) {
			if (Solution [i] == "") {
				int NumberOfUnknowns = 0;
				string PossibleSolution = "";
				foreach (string s in Unknown) {
					if (CategoryIndex (s) == i) {
						NumberOfUnknowns += 1;
						if (NumberOfUnknowns > 1)
							break;
						else
							PossibleSolution = s;
					}
				}
				if (NumberOfUnknowns == 1)
					Solution [i] = PossibleSolution;
			}
		}

		Phase = CPhases.Guess;
		CurrentPlayer = (CurrentPlayer + 1) % Players.Count;
		Turn.text = "Enter " + Players [CurrentPlayer].Name + "'s guess then press Submit, or press No Guess";
		if (CurrentPlayer == 0)
			Turn.text = "Enter your guess then press Submit, or press No Guess";
		Submit.interactable = true;
		NoGuess.interactable = true;

		UpdateGrid ();
	}

	public void UpdateGrid () {
		string LongString = "";
		//PLAYERS
		foreach (Player P in Players) {
			LongString = "";
			foreach (string s in All) {
				if (P.DoesHave.Contains (s))
					LongString += " X \n";
				else if (P.NotHave.Contains (s))
					LongString += "   \n";
				else
					LongString += " ? \n";

			}
			Players [P.Index].Visuals.SetText (LongString);
		}

		//SOLUTION
		LongString = "";
		foreach (string s in All) {
			if (Solution [0] == s || Solution [1] == s || Solution [2] == s)
				LongString += " X \n";
			else if (Known.Contains (s)) {
				LongString += " <> \n"; //Know where it is
			} else {
				LongString += "   \n"; //DON'T know
			}
		}
		SolutionGrid.SetText (LongString);

	}

	private int CategoryIndex (string s) {
		//There are more rooms than others, so room is the default to make the program quicker.
		if (Weapons.Contains (s))
			return 0;
		else if (Suspects.Contains (s))
			return 2;
		else
			return 1;
	}

	private int AllCardsCount {
		get { return (NumWeapons + NumRooms + NumSuspects); }
	}

	//Button functions:
	public void Log (Text TextObject) {
		Debug.Log ("Button Pressed: " + TextObject.text);
	}

	public void PressSubmit () {
		//TheButton.GetComponent<Button> ().interactable = false;
	}

}

public class Player {
	public string Name;
	public int NumberOfCards;
	public int Index = 0;
	public TemporaryMessage Visuals;
	/// <summary>
	/// Cards we have been shown, or that we can be sure are in their hand.
	/// </summary>
	public List<string> DoesHave = new List<string> ();
	/// <summary>
	/// Cards that the player didn't have when a guess came to them.
	/// </summary>
	public List<string> NotHave = new List<string> ();
	/// <summary>
	/// Cards that we have shown to this player, so that we can show them again if possible.
	/// </summary>
	public List<string> HasSeenUs = new List<string> ();
	/// <summary>
	/// Cards that we can confirm this player has seen, so we can estimate how close to the solution they are.
	/// </summary>
	public List<string> HasSeenAll = new List<string> ();

	public Player (string PlayerName, int Number) {
		Name = PlayerName;
		Index = Number;
	}

	/// <summary>
	/// Adds the (only possible) owned card to this players 'Does Have' list.
	/// </summary>
	/// <returns>The card added to 'Does Have'.</returns>
	public string AddOwnedCard (Guess G) {
		string HaveCard = "";
		for (int i = 0; i < 3; ++i) {
			if (!NotHave.Contains (G.Cards [i])) {
				HaveCard = G.Cards [i];
				DoesHave.Add (HaveCard);
				if (NotHave.Contains (HaveCard))
					Debug.Log ("Broken: Contains a card that is in NotHave");
			}
		}
		return HaveCard;
	}

	public int FindOwnedCard (Guess G) {
		for (int i = 0; i < 2; ++i) {
			if (DoesHave.Contains (G.Cards [i])) {
				return i;
			}
		}
		return 2;
	}

	public int CountHAS (Guess G) {
		int NumHas = 0;
		foreach (string s in DoesHave) {
			if (G.Cards[0] == s || G.Cards[1] == s || G.Cards[0] == s)
				NumHas += 1;
		}
		return NumHas;
	}

	public int CountNOT (Guess G) {
		int NumNot = 0;
		foreach (string s in NotHave) {
			if (G.Cards[0] == s || G.Cards[1] == s || G.Cards[0] == s)
				NumNot += 1;
		}
		return NumNot;
	}
}

public class Guess {
	/// <summary>
	/// [0]=Weapon, [1]=Room, [2]=Suspect.
	/// </summary>
	public string[] Cards = new string[3];
	/// <summary>
	/// The player who showed a card when this guess was made (-1 if none was shown).
	/// </summary>
	public int ShownBy = -1;
	/// <summary>
	/// The player who made this guess, and was shown the card.
	/// </summary>
	public int ShownTo;

	public Guess (string Weapon, string Room, string Suspect, int AskedBy) {
		Cards [0] = Weapon;
		Cards [1] = Room;
		Cards [2] = Suspect;
		ShownTo = AskedBy;
	}

	public Guess (Guess OldGuess) {
		Cards [0] = OldGuess.Cards [0];
		Cards [1] = OldGuess.Cards [1];
		Cards [2] = OldGuess.Cards [2];
		ShownTo = OldGuess.ShownTo;
		ShownBy = OldGuess.ShownBy;
	}

	public string Weapon {
		get { return Cards [0]; }
	}

	public string Room {
		get { return Cards [1]; }
	}

	public string Suspect {
		get { return Cards [2]; }
	}
	
	public bool Contains (string Card) {
		if (Cards [0] == Card || Cards [1] == Card || Cards [2] == Card)
			return true;
		else
			return false;
	}
}

public class CompletedGuess : Guess {
	public int CardThatWasShown;

	public CompletedGuess (Guess OldGuess, int ShownCard) : base(OldGuess) {
		CardThatWasShown = ShownCard;
	}

	public string ShownCard {
		get { return Cards [CardThatWasShown]; }
	}
}

public enum CardTypes {
	Weapon,
	Room,
	Suspects
}

public enum CPhases {
	Names,
	FirstPlayer,
	NumCards,
	StartingCards,
	Guess,
	ResultsOfGuess,
	Calculate
}