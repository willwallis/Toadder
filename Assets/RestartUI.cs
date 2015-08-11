using UnityEngine;
using System.Collections;

public class RestartUI : MonoBehaviour {

	public GameObject persistData;

	void OnTriggerEnter (Collider other) {
		persistData.GetComponent<StoreData> ().restartGame ();
	}
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
