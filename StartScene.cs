using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour {
	public enum LEVEL {EASY, NORMAL, HARD};
	public static LEVEL Level;
	public int Sequence;
	private float Timer;
	private bool jumpflag=false;


	private void StartGame(){
		Application.LoadLevel (1);//change the variable according to your build settings^^
	}

	public void EasyMode(){
		Level = LEVEL.EASY;
		StartGame ();
	}
	public void NormalMode(){
		Level = LEVEL.NORMAL;
		StartGame ();
	}
	public void HardMode(){
		Level = LEVEL.HARD;
		StartGame ();
	}

	public void QuitButton()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}




	void Start () {

	}
		
	void Update () {
		Timer += Time.deltaTime;
		//Debug.Log (Timer);
		int tmp = (int)(Timer*2.5);
		if (tmp % 3 == Sequence && !jumpflag) {
			transform.position = new Vector3 (transform.position.x, transform.position.y+0.1f, transform.position.z);

			jumpflag = !jumpflag;
		}
		if (tmp % 3 != Sequence && jumpflag) {
			jumpflag = !jumpflag;
		}

	}
}



