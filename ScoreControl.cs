using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour {
	private int score;

	private int speedBoostsAvailable = 0; // power up
	private float speedBoost = 2.0f;
	public float boostTimer = 5.0f; // times power up
	private float boostCountDown = 0f; // stores current timing of timer

	private int scoreMultiplier = 2; // power up
	private int currentScoreMultiplier = 1;
	public float multiplierTimer = 5f; // times power up
	private float multiplierCountDown = 0f; // stores current timing of timer

	private AudioSource itemCollectSound;

	public Text hudShowScore; //the number showed in the UI HUD Canvas
	public Text hudShowMultiplier;
	public Text hudShowBoosts;
	public Text pauseShowScore; //the number showed in the UI Pause Canvas
	public Text pauseShowMultiplier;
	public Text pauseShowBoosts;

	// Use this for initialization
	void Start () {
		score = 0;	//set the original score to be 0
		itemCollectSound = GetComponent<AudioSource>();
		updateTexts();
	}

	// Update is called once per frame
	void Update () {
		updateTexts();
	
		if (boostCountDown > 0) {
			boostCountDown -= Time.deltaTime;
		}

		if (multiplierCountDown > 0) {
			multiplierCountDown -= Time.deltaTime;
		} 

		else {
			currentScoreMultiplier = 1;
		}
	}

	public void incrementScore(string itemName) {
		itemCollectSound.Play (); 
		// apple == 3 points
		if (itemName.Contains ("Apple Item")) {
			score += 3 * currentScoreMultiplier;
		} 

		// strawberry == 2 points
		else if (itemName.Contains ("Strawberry Item")) {
			score += 2 * currentScoreMultiplier;
		} 

		// blueberry == 1 point
		else if (itemName.Contains ("Blueberry Item")) {
			score += 1 * currentScoreMultiplier;
		} 

		// sugar cube == 2x multiplier (compounding)
		else if (itemName.Contains ("Sugar Cube Item")) {
			// if timer is not counting down, set it and start
			if (multiplierCountDown <= 0) {
				multiplierCountDown = multiplierTimer;
			}
			currentScoreMultiplier *= scoreMultiplier;
		}

		// nectar drop == 1 speed boost (time accumulative)
		else if (itemName.Contains ("Nectar Drop Item")) {
			speedBoostsAvailable += 1;
		}
	}

	public float getBoost(bool request) {
		// if timer is currently going
		if (boostCountDown > 0) {
			return speedBoost;
		} 

		// if player requests a boost, and there is one available
		else if (request && speedBoostsAvailable > 0) {
			speedBoostsAvailable -= 1;
			boostCountDown = boostTimer;
			return speedBoost;
		}

		// default boost
		else {
			return 1f;
		}
	}

	public int getScore() {
		return score;
	}

	private void updateTexts() {
		hudShowScore.text = score.ToString ();
		hudShowMultiplier.text = "(x" + currentScoreMultiplier.ToString () + ")";
		hudShowBoosts.text = speedBoostsAvailable.ToString ();

		pauseShowScore.text = score.ToString ();
		pauseShowMultiplier.text = "(x" + currentScoreMultiplier.ToString () + ")";
		pauseShowBoosts.text = speedBoostsAvailable.ToString ();
	}
}
