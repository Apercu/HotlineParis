using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Animator legs;
	private Rigidbody2D rbody;
	private Vector3 acceleration = Vector3.zero;

	public float speed = 8.0f;

	void Start () {
		legs = gameObject.transform.FindChild("legs").GetComponent<Animator>();
		rbody = gameObject.GetComponent<Rigidbody2D>();
	}

	void Update () {

		acceleration += Vector3.up * Input.GetAxis("Vertical");
		acceleration += Vector3.right * Input.GetAxis("Horizontal");

		legs.SetBool("isWalking", (rbody.velocity.magnitude > 0.0f));

		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - mouse);
	}

	void FixedUpdate () {

		Vector3.Normalize(acceleration);
		acceleration *= speed;
		rbody.velocity = acceleration;
		acceleration = Vector3.zero;

	}
}
