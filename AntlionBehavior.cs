using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntlionBehavior : MonoBehaviour {
	private GameObject antlionCharacter;
	private GameObject player;
	private GameObject ground; // for referencing collissions with the ground
	public float speedX = 3.0f;
	private bool eating = false;
	private Animator animator;
	private float countDownTimer = 0f;
	private bool gameOver = false;

	public float eatingTime = 2f;

	void Start () {
		antlionCharacter = transform.GetChild (0).gameObject;
		player = GameObject.Find ("Player").GetComponent<Transform> ().gameObject;
		ground = GameObject.Find ("Ground").GetComponent<Transform>().gameObject;
		//transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		initializeIgnoreColliders (); 
		animator = antlionCharacter.GetComponent<Animator> ();
		animator.speed = 1;
	}

	void Update () {
		if (player.GetComponent<PlayerController> ().allAntsEaten ()) {
			gameOver = true;
		}

		Vector3 playerLastAnt = Vector3.zero;

		if (!eating && !gameOver) {
			playerLastAnt = player.GetComponent<PlayerController> ().getLastAntPosition();

			if (playerLastAnt [1] < transform.position.y) {
				//dig ();
			}

			else if (playerLastAnt[1] > transform.position.y)
				
			transform.Translate (new Vector3 (speedX * Time.deltaTime, 0f, 0f));
			moveAntlion ();
		}

		if (countDownTimer > 0 && !gameOver) {
			countDownTimer = countDownTimer - Time.deltaTime;
		} 

		else {
			eating = false;
		}
	}

	private void moveAntlion() {
		float bufferSpace = 0.2f;
		if (antlionCharacter.transform.position.x < transform.position.x - bufferSpace) {
			antlionCharacter.transform.Translate (new Vector3 (speedX * Time.deltaTime, 0f, 0f));
		}
		else if (antlionCharacter.transform.position.x > transform.position.x + bufferSpace) {
			antlionCharacter.transform.Translate (new Vector3 (-speedX * Time.deltaTime, 0f, 0f));
		}
	}
		
	/*
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
	*/

	private void initializeIgnoreColliders () {
		Collider2D coll = GetComponent<Collider2D> ();
		Collider2D collAntlion = antlionCharacter.GetComponent<Collider2D> ();
		Physics2D.IgnoreCollision (collAntlion, coll, true);

		GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject> ();
		for (int i = 0; i<  allGameObjects.Length; i++) {
			if (allGameObjects[i].activeInHierarchy) {
				Physics2D.IgnoreCollision (coll, allGameObjects[i].GetComponent<Collider2D> (), true);
			}
			if (allGameObjects[i].activeInHierarchy && !allGameObjects[i].name.Contains("Ant")) {
				Physics2D.IgnoreCollision (collAntlion, allGameObjects[i].GetComponent<Collider2D>(), true);
			}
		}

		Collider2D[] groundColliders = ground.GetComponentsInChildren<Collider2D> ();
		for (int i = 0; i < groundColliders.Length; i++) {
			Physics2D.IgnoreCollision (coll, groundColliders [i], false);
			Physics2D.IgnoreCollision (collAntlion, groundColliders [i], false);
		}
	}

	public bool isGameOver() {
		return gameOver;
	}

	public void setEating(bool isEating) {
		eating = isEating;
		countDownTimer = eatingTime;
	}
}
