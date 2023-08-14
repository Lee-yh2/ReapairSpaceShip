using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class conrol : MonoBehaviour {
	public static float MOVE_AREA_RADIUS = 15.0f;
	public static float MOVE_SPEED = 7.0f;
	private GameObject seeItem = null;
	private GameObject getItem = null;
	private ItemRoot item_root = null;
	private GameObject closet_event = null;
	private EventRoot event_root = null;
	private GameStatus game_status = null;
	public GUIStyle guistyle;
	public Animator animator;
	Vector3 moveDirection = Vector3.zero;
	private AudioSource audio;
	public AudioClip pickUpSound;
	public AudioClip dropSound;
	public AudioClip repairSound;
	public AudioClip repairSoundre;
	public AudioClip clearSound;
	public AudioClip deadSound;
	public AudioClip eatSound;
	public AudioClip jumpSound;
	Rigidbody rb;
	private float v;
	private int jumpCount;
	private bool isGround;

	private struct Key{
		public bool up;
		public bool down;
		public bool right;
		public bool left;
		public bool jump;
		public bool pick;
		public bool action;
	}
	private Key key;

	public enum STEP{
		NONE = -1,
		MOVE = 0,
		REPAIRING,
		EATING,
		NUM,
	};

	public STEP step = STEP.NONE;
	public STEP next_step = STEP.NONE;
	public float step_timer = 0.0f;

	void Start () {
		jumpCount = 1;
		isGround = true;
		rb = GetComponent<Rigidbody>();
		this.audio = this.gameObject.AddComponent<AudioSource> ();
		animator = GetComponent<Animator>();
		this.step = STEP.NONE;
		this.next_step = STEP.MOVE;
		this.item_root = GameObject.Find ("GameRoot").GetComponent<ItemRoot> ();
		this.guistyle.fontSize = 20;
		this.guistyle.normal.textColor = Color.white;

		this.event_root = GameObject.Find ("GameRoot").GetComponent<EventRoot> ();
		this.game_status = GameObject.Find ("GameRoot").GetComponent<GameStatus> ();
	}

	void Update () {
		this.get_input();
		this.step_timer += Time.deltaTime;
		float eat_time = 1.5f;
		float repair_time = 1.5f;

		if (this.next_step == STEP.NONE) {
			switch(this.step){
			case STEP.MOVE:
				do {
					if (!this.key.action) {
						break;
					}

					if(this.closet_event != null){
						if(! this.is_event_ignitable()){
							break;
						}

						Event.TYPE ignitable_event = this.event_root.getEventType(this.closet_event);
						switch(ignitable_event){
						case Event.TYPE.SPACESHIP:
							this.next_step = STEP.REPAIRING;
							break;
						}
						break;
					}

					if (this.getItem != null) {
						Item.TYPE getItem_type = this.item_root.getItemType (this.getItem);
						switch (getItem_type) {
						case Item.TYPE.APPLE:
							this.next_step = STEP.EATING;
							break;
						}
					}
				} while(false);
				break;

			case STEP.EATING:
				if (this.step_timer > eat_time) {
					this.next_step = STEP.MOVE;
				}
				break;
			case STEP.REPAIRING:
				if (this.step_timer > repair_time) {
					this.next_step = STEP.MOVE;
				}
				break;
			}
		}

		while (this.next_step != STEP.NONE) {
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch (this.step) {
			case STEP.MOVE:
				break;
			case STEP.EATING:
				if (this.getItem != null) {
					this.audio.clip = this.eatSound;
					//this.audio.loop = false;
					this.audio.Play();
					this.game_status.addSatiety (this.item_root.getRegainSatiety (this.getItem));
					GameObject.Destroy (this.getItem);
					this.getItem = null;
				}
				break;
			case STEP.REPAIRING:
				if (this.getItem != null) {
					this.game_status.addRepairment (this.item_root.getGainRepairment (this.getItem));
					GameObject.Destroy (this.getItem);
					this.getItem = null;
					this.seeItem = null;
				}
				break;
			}
			this.step_timer = 0.0f;
		}
		switch (this.step) {
		case STEP.MOVE:
			this.move_control ();
			this.pickOrDrop ();
			if (game_status.satiety <= 0.0f) {
				this.audio.clip = this.deadSound;
				this.audio.Play();
			}
			break;
		case STEP.REPAIRING:
			if (game_status.repairment > 0.8f) {
				this.audio.clip = this.clearSound;
			} else {
				this.audio.clip = this.repairSound;
				this.audio.Play();
				this.audio.clip = this.repairSoundre;
				this.audio.Play();
			}
			//this.audio.loop = false;
			this.audio.Play();
//			this.space_ship.transform.localRotation *= Quaternion.AngleAxis (360.0f / 10.0f * Time.deltaTime, Vector3.up);
			break;
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Ground") {
			isGround = true;
			jumpCount = 1;
		}
	}

	void OnTriggerStay(Collider other){
		GameObject other_go = other.gameObject;

		if (other_go.layer == LayerMask.NameToLayer ("Item")) {
			if (this.seeItem == null) {
				if (this.isInView (other_go)) {
					this.seeItem = other_go;
				}
			} else if (this.seeItem == other_go) {
				if (!this.isInView (other_go)) {
					this.seeItem = null;
				}
			}
		} else if (other_go.layer == LayerMask.NameToLayer ("Event")) {
			if (this.closet_event == null) {
				if (this.isInView (other_go)) {
					this.closet_event = other_go;
				}
			} else if (this.closet_event == other_go) {
				if (!this.isInView (other_go)) {
					this.closet_event = null;
				}
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (this.seeItem == other.gameObject) {
			this.seeItem = null;
		}
		this.closet_event = null;
	}

	void OnGUI(){
		float x = 20.0f;
		float y = Screen.height - 60.0f;
		this.guistyle.fontSize = 25;
		GUI.Box (new Rect (x+ 1350, y- 10, 140.0f, 50.0f), "");
		GUI.Label (new Rect (x+1370, y, 200.0f, 20.0f), "R : RESTART", guistyle);

		if (this.getItem != null) {
			this.guistyle.fontSize = 25;
			GUI.Box (new Rect (x, y- 10, 130.0f, 50.0f), "");
				GUI.Label (new Rect (x+20, y, 200.0f, 20.0f), "Z : drop", guistyle);
				do {
					if (this.is_event_ignitable()) {
						break;
					}
					if (item_root.getItemType (this.getItem) == Item.TYPE.IRON) {
						break;
					}
				GUI.Box (new Rect (x+140, y- 10, 130.0f, 50.0f), "");
				this.guistyle.fontSize = 25;
					GUI.Label (new Rect (x + 160.0f, y, 200.0f, 20.0f), "X : eat", guistyle);
				} while(false);

		} else {
			if (this.seeItem != null) {
				if (item_root.getItemType (this.seeItem) == Item.TYPE.APPLE
				   || item_root.getItemType (this.seeItem) == Item.TYPE.IRON) {
					GUI.Box (new Rect (x, y- 10, 130.0f, 50.0f), "");
					this.guistyle.fontSize = 25;
					GUI.Label (new Rect (x+20, y, 200.0f, 20.0f), "Z : pickup", guistyle);
				}
			}
		}

		switch (this.step) {
		case STEP.EATING:
			this.guistyle.fontSize = 30;
			animator.SetTrigger("eat");
			GUI.Label (new Rect (x+550, y- 300, 200.0f, 20.0f), "냠냠냠...", guistyle);
			break;
		case STEP.REPAIRING:
			//animator.SetTrigger("eat");
			this.guistyle.fontSize = 30;
			GUI.Label (new Rect (x+550, y- 300, 200.0f, 20.0f), "뚝딱뚝딱...", guistyle);
			GUI.Label (new Rect (x, y, 200.0f, 30.0f), "수리중...", guistyle);
			break;
		}

		if (this.is_event_ignitable ()) {
			string message = this.event_root.getIgnitableMessage (this.closet_event);
			this.guistyle.fontSize = 25;
			GUI.Box (new Rect (x+180, y- 10, 130.0f, 50.0f), "");
			GUI.Label (new Rect (x+ 200.0f, y, 200.0f, 20.0f), "X : " + message, guistyle);
		}
		if (this.is_guide_ignitable ()) {
			string message = this.event_root.getIgnitableMessage (this.closet_event);
			this.guistyle.fontSize = 40;
			GUI.Box (new Rect (x+20, y- 410, 640.0f, 300.0f), "");
			GUI.Label (new Rect (x+ 40.0f, y-380.0f, 500.0f, 50.0f), message, guistyle);
		}

	}

	private void pickOrDrop(){
			do {
				if (!this.key.pick) {
					break;
				}
				if (this.getItem == null) {
					if (this.seeItem == null) {
						break;
					}
					//animator.SetTrigger("tr_pickup");
					//animator.Play("Pickup");
				    this.audio.clip = this.pickUpSound;
				    //this.audio.loop = false;
				    this.audio.Play();
			    	animator.SetTrigger ("tr_drop");
					this.getItem = this.seeItem;
					this.getItem.transform.parent = this.transform;
					this.getItem.transform.localPosition = Vector3.up * 2.5f;
					this.seeItem = null;
				} else {
				this.audio.clip = this.dropSound;
				//this.audio.loop = false;
				this.audio.Play();
					animator.SetTrigger ("tr_drop");
				//	this.getItem.transform.localPosition = Vector3.forward * 1.0f;
				this.getItem.transform.localPosition = Vector3.up * 1.0f;
					this.getItem.transform.parent = null;
					this.getItem = null;
				}
			} while(false);
	}

	private bool isInView(GameObject other){
		bool view = false;
		do {
			Vector3 heading = this.transform.TransformDirection (Vector3.forward);
			Vector3 to_other = other.transform.position - this.transform.position;
			heading.y = 0.0f;
			to_other.y = 0.0f;
			heading.Normalize ();
			to_other.Normalize ();
			float dp = Vector3.Dot (heading, to_other);
			if (dp < Mathf.Cos (45.0f)) {
				break;
			}
			view = true;
		} while(false);
		return view;
	}

	private bool is_event_ignitable(){
		bool ret = false;
		do {
			if (this.closet_event == null) {
				break;
			}
			Item.TYPE get_item_type = this.item_root.getItemType (this.getItem);

			if (!this.event_root.isEventIgnitable (get_item_type, this.closet_event)) {
				break;
			}
			ret = true;
		} while(false);
		return(ret);
	}

	private bool is_guide_ignitable(){
		bool ret = false;
		do {
			if (this.closet_event == null) {
				break;
			}

			if (!this.event_root.isGuideIgnitable (this.closet_event)) {
				break;
			}
			ret = true;
		} while(false);
		return(ret);
	}

	private void get_input(){
		this.key.up = false;
		this.key.down = false;
		this.key.right = false;
		this.key.left = false;
		this.key.jump = false;
        this.key.pick = false;


        this.key.up |= Input.GetKey (KeyCode.UpArrow);
		this.key.down |= Input.GetKey (KeyCode.DownArrow);
		this.key.right |= Input.GetKey (KeyCode.RightArrow);
		this.key.left |= Input.GetKey (KeyCode.LeftArrow);
		this.key.jump = Input.GetKeyDown(KeyCode.Space);
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
		this.key.action = Input.GetKeyDown(KeyCode.X);
	}

	private void move_control(){
		//v = Input.GetAxis ("Vertical");
		//Vector3 moveP = Vector3.zero;
		Vector3 position = this.transform.position;
		bool isMove = false;

		if(moveDirection.y > 15 * -1) {
			moveDirection.y -= 15 * Time.deltaTime;
		}
		if (this.key.up) {
			animator.SetBool ("run", true);
			this.transform.Translate (Vector3.forward * MOVE_SPEED * Time.deltaTime);
			isMove = true;
		} else {
			animator.SetBool ("run", false);
		}
		if (isGround) {
			if (this.key.right) {
				animator.SetBool ("right", true);
				this.transform.Rotate (0, MOVE_SPEED * 15 * Time.deltaTime, 0);
				isMove = true;
			} else {
				animator.SetBool ("right", false);
			}
			if (this.key.left) {
				animator.SetBool ("left", true);
				this.transform.Rotate (0, -MOVE_SPEED * 15 * Time.deltaTime, 0);
				isMove = true;
			} else {
				animator.SetBool ("left", false);
			}
			if (this.key.down) {
				animator.SetBool ("back", true);
				this.transform.Translate (Vector3.back * MOVE_SPEED * Time.deltaTime);
				isMove = true;
			} else {
				animator.SetBool ("back", false);
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadSceneAsync ("escape");
		}
		if (isGround) {
			jumpCount = 1;
			if (this.key.jump) {
				this.audio.clip = this.jumpSound;
				this.audio.Play ();
				animator.Play ("Jump", -1, 0);
				//animator.SetBool("jump", true);
				//animator.SetTrigger("jump0");
				rb.AddForce (0, 250f, 0);
				isGround = false;
				jumpCount = 0;
			}
		}
       // else {
		//	animator.SetBool("jump", false);
		//}

		//moveP.Normalize();
		//moveP *= MOVE_SPEED * Time.deltaTime;
		//position += moveP;
		//position.y = 0.0f;

		/*if (position.magnitude > MOVE_AREA_RADIUS) {
			position.Normalize ();
			position *= MOVE_AREA_RADIUS;
		}*/
		//animator.SetFloat("v", v);
		/*position.y = this.transform.position.y;
		this.transform.position = position;
		if (moveP.magnitude > 0.01f) {
			Quaternion q = Quaternion.LookRotation (moveP, Vector3.up);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, q, 0.1f);
		}*/

		if (isMove) {
//			this.audio.clip = this.walkSound;
//			this.audio.Play();
			float consume = this.item_root.getConsumeSatiety (this.getItem);
			this.game_status.addSatiety (-consume * Time.deltaTime);
		}
	}
}
