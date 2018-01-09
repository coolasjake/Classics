using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryMessage : MonoBehaviour {

	public float Delay = 10f;

	// Use this for initialization
	void Start () {
		if (Delay >= 0)
			Destroy (gameObject, Delay);
	}

	public void SetText(string Message) {
		GetComponent<Text>().text = Message;
	}
}
