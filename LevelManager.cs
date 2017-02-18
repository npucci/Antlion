using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	private GameObject antlionStartNode;
	private GameObject antlionEndNode;
	private GameObject playerStartNode;
	private GameObject playerEndNode;
	private GameObject player;
	private GameObject antlion;

	// Use this for initialization
	void Start () {
		antlionStartNode = transform.GetChild (0).gameObject;
		antlionEndNode = transform.GetChild (1).gameObject;
		playerStartNode = transform.GetChild (2).gameObject;
		playerEndNode = transform.GetChild (3).gameObject;

		player = GameObject.Find ("Player");
		player.transform.position = playerStartNode.transform.position;

		antlion = GameObject.Find ("Antlion");
		antlion.transform.position = antlionStartNode.transform.position;

		player.GetComponent<PlayerController> ().startMovement ();
		antlion.GetComponent<AntlionBehavior> ().startMovement ();
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x >= playerEndNode.transform.position.x) {
			Debug.Log ("You Win!");
			player.GetComponent<PlayerController>().stopMovement ();
		}

		if (antlion.transform.position.x >= antlionEndNode.transform.position.x) {
			antlion.GetComponent<AntlionBehavior>().stopMovement ();
		}
	}
}
