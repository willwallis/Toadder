using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToadCollider : MonoBehaviour {

	public Text winMessage;
	public Material winMaterial;
	public bool found = false;
	public GameObject persistData;
	public string winText = "All safe!"; 

	// Determine if Toad enters Home
	void OnTriggerEnter (Collider other) {
		if (!found) {
			winMessage.text = "You're Home!";
			Renderer rend = GetComponent<Renderer> ();
			rend.material = winMaterial;
			persistData.GetComponent<StoreData> ().addScore (500);
			persistData.GetComponent<StoreData> ().homeFound ();
			found = true;
		} else if (!System.String.Equals (winMessage.text, winText)) {
			winMessage.text = "Try another";
		}
	}

	void Start() {
	}



}
