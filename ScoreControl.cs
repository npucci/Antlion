using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour {
	private float score;
	private PlayerController player; //to get the player's score

	public Text showscore; //the number showed in the UI

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").GetComponent<PlayerController> ();
		score = 0;	//set the original score to be 0
		showscore.text = "Score: " + score.ToString ();

	}

	// Update is called once per frame
	void Update () {
		if (player != null) {
			score = player.getscore (); 	//getscore() is a public function of the player class 
			showscore.text = "Score: " + score.ToString ();
		}

		if(player.GetComponent<PlayerController>().allAntsEaten()) {
			// GAME OVER
		}
	}
}
