using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDownTimer : MonoBehaviour {

	public Text CountDownText;
	public int TimeLimit;
	public bool CounterOn;

	private int StartTime;
	private int TimeOffset;
	private int CurrentTime;
	private int TimeLeft;
	private GameObject startDataObj;


	void Awake() {
		StartTime = Mathf.CeilToInt (Time.time);
	}

	private void Start() {
		startDataObj = GameObject.FindWithTag("StartData");
		TimeLimit = startDataObj.GetComponent<HoldData>().totalTime;
	}

	void OnGUI() {
		if (CounterOn) {
			CurrentTime = Mathf.CeilToInt (Time.time);
			TimeLeft = TimeLimit - CurrentTime + StartTime;
		}
		CountDownText.text = "Time: " + TimeLeft.ToString ("D4");
		if (TimeLeft <= 0)
			GetComponent<StoreData> ().gameOver ("you ran out of time");
	}

	// Pauses timer and returns current value
	public int pauseTime() {
		CounterOn = false;
		return TimeLeft;
	}

	// Reset timer
	public void resetTime() {
		StartTime = Mathf.CeilToInt (Time.time);
		CounterOn = true;
	}
}
