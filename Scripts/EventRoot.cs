using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event{
	public enum TYPE{
		NONE = -1,
		SPACESHIP = 0,
		GUIDE,
		NUM,
	};
};

public class EventRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Event.TYPE getEventType(GameObject event_go){
		Event.TYPE type = Event.TYPE.NONE;

		if (event_go != null) {
			if (event_go.tag == "SpaceShip") {
				type = Event.TYPE.SPACESHIP;
			}
			if (event_go.tag == "Guide") {
				type = Event.TYPE.GUIDE;
			}
		}
		return(type);
	}

	public bool isEventIgnitable(Item.TYPE getItem, GameObject event_go){
		bool ret = false;
		Event.TYPE type = Event.TYPE.NONE;

		if (event_go != null) {
			type = this.getEventType (event_go);
		}

		switch (type) {
		case Event.TYPE.SPACESHIP:
			if (getItem == Item.TYPE.IRON) {
				ret = true;
			}
			break;
		}
		return(ret);
	}

	public bool isGuideIgnitable(GameObject event_go){
		bool ret = false;
		Event.TYPE type = Event.TYPE.NONE;

		if (event_go != null) {
			type = this.getEventType (event_go);
		}

		switch (type) {
		case Event.TYPE.GUIDE:
			ret = true;
			break;
		}

		return(ret);
	}

	public string getIgnitableMessage(GameObject event_go){
		string message = "";
		Event.TYPE type = Event.TYPE.NONE;
		if (event_go != null) {
			type = this.getEventType (event_go);
		}
		switch (type) {
		case Event.TYPE.SPACESHIP:
			message = "REPAIR";
			break;
		case Event.TYPE.GUIDE:
			message = " 선장님 제 옆의 모양 광석 3개만  \n" +
				" 더 모아 우주선에 넣어주시면   \n" +
				" 우주선을 수리 하실 수 있습니다!  \n" +
				" 체력이 모자라시면 사과를 드시면 \n" +
				" 회복할 수 있습니다.";
			break;
		}
		return(message);
	}
}
