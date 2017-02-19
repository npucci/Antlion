using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour {
	private CanvasGroup pauseCanvas;
	private CanvasGroup gameOverCanvas;
	private CanvasGroup hudCanvas;
	private CanvasGroup winCanvas;
	private bool pause = false;
	private bool hasWon = false;
	private bool hasLost = false;

	public Text score;

	void Awake() {
	}
		
	void Start () {
		pauseCanvas = GameObject.Find ("Pause Canvas").GetComponent<CanvasGroup> ();
		gameOverCanvas = GameObject.Find ("GameOver Canvas").GetComponent<CanvasGroup> ();
		hudCanvas = GameObject.Find ("HUD Canvas").GetComponent<CanvasGroup> ();
		winCanvas = GameObject.Find ("Win Canvas").GetComponent<CanvasGroup> ();
		initializeCanvas ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			pause = !pause;
			if (pause) {
				pauseGame ();
			} 
			else {
				resumeGame ();
			}
		}

		// debug restart
		if (Input.GetKeyDown (KeyCode.F1)) {
			restartLevel();
		}

		// debug go to main menu
		if (Input.GetKeyDown (KeyCode.F2)) {
			goToMainMenu();
		}
	}

	private void initializeCanvas () {
		// Enable HUD Canvas
		enableCanvas(hudCanvas, true);

		// Disable Pause Canvas
		enableCanvas(pauseCanvas, false);

		// Disable Win Canvas
		enableCanvas(winCanvas, false);

		// Disable Game Over Canvas
		enableCanvas(gameOverCanvas, false);
	}

	private void pauseGame() {
		Time.timeScale = 0f;
		// Disable HUD Canvas
		enableCanvas(hudCanvas, false);
		// Enable Pause Canvas
		enableCanvas(pauseCanvas, true);
	}

	private void resumeGame() {
		Time.timeScale = 1f;
		// Disable Pause Canvas
		enableCanvas(pauseCanvas, false);
		// Enable HUD Canvas
		enableCanvas(hudCanvas, true);
	}

	public void gameOver() {
		if (!hasLost) {
			// True Game Over Canvas
			enableCanvas (gameOverCanvas, true);
			hasLost = true;
		}
	}

	public void win() {
		if (!hasWon) {
			// Disable HUD Canvas
			enableCanvas (hudCanvas, false);
			// Enable Win Canvas
			enableCanvas (winCanvas, true);
			score.text += GameObject.Find ("Level Manager").GetComponent<ScoreControl> ().getScore ().ToString ();
			hasWon = true;
		}
	}

	private void enableCanvas(CanvasGroup canvas, bool enable) {
		if(enable) {
			canvas.alpha = 1f;
		}
		else {
			canvas.alpha = 0f;
		}

		canvas.interactable = enable;
		canvas.blocksRaycasts = enable;
	}

	public void goToMainMenu() {
		Application.LoadLevel (0);
	}

	public void restartLevel() {
		Application.LoadLevel (SceneManager.GetActiveScene().buildIndex);
	}
}
