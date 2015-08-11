using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreData : MonoBehaviour {

	public int lives = 0;
	public int startLives = 3;
	public Text liveText;
	public int score = 0;
	public int stepScore = 10;
	public int homeScore = 50;
	public int allHomeScore = 1000;
	public int secondsRemainingScore = 10;
	public Text scoreText;
	public int homes = 0;
	public int homesFound = 0;
	public int homesMax = 5;
	public Text winText;
	public GameObject restartObject;
	public GameObject spawnBlocker;

	private int timeLeft;
	private GameObject startDataObj;

	private void Awake() {
	}


	private void Start() {
		startDataObj = GameObject.FindWithTag("StartData");
        lives = startDataObj.GetComponent<HoldData>().lives;
		//		lives = startLives;
		setLivesUI ();
	}
			
	// LIVES
	public void loseLife() {
		// Subtract lives from lives
		lives = lives - 1;
		setLivesUI ();
		if (lives == 0) {
			gameOver("you ran out of lives");
		}
	}

	private void setLivesUI() {
		liveText.text = "Lives: " + lives.ToString("D4");
	}

	public void gameOver(string reasonMessage) {
		// Pause time and get value
		timeLeft = GetComponent<CountDownTimer> ().pauseTime ();

		// Win Text Game Over
		winText.text = "GAME OVER\n" + reasonMessage; 

		// Remove Cars
		removeCars();

		// Deactivate Homes
		GameObject[] Homes = GameObject.FindGameObjectsWithTag("ToadHome");
		for(int i = 0; i < Homes.Length; i++)
		{
			if (Homes[i].GetComponent<ToadCollider>().active == true)
				Homes[i].GetComponent<ToadCollider>().setNormal ();
			}

		// Call restart prompt delay
		StartCoroutine(restartCube());
	}

	// Prompts for a restart
	IEnumerator restartCube() {
		yield return new WaitForSeconds (3.0f);
		winText.text = "GAME OVER" +
			"\nto play again move to blue cube"; 
		restartObject.SetActive (true);
	}

	// Restarts the Games
	public void restartGame() {
		winText.text = "Game Restarted";
		// Reset Scores
		setScore (0);
		// Reset Lives
		lives = startLives;
		setLivesUI ();
		// Reset Time
		GetComponent<CountDownTimer> ().resetTime ();
		// Reset Homes
		GameObject[] Homes = GameObject.FindGameObjectsWithTag("ToadHome");
		for(int i = 0; i < Homes.Length; i++)
		{
				Homes[i].GetComponent<ToadCollider>().setNormal();
		}
		// Set Home Active (true will pick opposite start side)
		Homes [0].GetComponent<ToadCollider> ().nextActive (true);
		// Reset Found Counter
		homesFound = 0;
		// Start Car Spawners
		switchSpawners (true);
		// Stop Cube (on delay)
		StartCoroutine(stopCube());
	}

	// Removes prompt for a restart
	IEnumerator stopCube() {
		yield return new WaitForSeconds (1.0f);
		winText.text = ""; 
		restartObject.SetActive (false);
	}

	//SCORES
	// Setting and adding to score
	private void setScore(int newScore) {
		score = newScore;
		setScoreBoard ();
	}

	private void addScore(int addScore) {
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
		addScore (homeScore);
		if (homesFound == homesMax) {
			forTheWin();
		}
	}
	
	private void forTheWin () {
		// Pause time and get value
		timeLeft = GetComponent<CountDownTimer> ().pauseTime ();
		int timeBonus = timeLeft * secondsRemainingScore;

		// Win Text All Homes Saved + Time Bonus
		winText.text = "All homes saved!" + "\nTime Bonus: " 
			+ timeLeft.ToString().TrimStart('0') + "s x " + secondsRemainingScore 
			+ " = " + timeBonus;

		// Update score with time bonues
		addScore (timeBonus);

		// Update score for savings all homes
		addScore (allHomeScore);

		// Remove Cars
		removeCars ();

		// Start the fireworks
		GameObject[] Fireworks = GameObject.FindGameObjectsWithTag("Fireworks");
		for(int i = 0; i < Fireworks.Length; i++){
			Fireworks [i].GetComponent<ParticleSystem> ().Play ();
			Fireworks [i].GetComponent<AudioSource> ().Play ();

		}

	}	

	private void removeCars() {
		// Turn off spawners
		switchSpawners (false);
		// Turn off vehicles
		GameObject[] Vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
		for(int i = 0; i < Vehicles.Length; i++){
			Vehicles[i].GetComponent<TrafficSystemVehicle>().Kill();
		}
	}

	// Switches spawners on or off based on input boolean.
	private void switchSpawners(bool input) {
		spawnBlocker.SetActive (!input);
	}

}
