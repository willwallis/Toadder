using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToadCollider : MonoBehaviour {

	public bool active = false;
	public bool found = false;
	public bool startSide = false;
	public Material normalMaterial;
	public Material activeMaterial;
	public Material winMaterial;
	public GameObject persistData;
	public Text winMessage;
	public string saveText = "Home saved!"; 

	void Start() {
		// Set material to active on start if flag set
		if (active) {
			setActive();
		}
	}


	// Determine if Toad enters Home
	void OnTriggerEnter (Collider other) {
		if (!found && active) {
			winMessage.text = saveText;
			setFound();
			persistData.GetComponent<StoreData> ().homeFound ();
			nextActive(startSide);
			// Plash win sound
			GetComponent<AudioSource>().Play();
		}
	}

	void OnTriggerExit (Collider other) {
		if (System.String.Equals (winMessage.text, saveText))
			winMessage.text = "";
	}

	public void setNormal() {
		Renderer rend = GetComponent<Renderer> ();
		rend.material = normalMaterial;
		active = false;
		found = false;
		transform.GetChild(0).gameObject.SetActive(false);
	}

	public void setActive() {
		Renderer rend = GetComponent<Renderer> ();
		rend.material = activeMaterial;
		active = true;
		found = false;
		transform.GetChild(0).gameObject.SetActive(true);
	}

	public void setFound() {
		Renderer rend = GetComponent<Renderer> ();
		rend.material = winMaterial;
		active = false;
		found = true;
		transform.GetChild(0).gameObject.SetActive(false);
	}

	public void nextActive(bool side) {
		GameObject[] Homes = GameObject.FindGameObjectsWithTag("ToadHome");
		for(int i = 0; i < Homes.Length; i++)
		{
			if (Homes[i].GetComponent<ToadCollider>().active == false && Homes[i].GetComponent<ToadCollider>().found == false && Homes[i].GetComponent<ToadCollider>().startSide != side) {
				Homes[i].GetComponent<ToadCollider>().setActive();
				break;
			}
		}
	}

}
