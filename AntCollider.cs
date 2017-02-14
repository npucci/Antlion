using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntCollider : MonoBehaviour {
	private PlayerController player;

	void Start() {
		player = GameObject.Find("Player").GetComponent<PlayerController>();;
	}
		
	private void OnCollisionEnter2D(Collision2D Coll)
	{
		if (Coll.gameObject.name.Contains("Item")) {
			player.incrementScore ();
			Destroy (Coll.gameObject);
		} 
	}
}