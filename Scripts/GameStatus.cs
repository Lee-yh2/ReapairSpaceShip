using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour {
	public static float GAIN_REPAIRMENT_IRON = 0.40f;

	public static float CONSUME_SATIETY_IRON = 0.02f;
	public static float CONSUME_SATIETY_APPLE = 0.005f;

	public static float REGAIN_SATIETY_APPLE = 0.3f;

	public float repairment = 0.0f;
	public float satiety = 1.0f;

	public GUIStyle guistyle;
	// Use this for initialization
	void Start () {
		this.guistyle.fontSize = 24;
		this.guistyle.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		float x = Screen.width * 0.2f;
		float y = 20.0f;

		GUI.Box (new Rect (x*0.1f + 5, y, 155.0f, 50.0f), "");
		GUI.Label(new Rect (x*0.1f + 30, y+10, 100.0f, 30.0f), "Energy : " + (this.satiety * 100.0f).ToString ("000"),guistyle);
	//	x += 200;
		GUI.Box (new Rect (x*0.9f -30, y, 160.0f, 50.0f), "");
		GUI.Label (new Rect (x*0.9f -15, y+10, 200.0f, 20.0f), "SpaceShip : " + (this.repairment * 100.0f).ToString ("000"), guistyle);
	}

	public void addRepairment(float add){
		this.repairment = Mathf.Clamp01 (this.repairment + add);
	}

	public void addSatiety(float add){
		this.satiety = Mathf.Clamp01 (this.satiety + add);
	}

	public bool isGameClear(){
		bool is_clear = false;
		if (this.repairment >= 1.0f) {
			is_clear = true;
		}
		return(is_clear);
	}

	public bool isGameOver(){
		bool is_over = false;
		if (this.satiety <= 0.0f) {
			is_over = true;
		}
		return(is_over);
	}
}
