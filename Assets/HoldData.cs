using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HoldData : MonoBehaviour {

	public int multiplier = 8;
	public int totalTime = 90;
	public int lives = 3;
	public Text multiplierText;
	public Text timeText;
	public Text livesText;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		updateUI ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startGame() {
		Application.LoadLevel(1);
	}

	public void increase(int whichOne) {
		switch (whichOne) {
		case 0:
			multiplier++;
			break;
		case 1:
			totalTime = totalTime + 5;
			break;
		case 2:
			lives++;
			break;
		}
		updateUI ();
	}

	public void decease(int whichOne) {
		switch (whichOne) {
		case 0:
			multiplier--;
			break;
		case 1:
			totalTime = totalTime - 5;
			break;
		case 2:
			lives--;
			break;
		}
		updateUI ();
	}

	private void updateUI (){
		multiplierText.text = multiplier.ToString();
		timeText.text = totalTime.ToString();
		livesText.text = lives.ToString();
	}


}
