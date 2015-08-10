using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDownTimer : MonoBehaviour {

	public Text CountDownText;
	public Text WinMessage;
	public int TimeLimit;
	public bool CounterOn;

	private int CurrentTime;
	private int TimeLeft;


	void OnGUI() {
		if (CounterOn) {
			CurrentTime = Mathf.CeilToInt (Time.time);
			TimeLeft = TimeLimit - CurrentTime;
		}
		CountDownText.text = "Time: " + TimeLeft.ToString ("D4");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
