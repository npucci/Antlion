/* Team Solo Players
 * IAT 312
 * Assignment: Antlion Redesign
 * Date Created: February 11, 2017
 * Purpose: This script is used for the Player GameObject, which is the parent of the player's Ant GameObjects
 * NOTE: Can include as many ants as you want! Just add parent them, and the swapping/positioning mechanics will work automatically!
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private bool leftArrow = false; // detect left arrow
	private bool rightArrow = false; // detect right arrow
	private bool downArrow = false; // detect down arrow
	private List<GameObject> ant = new List<GameObject>(); // dynamic storage for all controllable Ant GameObjects

	public float speedX = 6.0f; // speed at which ants move on the x-axis
	public float distanceBetweenAnts = 1f; // defined distance between each ant in the party

	/* Purpose: called once at object instantiation, for setting values
	 * NOTE: any public values that are modified in the Unity editor will get that value overwritten
	 * if reassigned with this method
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Start () {
		for (int i = 0; i < transform.childCount; i++) {
			ant.Add(transform.GetChild (i).gameObject); // populate ant List with all children objects within Player GameObject 
		}
		positionAnts (); // position each ant in straight line with appropriate margins, according to their ordering
	}

	/* Purpose: called once every frame, setting player input boolean flags
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Update () {
		positionAnts (); // make sure ants have proper margins between them

		// check for player input keys
		if (Input.GetAxis ("Horizontal") == -1) {
			leftArrow = true;
		} 

		else if (Input.GetAxis ("Horizontal") == 1) {
			rightArrow = true;
		} 

		else {
			leftArrow = false;
			rightArrow = false;
			downArrow = false;
		}

		if (Input.GetAxis ("Vertical") == 1) {
			downArrow = true;
		} 

		if (Input.GetKeyUp (KeyCode.Space)) {
			swapAnts (); // move front ant to back of line
			positionAnts (); // assign ants to new positions in the line
		}
	}

	/* Purpose: called after each frame, which guarantees that all player input has been registered
	 * and applies updated movement at the end of the frame
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void FixedUpdate() {
		if (leftArrow) {
			transform.Translate (new Vector3 (-speedX * Time.deltaTime, 0f, 0f)); // move ants forward - DEBUG ONLY
		} 

		else if (rightArrow) {
			transform.Translate (new Vector3 (speedX * Time.deltaTime, 0f, 0f)); // move ants forward
		} 

		else if (downArrow) {
			// no dig method yet
			// dig();
		}
	}

	/* Original Creator: Niccolo Pucci
	 * Date created: February 11, 2017
	 * Purpose: Grabs the first ant in line, ant[0], and moves it to back of line
	 * by removing it from List and appending back on the List
	 * 
	 * Last Date Modified: February 11, 2017 by NP
	*/
	private void swapAnts() {
		if (ant.Count > 1) {
			GameObject antTemp = ant [0]; // stores the lead ant to be moved
			ant.Remove (ant[0]); // takes lead ant off the List 
			ant.Add (antTemp); // places lead ant at the back of List
		}
	}

	/* Original Creator: Niccolo Pucci
	 * Date created: February 11, 2017
	 * Purpose: checks and moves ants to their ordered positions in a straight line, 
	 * according to the margins specified by the public field distanceBetweenAnts
	 * NOTE: the lead ant will have its position at the origin of the Player GameObject
	 * 
	 * Last Date Modified: February 11, 2017 by NP
	*/
	private void positionAnts() {
		float bufferSpace = 0.2f; // gives ants a certain range of space they are allowed to be within

		for (int i = 0; i < ant.Count; i++) {
			float idealDistance = (transform.position.x - (distanceBetweenAnts * i)); // calculates the idea distance, relative to each ant

			if (ant [i].transform.position.x < idealDistance - bufferSpace) { // check if ant is too far back from position
				ant [i].transform.Translate (speedX * Time.deltaTime, 0f, 0f); // move ant forward
			}

			else if (ant [i].transform.position.x > idealDistance + bufferSpace) { // check if ant is too far ahead from position
				ant [i].transform.Translate (-speedX * Time.deltaTime, 0f, 0f); // move ant backwards
			}
		}
	}
}
