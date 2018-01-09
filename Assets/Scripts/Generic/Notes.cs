using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notes : MonoBehaviour {

	public void AddNote (GameObject TextObject) {
		if (TextObject.GetComponentsInChildren<Text> () [1].text != "") {
			GetComponent<Text> ().text = "- " + TextObject.GetComponentsInChildren<Text> () [1].text + "\n" + GetComponent<Text> ().text;
			TextObject.GetComponent<InputField> ().text = ""; //Clear text in input field.
		}
	}
}
