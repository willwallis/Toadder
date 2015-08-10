using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreData : MonoBehaviour {

	public int score = 0;
	public int stepScore = 10;
	public int homeScore = 50;
	public int allHomeScore = 1000;
	public int secondsRemainingScore = 10;
	public Text scoreText;
	public int homes = 0;
	public int homesFound = 0;
	public int homesMax = 3;
	public Text winText;

	//SCORES
	// Setting and adding to score
	public void setScore(int newScore) {
		score = newScore;
		setScoreBoard ();
	}

	public void addScore(int addScore) {
		score = score + addScore;
		setScoreBoard ();
	}

	// Update Scoreboard
	private void setScoreBoard() {
		scoreText.text = "Score: " + score.ToString("D4");
	}

	// HOMES
	public void homeFound () {
		homesFound = homesFound + 1;
		if (homesFound == homesMax) {
			addScore (allHomeScore);
			winText.text = "All safe!";

		}
	}
}
