using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Animator legs;
	private Rigidbody2D rbody;

	public float speed = 100.0f;

	void Start () {
		legs = gameObject.transform.FindChild("legs").GetComponent<Animator>();
		rbody = gameObject.GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			rbody.AddForce(Vector3.up * speed);

		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			rbody.AddForce(Vector3.left * speed);
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			rbody.AddForce(Vector3.down * speed);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			rbody.AddForce(Vector3.right * speed);
		}
	}
}
