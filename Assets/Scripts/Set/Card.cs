using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

	public int MySeed;
	public Colours Colour;
	public Shades Shade;
	public Shapes Shape;
	public Numbers Number;

	public Vector3 TargetPos = new Vector3();
	public float Speed = 0.3f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards (transform.position, TargetPos, Speed);
		/*
		if (Input.GetKeyDown (KeyCode.Space)) {
			SetDesign (Random.Range (0, 80));
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			SetDesign (MySeed);
		}
		if (Input.GetKeyDown (KeyCode.Tab)) {
			SetDesign (MySeed + 1);
		}
		*/
	}

	public void SetDesign (int Seed) {
		//Colour is Seed/3
		//Shade is Colour/3
		//Shape is Shade/3
		//Number is Shape/3

		MySeed = Seed;

		//Choose Colour.
		int ImportantPart = Seed;
		int ColourNum = Mathf.FloorToInt (ImportantPart / 27f);

		if (ColourNum == 0)
			Colour = Colours.Red;
		else if (ColourNum == 1)
			Colour = Colours.Green;
		else
			Colour = Colours.Purple;
		
		//Choose Shade.
		ImportantPart = ImportantPart - (ColourNum * 27);
		int ShadeNum = Mathf.FloorToInt (ImportantPart / 9);

		if (ShadeNum == 0)
			Shade = Shades.Empty;
		else if (ShadeNum == 1)
			Shade = Shades.Half;
		else
			Shade = Shades.Filled;

		//Choose Shape.
		ImportantPart = ImportantPart - (ShadeNum * 9);
		int ShapeNum = Mathf.FloorToInt (ImportantPart / 3);

		if (ShapeNum == 0)
			Shape = Shapes.Diamond;
		else if (ShapeNum == 1)
			Shape = Shapes.Capsule;
		else
			Shape = Shapes.Squiggle;

		//Choose Number.
		ImportantPart = ImportantPart - (ShapeNum * 3);		
		int NumberNum = ImportantPart;							

		if (NumberNum == 0)
			Number = Numbers.One;
		else if (NumberNum == 1)
			Number = Numbers.Two;
		else
			Number = Numbers.Three;

		ChooseSprite (ShapeNum, ShadeNum);

		//Debug.Log ("Seed: " + Seed + " >>> Design: " + Colour + ", " + Shade + ", " + Shape + ", " + Number);
	}

	public void ChooseSprite (int ShapeNumber, int ShadeNumber) {

		//int SpriteNumber = ShapeNumber * 3 + ShadeNumber;

		//Empty -> Half -> Filled
		//Diamond -> Capsule -> Squiggle

		//  D | C | S
		//E 0   1   2
		//H 3   4   5
		//F 6   7   8
		string SpritePath = "Art/Shapes/Shape_" + (ShadeNumber * 3 + ShapeNumber);

		Sprite ChosenSprite = Resources.Load<Sprite> (SpritePath);

		GameObject EmptySprite = Resources.Load<GameObject>("Prefabs/EmptySprite");
		SpriteRenderer SR;
		if (Number == Numbers.One || Number == Numbers.Three) {
			SR = Instantiate (EmptySprite, transform).GetComponent<SpriteRenderer>();
			SR.sprite = ChosenSprite;
			SR.color = ColourToRGB(Colour);
			SR.transform.localPosition = new Vector3(0, 0, -1);
			SR.transform.localScale = new Vector3(2, 2, 1);
			if (Number == Numbers.Three) {
				SR = Instantiate (EmptySprite, transform).GetComponent<SpriteRenderer>();
				SR.sprite = ChosenSprite;
				SR.color = ColourToRGB(Colour);
				SR.transform.localPosition = new Vector3(0.6f, 0, -1);
				SR.transform.localScale = new Vector3(2, 2, 1);
				SR = Instantiate (EmptySprite, transform).GetComponent<SpriteRenderer>();
				SR.sprite = ChosenSprite;
				SR.color = ColourToRGB(Colour);
				SR.transform.localPosition = new Vector3(-0.6f, 0, -1);
				SR.transform.localScale = new Vector3(2, 2, 1);
			}
		} else {
			SR = Instantiate (EmptySprite, transform).GetComponent<SpriteRenderer>();
			SR.sprite = ChosenSprite;
			SR.color = ColourToRGB(Colour);
			SR.transform.localPosition = new Vector3(0.3f, 0, -1);
			SR.transform.localScale = new Vector3(2, 2, 1);
			SR = Instantiate (EmptySprite, transform).GetComponent<SpriteRenderer>();
			SR.sprite = ChosenSprite;
			SR.color = ColourToRGB(Colour);
			SR.transform.localPosition = new Vector3(-0.3f, 0, -1);
			SR.transform.localScale = new Vector3(2, 2, 1);
		}
			
	}

	public void PrintCard() {
		Debug.Log (">>> Design: " + Colour + ", " + Shade + ", " + Shape + ", " + Number);
	}

	public static Color ColourToRGB(Colours C) {
		if (C == Colours.Red)
			return new Color (1, 0, 0);
		else if (C == Colours.Green)
			return new Color (0, 1, 0);
		else
			return new Color (0.55f, 0, 1);
	}
}

public enum Shapes {
	Diamond,		//Shapes_[0, 3, 6]
	Capsule,		//Shapes_[1, 4, 7]
	Squiggle		//Shapes_[2, 5, 8]
}

public enum Shades {
	Empty,			//Shapes_[0, 1, 2]
	Half,			//Shapes_[3, 4, 5]
	Filled			//Shapes_[6, 7, 8]
}

public enum Colours {
	Red,
	Green,
	Purple
}

public enum Numbers {
	One,
	Two,
	Three
}