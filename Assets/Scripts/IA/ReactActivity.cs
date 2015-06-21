using UnityEngine;
using System.Collections;

public class ReactActivity : MonoBehaviour {

	private IaManager ai;

	void Start () {
		ai = transform.parent.FindChild("Content").GetComponent<IaManager>();
	}

	void Update () {
	
	}

	void OnTriggerStay2D (Collider2D obj) {
		if (obj.tag == "AmmoPlayer") {
			ai.chasing = true;
		}
	}

	void OnTriggerExit2D (Collider2D obj) {
		if (obj.tag == "AmmoPlayer") {
			// Do something, or not?
		}
	}
}
