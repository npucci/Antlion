using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour {

	float score;
	public Text showscore; //the number showed in the UI
	public Control player; //to get the player's score

	// Use this for initialization
	void Start () {
		score = 0;	//set the original score to be 0
		showscore.text = score.ToString ();

	}

	// Update is called once per frame
	void Update () {
		score = player.getscore(); 	//getscore() is a public function of the player class 
		showscore.text = score.ToString();
	}
}
