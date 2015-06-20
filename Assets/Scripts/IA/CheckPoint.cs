using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	public CheckPoint next;
		
	void Start () {
		Transform go = gameObject.transform.parent.FindChild((int.Parse (gameObject.name) + 1).ToString());
		if (go) {
			next = go.GetComponent<CheckPoint>();
		}
	}

	void OnDrawGizmos () {
		if (next) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, next.GetComponent<Transform>().position);
		}
	}
}
