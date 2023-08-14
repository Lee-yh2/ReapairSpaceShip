using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item{
	public enum TYPE{
		NONE = -1,
		IRON = 0,
		APPLE,
		NUM,
	}
}

public class ItemRoot : MonoBehaviour {
	public GameObject ironPrefab = null;
	public GameObject applePrefab = null;
	protected List<Vector3> iron_respawn_points;
	protected List<Vector3> apple_respawn_points;

	public float step_timer = 0.0f;

	void Start(){
		this.iron_respawn_points = new List<Vector3> ();
		GameObject[] iron_respawns = GameObject.FindGameObjectsWithTag ("IronRespawn");
		foreach (GameObject go in iron_respawns) {
			MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer> ();
			if (renderer != null) {
				renderer.enabled = false;
			}
			this.iron_respawn_points.Add (go.transform.position);
		}

		this.apple_respawn_points = new List<Vector3> ();
		GameObject[] apple_respawns = GameObject.FindGameObjectsWithTag ("AppleRespawn");
		foreach (GameObject go in apple_respawns) {
			MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer> ();
			if (renderer != null) {
				renderer.enabled = false;
			}
			this.apple_respawn_points.Add (go.transform.position);
		}

		this.respawnIron ();
		this.respawnApple ();
	}

	void Update(){
	}

	public void respawnApple(){
		for (int i = 0; i < apple_respawn_points.Count; i++) {
			GameObject go = GameObject.Instantiate (this.applePrefab) as GameObject;
			//			int n = Random.Range (0, this.respawn_points.Count);
			Vector3 pos = this.apple_respawn_points [i];
			//pos.y = 3.0f;
			//			pos.x += Random.Range (-1.0f, 1.0f);
			//			pos.z += Random.Range (-1.0f, 1.0f);
			go.transform.position = pos;
		}
		//		}
	}

	public void respawnIron(){
		for (int i = 0; i < iron_respawn_points.Count; i++) {
			GameObject go = GameObject.Instantiate (this.ironPrefab) as GameObject;
//			int n = Random.Range (0, this.respawn_points.Count);
			Vector3 pos = this.iron_respawn_points [i];
			//pos.y = 3.0f;
//			pos.x += Random.Range (-1.0f, 1.0f);
//			pos.z += Random.Range (-1.0f, 1.0f);
			go.transform.position = pos;
		}
//		}
	}

	public Item.TYPE getItemType(GameObject item_go){
		Item.TYPE type = Item.TYPE.NONE;
		if (item_go != null) {
			switch (item_go.tag) {
			case"Iron":
				type = Item.TYPE.IRON;
				break;
			case"Apple":
				type = Item.TYPE.APPLE;
				break;
			}
		}
		return(type);
	}

	public float getGainRepairment(GameObject item_go){
		float gain = 0.0f;
		if (item_go == null) {
			gain = 0.0f;
		} else {
			Item.TYPE type = this.getItemType (item_go);
			switch (type) {
			case Item.TYPE.IRON:
				gain = GameStatus.GAIN_REPAIRMENT_IRON;
				break;
			}
		}
		return(gain);
	}

	public float getConsumeSatiety(GameObject item_go){
		float consume = 0.0f;
		if (item_go == null) {
			consume = 0.0f;
		} else {
			Item.TYPE type = this.getItemType (item_go);
			switch (type) {
			case Item.TYPE.IRON:
				consume = GameStatus.CONSUME_SATIETY_IRON;
				break;
			case Item.TYPE.APPLE:
				consume = GameStatus.CONSUME_SATIETY_APPLE;
				break;
			}
		}
		return(consume);
	}

	public float getRegainSatiety(GameObject item_go){
		float regain = 0.0f;
		if (item_go == null) {
			regain = 0.0f;
		} else {
			Item.TYPE type = this.getItemType (item_go);
			switch (type) {
			case Item.TYPE.APPLE:
				regain = GameStatus.REGAIN_SATIETY_APPLE;
				break;
			}
		}
		return(regain);
	}
}
