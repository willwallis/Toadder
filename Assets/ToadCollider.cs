using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToadCollider : MonoBehaviour {

	public Text winMessage;

	// Determine if Toad enters Home
	void OnTriggerEnter (Collider other) {
		winMessage.text = "You're Home!";
	}

}
