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
	private bool start = false; // flag used by the Level Controller to either start or stop the player
	private List<GameObject> ant = new List<GameObject>(); // dynamic storage for all controllable Ant GameObjects
	private GameObject throwableObject = null;
	private GameObject ground; // for referencing collissions with the ground
	private float digStartX = 0f; // the x position where ants should start digging
	private float digEndX = 0f; // the x position where ants should start surfacing
	private float bufferSpace = 0.1f; // gives ants a certain range of space they are allowed to be within
	private ScoreControl scoreKeeper;

	public float digSpeedY = 5f;
	public float digDepth = 3f;
	public float speedX = 7.0f; // speed at which ants move on the x-axis
	public float distanceBetweenAnts = 1.5f; // defined distance between each ant in the party

	/* Purpose: called once at object instantiation, for initializing fields
	 * NOTE: any public values that are modified in the Unity editor will get that value overwritten
	 * if reassigned with this method
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Start () {
		scoreKeeper = GameObject.Find ("Level Manager").GetComponent<ScoreControl> ();
		ground = GameObject.Find ("Ground").GetComponent<Transform>().gameObject;
		for (int i = 0; i < transform.childCount; i++) {
			ant.Add(transform.GetChild (i).gameObject); // populate ant List with all children objects within Player GameObject
		}
		initializeIgnoreColliders ();
		initializeAnts ();
		positionAnts (); // position each ant in straight line with appropriate margins, according to their ordering and position from point
	}

	/* Purpose: called once every frame, setting player input boolean flags
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void Update () {
		positionAnts (); // make sure ants have proper margins between them
		if (start) {

			// horizontal movement input checks
			if (!allAntsEaten () && Input.GetKey (KeyCode.LeftArrow)) {
				moveLeftButton = true;
				moveRightButton = false;
			} else if (!allAntsEaten () && Input.GetKeyDown (KeyCode.RightArrow)) {
				moveRightButton = true;
				moveLeftButton = false;
			} else {
				moveLeftButton = false;
				moveRightButton = false;
			}

			// vertical movement input checks
			if (!allAntsEaten () && Input.GetKeyDown (KeyCode.DownArrow)) { 
				moveDownButton = true;
				moveUpButton = false;
			} else if (!allAntsEaten () && Input.GetKeyDown (KeyCode.UpArrow)) {
				moveUpButton = true;
				moveDownButton = false;
			} else if (!allAntsEaten () && Input.GetKeyDown (KeyCode.LeftControl)) {
				attackButton = true;
			} else if (!allAntsEaten () && Input.GetKeyUp (KeyCode.LeftControl)) {
				attackButton = false;
			} else {
				moveDownButton = false;
				moveUpButton = false;
			}

			// action key input checks
			if (!allAntsEaten () && Input.GetKeyUp (KeyCode.Space) && allAntsAboveSurface ()) {
				swapAnts (); // move front ant to back of line
				positionAnts (); // assign ants to new positions in the line
			}
		}
	}

	/* Purpose: called after each frame, which guarantees that all player input has been registered
	 * and applies updated movement at the end of the frame
	 *
	 * Last Date Modified: February 11, 2017 by NP
	*/ 
	void FixedUpdate() {
		// game started
		if (start) {
			float moveX = speedX * Time.deltaTime;

			// Start digging
			if (!allAntsEaten () && moveDownButton) { //  && allAntsAboveSurface ()
				digStartX = ant [0].transform.position.x;
				if (leadAntStuck ()) {
					 digStartX -= 5.5f; // move the dig start back so that following ants can recuperate
				} 
			
				isDigging = true;
				antCollidersIgnoreGround (true); // disable ant colliders to ignore all ground colliders
			} 

			// Start surfacing
			else if (!allAntsEaten () && moveUpButton) {
				if (isDigging) {
					digEndX = ant [0].transform.position.x;
				}
				isDigging = false;	
			} 

			else if (!allAntsEaten () && ant [0].name.Contains ("Carpenter Ant")) {
				if (attackButton && isLeadAntStuck ("Tree Obstacle")) {
					moveX = speedX * Time.deltaTime;
					Collider2D blockingObstacleColl = ant [0].GetComponent<AntCollider> ().getBlocingObstacleColl ();
					antCollidersIgnoreBlockingObstacle (blockingObstacleColl, true);
				}
			} 

			else if (!allAntsEaten () && ant [0].name.Contains ("Fire Ant")) {
				// if throwable object is in forn of lead ant, thow away any existing object being carried, and grab the new one
				if (attackButton && isLeadAntStuck ("Throwable Obstacle")) {
					throwThrowableObject ();
					pickUpThrowableObject ();
					attackButton = false;
				} else if (!allAntsEaten () && attackButton && throwableObject != null) {
					throwThrowableObject ();
				}
			}

			if (!allAntsEaten () && isDigging) {
				dig ();
				positionAnts ();
			} 

			else {
				if (!allAntsEaten () && allAntsAboveSurface ()) {
					antCollidersIgnoreGround (false); // re-enable ant colliders to collide all ground colliders
				}
				toSurface ();
				positionAnts ();
			}

			if (!allAntsEaten () && !allAntsAboveSurface () && ant [0].name != "Pavement Ant") {
				moveX = moveX / 2;
			}

			// add speed boost if it is being requested of the player
			if (!leadAntStuck ()) {
				moveX *= scoreKeeper.getBoost (moveRightButton); // either multiplies a boost if available, or 1
			} 

			else {
				moveX = 0f; // if lead ant is stuck, don't move at all
			}

			if (!allAntsEaten () && moveLeftButton) {
				Debug.Log ("here");
				moveX = moveX / 3; // slow down to a third the pace
			} 

			transform.Translate (new Vector3 (moveX, 0f, 0f));
		}

		// game stopped
		else {
			forceToSurface ();
			if (allAntsAboveSurface ()) {
				stopAntAnimations ();
			}
		}
	}

	// Purpose: handles descending dig movment of ants
	private void dig() {
		float moveY = 0f;
		float maxDepth = transform.position.y - digDepth;
		for (int i = 0; i < ant.Count; i++) {
			if (ant [i].transform.position.x >= digStartX && ant [i].transform.position.y > maxDepth) {
				moveY = -digSpeedY * Time.deltaTime;
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, -45f);
			} 

			else {
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 0f);
				moveY = 0f;
			}
	
			if (ant [0].name != "Pavement Ant") { // if front ant is not Pavement Ant, slow down movement
				moveY = moveY / 2;
			}

			if (ant [i].transform.position.y < maxDepth) {
				ant [i].transform.position = new Vector3 (ant [i].transform.position.x, maxDepth, ant [i].transform.position.z);
			} 

			else {	
				ant [i].transform.Translate (new Vector3 (0f, moveY, 0f));
			}
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

	// Purpose: Forces all ants to surface - used for ending level at Ant Hill
	private void forceToSurface() {
		float moveY = 0f;
		float surface = transform.position.y;

		for (int i = 0; i < ant.Count; i++) {
			if (ant [i].transform.position.y < surface) {
				moveY = digSpeedY * Time.deltaTime/4;
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 45f);
			} 

			else {
				ant [i].transform.rotation = Quaternion.Euler (0f, 0f, 0f);
			}

			ant [i].transform.Translate (new Vector3 (0f, moveY, 0f));
		}
	}
		
	// Purpose: checks and moves ants to their ordered positions in a straight line, and appends trowable objects at very back 
	private void positionAnts() {
		bool leadAntStuck = false;
		if (!allAntsEaten ()) {
			leadAntStuck = ant [0].GetComponent<AntCollider> ().isStuck ();
		}

		bool fireAntExists = false;
		Vector3 fireAntPosition = Vector3.zero;

		for (int i = 0; i < ant.Count && !leadAntStuck; i++) {
			float idealDistanceX = (transform.position.x - (distanceBetweenAnts * i)); // calculates the ideal x distance of each ant, relative to its parent Player

			// check x positioning - too far back from ideal position
			if (ant [i].transform.position.x < idealDistanceX - bufferSpace) {
				ant [i].transform.Translate (speedX * Time.deltaTime, 0f, 0f); // move ant forward
			}

			// check x positioning - too far forward from ideal position
			else if (ant [i].transform.position.x > idealDistanceX + bufferSpace) {
				ant [i].transform.Translate (-speedX * Time.deltaTime, 0f, 0f); // move ant backwards
			}

			// check that is used for throwable object positioning
			if (ant [i].name.Contains ("Fire Ant")) {
				fireAntPosition = ant [i].transform.position;
				fireAntExists = true;
			}

		}

		// move throwable object to position of fire ant, if it exists
		if (throwableObject != null && fireAntExists) {
			throwableObject.GetComponent<SpriteRenderer> ().flipY = true;

			// check if throwable object is too far back from fire ant
			if (throwableObject.transform.position.x < fireAntPosition [0] - bufferSpace) { 
				throwableObject.transform.Translate (speedX * Time.deltaTime, 0f, 0f);
			}

			// check if throwable object is too far in front from fire ant
			else if (throwableObject.transform.position.x > fireAntPosition [0] + bufferSpace) {
				throwableObject.transform.Translate (-speedX * Time.deltaTime, 0f, 0f);
			}

			// check if throwable object is too far above fire ant
			if (throwableObject.transform.position.y > fireAntPosition [1] + bufferSpace) { 
				throwableObject.transform.Translate (0f, -digSpeedY * Time.deltaTime, 0f);
			}

			// check if throwable object is too far below fire ant
			else if (throwableObject.transform.position.y < fireAntPosition [1] - bufferSpace) {
				throwableObject.transform.Translate (0f, digSpeedY * Time.deltaTime, 0f);
			}
		} 
	}

	// Purpose: enables/Disables ant colliders with ground colliders when digging
	private void antCollidersIgnoreGround(bool ignore) {
		for (int i = 0; i < ant.Count; i++) {
			objectColliderIgnoreGround (ant [i], ignore);
		}
	}

	private void throwableObjectColliderIgnoreGround(bool ignore) {
		if(throwableObject != null) {
			objectColliderIgnoreGround (throwableObject, ignore);
		}
	}

	private void objectColliderIgnoreGround(GameObject objectWithCollider, bool ignore) {
		Collider2D[] groundColliders = ground.GetComponentsInChildren<Collider2D> ();
		for (int i = 0; i < groundColliders.Length; i++) {
			Physics2D.IgnoreCollision (objectWithCollider.GetComponent<Collider2D> (), groundColliders [i], ignore);
		}
		Rigidbody2D rb = objectWithCollider.GetComponent<Rigidbody2D> ();
		if (ignore) {
			rb.gravityScale = 0f;
			rb.freezeRotation = true;
		} 

		else {
			rb.gravityScale = 1f;
			rb.freezeRotation = false;
		}
	}

	// Purpose: enables/Disables ant colliders with collider of blocking obstacle
	private void antCollidersIgnoreBlockingObstacle(Collider2D blockingObstacleColl, bool ignore) {
		for (int i = 0; i < ant.Count; i++) {
			Physics2D.IgnoreCollision (ant [i].GetComponent<Collider2D> (), blockingObstacleColl, ignore);
		}

		if (throwableObject != null) {
			Physics2D.IgnoreCollision (throwableObject.GetComponent<Collider2D> (), blockingObstacleColl, ignore);
		}
	}

	private void pickUpThrowableObject() {
		throwableObject = ant[0].GetComponent<AntCollider>().getBlocingObstacleColl().gameObject;
		antCollidersIgnoreBlockingObstacle (throwableObject.GetComponent<Collider2D>(), true);
		throwableObjectColliderIgnoreGround (true);
	}

	private void throwThrowableObject() {
		if (throwableObject != null && throwableObject.transform.position.y >= transform.position.y) {
			throwableObjectColliderIgnoreGround (false);
			throwableObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-10f, 5f), ForceMode2D.Impulse);
			GameObject.Find ("Antlion").GetComponent<AntlionBehavior>().enableAntlionCharacterCollider(throwableObject);
			throwableObject = null;
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
	public bool allAntsAboveSurface() {
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
		}
	}

	private bool leadAntStuck() {
		if (!allAntsEaten ()) {
			return ant [0].GetComponent<AntCollider> ().isStuck ();
		}
		return true;
	}

	private bool isLeadAntStuck(string obstacleName) {
		if (allAntsEaten ()) {
			return false;
		}
		Collider2D blockingObstacleColl = ant [0].GetComponent<AntCollider> ().getBlocingObstacleColl ();
		if (blockingObstacleColl != null && blockingObstacleColl.name.Contains (obstacleName)) {
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

	public Vector2 getLastAntPosition() {
		if (!allAntsEaten ()) {
			return ant [ant.Count - 1].transform.position;
		}

		return Vector2.zero;
	}

	public GameObject lastAntEaten(GameObject eatenAnt) {
		GameObject antGoner = null;
		if (!allAntsEaten() && ant [ant.Count - 1].Equals (eatenAnt)) {
			antGoner = ant [ant.Count - 1];
			ant.RemoveAt(ant.Count -1);
		}
		return antGoner;
	}

	public bool allAntsEaten() {
		return ant.Count == 0;
	}

	public float getDigStartX() {
		return digStartX;
	}

	public float getDigEndX() {
		return digEndX;
	}

	private void initializeIgnoreColliders () {
		Collider2D coll = GetComponent<Collider2D> ();
		GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();
		foreach (GameObject obj in allGameObjects) {
			if (obj.activeInHierarchy) {
				Physics2D.IgnoreCollision (coll, obj.GetComponent<Collider2D>(), true);
			}
		}

		Collider2D[] groundColliders = ground.GetComponentsInChildren<Collider2D> ();
		for (int i = 0; i < groundColliders.Length; i++) {
			Physics2D.IgnoreCollision (coll, groundColliders [i], false);
		}
	}

	public bool holdingThrowableObject() {
		return throwableObject != null;
	}

	public void destroyThrowableObject () {
		if (throwableObject != null) {
			GameObject tempObj = throwableObject;
			throwableObject = null;
			Destroy (tempObj);
		}
	}

	public void startMovement() {
		start = true;
		playAntAnimations ();
	}

	public void stopMovement() {
		start = false;
		if (allAntsAboveSurface ()) {
			stopAntAnimations ();
		}
	}

	private void stopAntAnimations () {
		for (int i = 0; i < ant.Count; i++) {
			ant [i].GetComponent<Animator> ().Stop ();
		}
	}

	private void playAntAnimations () {
		for (int i = 0; i < ant.Count; i++) {
			//ant [i].GetComponent<Animator> ().StartPlayback();//Play ("Base Layer RUN");
		}
	}

	public bool lastAntDigging() {
		bool isDigging = false;
		if (ant [ant.Count - 1].transform.position.y <= transform.position.y - digDepth + bufferSpace &&
			ant [ant.Count - 1].transform.position.y >= transform.position.y - digDepth - bufferSpace) {
			isDigging = true;
		}
		return isDigging;
	}

	public float getDigDepth() {
		return digDepth;
	}

	public float positionYLastAnt() {
		return ant [ant.Count - 1].transform.position.y;
	}
}