using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntCollider : MonoBehaviour {
	private PlayerController player;
	private Collider2D blockingObstacleColl;

	void Start() {
		player = GameObject.Find("Player").GetComponent<PlayerController>();;
		blockingObstacleColl = null;
	}
		
	private void OnCollisionEnter2D(Collision2D coll)
	{
		string collName = coll.gameObject.name;

		if (collName.Contains("Item")) {
			player.incrementScore ();
			Destroy (coll.gameObject);
		}
	}

	private void OnCollisionStay2D(Collision2D coll) {
		string collName = coll.gameObject.name;
		if (collName.Contains("Tree Obstacle") || collName.Contains("Rock Obstacle") || collName.Contains("Bug Obstacle")) {
			if (transform.position.y >= coll.gameObject.transform.position.y - 2f) {
				blockingObstacleColl = coll.gameObject.GetComponent<Collider2D> ();
			}
		}
	}

	private void OnCollisionExit2D(Collision2D coll) {
		string collName = coll.gameObject.name;

		if (collName.Contains("Tree Obstacle") || collName.Contains("Rock Obstacle") || collName.Contains("Bug Obstacle")) {
			blockingObstacleColl = null;
		}
	}

	public bool isStuck() {
		return blockingObstacleColl != null;
	}
		
	public Collider2D getBlocingObstacleColl() {
		return blockingObstacleColl;
	}
}