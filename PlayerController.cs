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
	private bool upArrow = false; // detect up arrow
	private bool downArrow = false; // detect down arrow
	private bool eKeyDown = false; // detect D key
	private bool isDigging = false; // check to see if ants are digging
	private List<GameObject> ant = new List<GameObject>(); // dynamic storage for all controllable Ant GameObjects
	private GameObject ground; // for referencing collissions with the ground
	private float digStartX = 0f; // the x position where ants should start digging
	private float digEndX = 0f; // the x position where ants should start surfacing
	private int score = 0; 	//used to store the score 

	public float digSpeedY = 5f;
	public float digDepth = 3f;
	public float speedX = 6.0f; // speed at which ants move on the x-axis
	public float distanceBetweenAnts = 1.5f; // defined distance between each ant in the party

	/* Purpose: called once at object instantiation, for setting values
	 * NOTE: any public values that are modified in the Unity editor will get that value overwritten
	 * if reassigned with this method
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Start () {
		ground = GameObject.Find ("Ground").GetComponent<Transform>().gameObject;
		for (int i = 0; i < transform.childCount; i++) {
			ant.Add(transform.GetChild (i).gameObject); // populate ant List with all children objects within Player GameObject
		}
		initializeAnts ();
		positionAnts (); // position each ant in straight line with appropriate margins, according to their ordering and position from point
	}

	/* Purpose: called once every frame, setting player input boolean flags
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Update () {
		positionAnts (); // make sure ants have proper margins between them

		// horizontal movement input checks
		if (Input.GetAxis ("Horizontal") == -1) {
			leftArrow = true;
			rightArrow = false;
		} 

		else if (Input.GetAxis ("Horizontal") == 1) {
			rightArrow = true;
			leftArrow = false;
		} 

		else {
			leftArrow = false;
			rightArrow = false;
		}

		// vertical movement input checks
		if (Input.GetAxis ("Vertical") == -1) { 
			downArrow = true;
			upArrow = false;
		}

		else if (Input.GetAxis ("Vertical") == 1) {
			upArrow = true;
			downArrow = false;
		}

		else {
			downArrow = false;
			upArrow = false;
		}

		// action key input checks
		if (Input.GetKeyUp (KeyCode.Space) && allAntsAboveSurface()) {
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
		float moveX = 0f;

		if (leftArrow) {
			moveX = -speedX * Time.deltaTime;
		} 

		else if (rightArrow) {
			moveX = speedX * Time.deltaTime;
		} 

		if (downArrow && allAntsAboveSurface ()) {
			digStartX = ant [0].transform.position.x;
			isDigging = true;
			antCollidersIgnoreGround (true); // disable ant colliders to ignore all ground colliders
		} 

		else if (upArrow) {
			if (isDigging) {
				digEndX = ant [0].transform.position.x;
			}
			isDigging = false;	
		}

		if (isDigging) {
			dig ();
			positionAnts ();
		} 

		else {
			if (allAntsAboveSurface ()) {
				antCollidersIgnoreGround (false); // re-enable ant colliders to collide all ground colliders
			}
				toSurface ();
				positionAnts ();
		}

		if (!allAntsAboveSurface() && ant [0].name != "Pavement Ant") {
			Debug.Log ("slower");
			moveX = moveX / 2;
		}
		transform.Translate (new Vector3 (moveX, 0f, 0f)); 
	}

	/* Purpose: add up the score the player have and display the score in 
	 *
	 * Last Date Modified: February 14, 2017 by skyler
	*/
	void OnTriggerEnter2D(Collider2D Coll)
	{
		if (Coll.gameObject.name == "item") {
			score += 1;
			Destroy (Coll.gameObject);

		} 
	}
	public int getscore()
	{
		return score;
	}

	// Purpose: handles descending dig movment of ants
	private void dig() {
		float moveY = 0f;
		float maxDepth = transform.position.y - digDepth;

		for (int i = 0; i < ant.Count; i++) {
			if (ant[i].transform.position.x >= digStartX && ant [i].transform.position.y > maxDepth) {
				moveY = -digSpeedY * Time.deltaTime;
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, -45f);
			} else {
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 0f);
				moveY = 0f;
			}
	
			if (ant [0].name != "Pavement Ant") { // if front ant is not Pavement Ant, slow down movement
				moveY = moveY / 2;
			}
				
			ant [i].transform.Translate (new Vector3 (0f, moveY, 0f));
		}
	}

	private void toSurface() {
		float moveY = 0f;
		float surface = transform.position.y;

		for (int i = 0; i < ant.Count; i++) {
			if (ant[i].transform.position.x >= digEndX && ant [i].transform.position.y < surface) {
				moveY = digSpeedY * Time.deltaTime;
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 45f);
			} 

			else {
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 0f);
				moveY = 0f;
			}

			if (ant [0].name != "Pavement Ant") { // if front ant is not Pavement Ant, slow down movement
				moveY = moveY / 2;
			}

			ant [i].transform.Translate (new Vector3 (0f, moveY, 0f));
		}
	}
		
	// Purpose: checks and moves ants to their ordered positions in a straight line, 
	private void positionAnts() {
		float bufferSpace = 0.1f; // gives ants a certain range of space they are allowed to be within

		for (int i = 0; i < ant.Count; i++) {
			float idealDistanceX = (transform.position.x - (distanceBetweenAnts * i)); // calculates the ideal x distance of each ant, relative to its parent Player
			float idealDistanceY = (transform.position.y + (distanceBetweenAnts * i)); // calculates the ideal y distance of each ant, relative to its parent Player

			// check x positioning - too far back from ideal position
			if (ant [i].transform.position.x < idealDistanceX - bufferSpace) {
				ant [i].transform.Translate (speedX * Time.deltaTime, 0f, 0f); // move ant forward
			}

			// check x positioning - too far forward from ideal position
			else if (ant [i].transform.position.x > idealDistanceX + bufferSpace) {
				ant [i].transform.Translate (-speedX * Time.deltaTime, 0f, 0f); // move ant backwards
			}
		}
	}

	private void antCollidersIgnoreGround( bool ignore) {
		Collider2D[] groundColl = ground.GetComponentsInChildren<Collider2D> ();
		for (int i = 0; i < ant.Count; i++) {
			for (int j = 0; j < groundColl.Length; j++) {
				Physics2D.IgnoreCollision (ant [i].GetComponent<Collider2D> (), groundColl [j], ignore);
			}
			Rigidbody2D rb = ant[i].GetComponent<Rigidbody2D> ();
			if (ignore) {
				rb.gravityScale = 0f;
				rb.freezeRotation = true;
			} 

			else {
				rb.gravityScale = 1f;
				rb.freezeRotation = false;
			}
		}
	}

	// Purpose: Grabs the first ant in line, ant[0], and moves it to back of line
	// by removing it from List and appending back on the List
	private void swapAnts() {
		if (ant.Count > 1) {
			GameObject antTemp = ant [0]; // stores the lead ant to be moved
			ant[0].GetComponent<Rigidbody2D>().isKinematic = false;
			ant.Remove (ant[0]); // takes lead ant off the List 
			ant.Add (antTemp); // places lead ant at the back of List
		}
	}

	private bool allAntsAboveSurface() {
		for (int i = 0; i < ant.Count; i++) {
			if (ant [i].transform.position.y < transform.position.y) {
				return false;
			}
		}
		return true;
	}

	private void initializeAnts() {
		for (int i = 0; i < ant.Count; i++) {
			for (int j = i + 1; j < ant.Count; j++) {
				Physics2D.IgnoreCollision (ant[i].GetComponent<Collider2D>(), ant[j].GetComponent<Collider2D>());
			}
		}
	}
}