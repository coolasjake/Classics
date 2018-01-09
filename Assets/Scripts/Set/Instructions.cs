using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.I)) {
			if (GetComponent<Image> ().color.a == 0)
				GetComponent<Image> ().color = new Color(1, 1, 1, 1);
			else
				GetComponent<Image> ().color = new Color(1, 1, 1, 0);
		}
	}
}
