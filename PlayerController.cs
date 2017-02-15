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
	private bool moveLeftButton = false; // detect left arrow or a key
	private bool moveRightButton = false; // detect right arrow or d key
	private bool moveUpButton = false; // detect up arrow or w key
	private bool moveDownButton = false; // detect down arrow or s key
	private bool attackButton = false; // detect ctrl key
	private bool isDigging = false; // check to see if ants are digging
	private bool isChewing = false; // check to see if ants are chewing
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
			moveLeftButton = true;
			moveRightButton = false;
		} 

		else if (Input.GetAxis ("Horizontal") == 1) {
			moveRightButton = true;
			moveLeftButton = false;
		} 

		else {
			moveLeftButton = false;
			moveRightButton = false;
		}

		// vertical movement input checks
		if (Input.GetAxis ("Vertical") == -1) { 
			moveDownButton = true;
			moveUpButton = false;
		}

		else if (Input.GetAxis ("Vertical") == 1) {
			moveUpButton = true;
			moveDownButton = false;
		} 

		else if (Input.GetKeyDown(KeyCode.LeftControl)) {
			attackButton = true;
		}

		else if (Input.GetKeyUp(KeyCode.LeftControl)) {
			attackButton = false;
		}

		else {
			moveDownButton = false;
			moveUpButton = false;
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

		if ( moveLeftButton) {
			moveX = -speedX * Time.deltaTime;
		} 

		else if (!leadAntStuck() && moveRightButton) {
			moveX = speedX * Time.deltaTime;
		} 

		// Start digging
		if (moveDownButton && allAntsAboveSurface ()) {
			digStartX = ant [0].transform.position.x;
			isDigging = true;
			antCollidersIgnoreGround (true); // disable ant colliders to ignore all ground colliders
		} 

		// Start surfacing
		else if (moveUpButton) {
			if (isDigging) {
				digEndX = ant [0].transform.position.x;
			}
			isDigging = false;	
		} 

		else if (attackButton && leadAntStuckTree()) {
			if (ant [0].name.Contains ("Carpenter Ant")) {
				moveX = speedX * Time.deltaTime;
				Collider2D blockingObstacleColl = ant [0].GetComponent<AntCollider> ().getBlocingObstacleColl();
				antCollidersIgnoreBlockingObstacle (blockingObstacleColl, true);
			}
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
			moveX = moveX / 2;
		}
		transform.Translate (new Vector3 (moveX, 0f, 0f)); 
	}

	/* Purpose: add up the score the player have and display the score in 
	 *
	 * Last Date Modified: February 14, 2017 by skyler
	*/
	public int getscore()
	{
		return score;
	}

	public void incrementScore() {
		score++;
	}

	// Purpose: handles chewing through tree
	private void chewThrough() {

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

	// Purpose: moves all ants to the surface, bit by bit, frame by frame
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
		bool leadAntStuck = ant [0].GetComponent<AntCollider> ().isStuck ();

		for (int i = 0; i < ant.Count && !leadAntStuck; i++) {
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

	// Purpose: enables/Disables ant colliders with ground colliders when digging
	private void antCollidersIgnoreGround(bool ignore) {
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

	// Purpose: enables/Disables ant colliders with collider of blocking obstacle
	private void antCollidersIgnoreBlockingObstacle(Collider2D blockingObstacleColl, bool ignore) {
		for (int i = 0; i < ant.Count; i++) {
			Physics2D.IgnoreCollision (ant [i].GetComponent<Collider2D> (), blockingObstacleColl, ignore);
		}
	}

	// Purpose: grabs the first ant in line, ant[0], and moves it to back of line
	// by removing it from List and appending back on the List
	private void swapAnts() {
		if (ant.Count > 1) {
			GameObject antTemp = ant [0]; // stores the lead ant to be moved
			ant[0].GetComponent<Rigidbody2D>().isKinematic = false;
			ant.Remove (ant[0]); // takes lead ant off the List 
			ant.Add (antTemp); // places lead ant at the back of List
		}
	}

	// Purpose: checks if all Player's ants are above surface
	private bool allAntsAboveSurface() {
		for (int i = 0; i < ant.Count; i++) {
			if (ant [i].transform.position.y < transform.position.y) {
				return false;
			}
		}
		return true;
	}

	// Purpose: initializes all Player's ants so that their colliders ignore each other for when they swap.
	private void initializeAnts() {
		for (int i = 0; i < ant.Count; i++) {
			for (int j = i + 1; j < ant.Count; j++) {
				Physics2D.IgnoreCollision (ant[i].GetComponent<Collider2D>(), ant[j].GetComponent<Collider2D>());
			}
			//Physics2D.IgnoreCollision (transform.GetComponent<Collider2D>(), ant[i].GetComponent<Collider2D>());
		}
	}

	private bool leadAntStuck() {
		return ant [0].GetComponent<AntCollider> ().isStuck ();
	}

	private bool leadAntStuckTree() {
		Collider2D blockingObstacleColl = ant [0].GetComponent<AntCollider> ().getBlocingObstacleColl ();
		if (blockingObstacleColl != null && blockingObstacleColl.name.Contains ("Tree Obstacle")) {
			return true;
		}
		return false;
	}

	private bool noAntStuck() {
		for (int i = 0; i < ant.Count; i++) {
			if (ant [i].GetComponent<AntCollider> ().isStuck ())
				return false;
		}
		return true;
	}
}