using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour {
	private GameStatus game_status = null;
	private conrol player_control = null;
	private AudioSource audioSound;
	public AudioClip clearSound;

	public enum STEP {
		NONE = -1,
		PLAY = 0,
		CLEAR,
		GAMEOVER,
		NUM,
	};

	public STEP step = STEP.NONE;
	public STEP next_step = STEP.NONE;
	public float step_timer = 0.0f;
	private float clear_time = 0.0f;
	public GUIStyle guistyle;

	void Start () {
		this.game_status = this.gameObject.GetComponent<GameStatus> ();
		this.player_control = GameObject.Find ("lp_guy").GetComponent<conrol>();
		this.step = STEP.PLAY;
		this.next_step = STEP.PLAY;
		this.guistyle.fontSize = 30;
		//this.guistyle;
		this.guistyle.normal.textColor = Color.white;
	}

	void Update () {
		this.step_timer += Time.deltaTime;
		if (this.next_step == STEP.NONE) {
			switch (this.step) {
			case STEP.PLAY:
				if (this.game_status.isGameClear ()) {
					this.next_step = STEP.CLEAR;
				}
				if (this.game_status.isGameOver ()) {
					player_control.animator.SetTrigger ("death");
					this.next_step = STEP.GAMEOVER;
				}
				break;
			case STEP.CLEAR:
				if (Input.GetMouseButtonDown (0)) {
					SceneManager.LoadSceneAsync ("escape");
				}
				break;
			case STEP.GAMEOVER:
				if (Input.GetMouseButtonDown (0)) {
					SceneManager.LoadSceneAsync ("escape");
				}
				break;
			}
		}
	
		while (this.next_step != STEP.NONE) {
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch (this.step) {
			case STEP.CLEAR:
			//	this.audioSound.clip = this.clearSound;
			//	this.audioSound.Play();
				this.player_control.enabled = false;
				this.clear_time = this.step_timer;
				break;
			case STEP.GAMEOVER:
				this.player_control.enabled = false;
				break;
			}
			this.step_timer = 0.0f;
		}
	}

	void OnGUI(){
		float pos_x = Screen.width * 0.3f;
		float pos_y = Screen.height * 0.5f;
		switch (this.step) {
		case STEP.PLAY:
			//GUI.color = Color.white;
			if (this.step_timer >= 100.0f) {
				GUI.Box (new Rect (pos_x * 3.0f - 50.0f, pos_y * 0.1f - 10, 130, 50), "");
			} else if (this.step_timer < 100.0f && this.step_timer >= 10.0f) {
				GUI.Box (new Rect (pos_x * 3.0f - 50.0f, pos_y * 0.1f - 10, 120, 50), "");
			} else if (this.step_timer < 10.0f) {
				GUI.Box (new Rect (pos_x * 3.0f - 50.0f, pos_y * 0.1f - 10, 102, 50), "");
			}
			GUI.Label (new Rect (pos_x*3.0f -30.0f, pos_y * 0.1f, 200, 20), this.step_timer.ToString ("0.00"), guistyle);
			break;
		case STEP.CLEAR:
			this.guistyle.fontSize = 64;
		//	this.guistyle.normal.textColor = Color.black;
			GUI.color = Color.white;
			GUI.Label (new Rect (pos_x - 230, pos_y, 100, 20), "Escape :" + this.clear_time.ToString ("0.00"), guistyle);
			GUI.Box (new Rect (pos_x + 500, pos_y-5, 180, 80), "");
			GUI.Label (new Rect (pos_x + 520, pos_y, 200, 20), "Click", guistyle);
			break;
		case STEP.GAMEOVER:
			this.guistyle.fontSize = 64;
			//this.guistyle.normal.textColor = Color.black;
			GUI.color = Color.white;
			GUI.Label (new Rect (pos_x- 230, pos_y, 200, 20), "Game Over", guistyle);
			GUI.Box (new Rect (pos_x + 500, pos_y-5, 180, 80), "");
			GUI.Label (new Rect (pos_x + 520, pos_y, 200, 20), "Click", guistyle);
			break;
		}
	}
}
