/* Team Solo Players
 * IAT 312
 * Assignment: Antlion Redesign
 * Date Created: February 1, 2017
 * Purpose: Script plays beat
 * 
 * PLEASE NOTE: 
 * 
 * THIS EXPERIMENTAL SCRIPT IS A MESS!
 * 
 * It is functional, but not optimized. Haven't got around to redesigning it. ~ NP
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCreator : MonoBehaviour {
	private float startTime;
	public float interval;
	private float nextBeat;
	public int freq;
	private int count;
	//private float targetTime;
	private AudioSource audio;

	private bool jump;

	// Use this for initialization
	void Start () {
		startTime = 0.0f;
		interval = 1.0f;
		nextBeat = startTime + interval;
		//targetTime = 4.0f;
		freq = 0;
		count = freq;
		audio = GetComponent<AudioSource> ();

		jump = false;
	}
	
	// Update is called once per frame
	void Update () {
		startTime = startTime + Time.deltaTime;

		float timeDiff = Mathf.Abs (startTime - nextBeat);
		//bool resetTime = startTime >= targetTime;

		//if (timeDiff >= 0 && timeDiff < 0.1 && !resetTime) {
		if (timeDiff >= 0 && timeDiff < 0.1) {
			audio.Stop ();
			if (count <= 0) {
				audio.Play ();
				count = freq;

				jump = true;
			}
			//Debug.Log ("PLAY @ " + startTime);
			nextBeat = nextBeat + interval;
			if (freq != 0) {
				count--;
			}
				
		} else {
			jump = false;
		}

		//if (resetTime) {
			//audio.Stop ();
			//Debug.Log ("\nRESET" + startTime % 2 + "\n");
			//startTime = 0;
			//nextBeat = startTime + interval;
		//}

		//Debug.Log (startTime);
	}

	public bool canJump() {
		return jump;
	}
}//test
