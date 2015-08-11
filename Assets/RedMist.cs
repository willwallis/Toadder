using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedMist : MonoBehaviour {

	public float normalAlpha = 0.0f;
	public float hitAlpha = 0.8f;
	public float fadeLength = 0.5f;
	public float waitLength = 0.1f;

	Image image;

	public void Fade(float start, float end, float length) {
		image = GetComponent<Image>();
		Color c = image.color;
		for (float i = 0.0f ; i < 1.0f ; i += Time.deltaTime * (1.0f / length)) {
			//for the length of time 
			c.a = Mathf.Lerp(start, end, i);
			image.color = c;
			
			//lerp the value of the transparency from the start value to the end value 
			//in equal increments yield; 
			// ensure the fade is completely finished (because lerp doesn't always end on an exact value)
		}
		c.a = end;
		image.color = c;
	}

	public void FlashWhenHit() {
		StartCoroutine(FlashWithWait());
	}

	// need to run as coroutine in order to get wait
	IEnumerator FlashWithWait() {
		Fade (normalAlpha, hitAlpha, fadeLength);
		yield return new WaitForSeconds (waitLength);
		Fade (hitAlpha, normalAlpha, fadeLength);
	}
		

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
