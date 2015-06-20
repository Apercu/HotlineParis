using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Animator legs;
	private Rigidbody2D rbody;
	private Vector3 acceleration = Vector3.zero;
	private bool isMoving = false;

	public float speed = 8.0f;

	void Start () {
		legs = gameObject.transform.FindChild("legs").GetComponent<Animator>();
		rbody = gameObject.GetComponent<Rigidbody2D>();
	}

	void Update () {
		acceleration += Vector3.up * Input.GetAxis("Vertical");
		acceleration += Vector3.right * Input.GetAxis("Horizontal");
		isMoving = (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

		legs.SetBool("isWalking", (rbody.velocity.magnitude > 0.0f));

		Vector3 mousePos = Input.mousePosition;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		
		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
	}

	void FixedUpdate () {
		Vector3.Normalize(acceleration);
		acceleration *= speed;
		rbody.velocity = acceleration;
		acceleration = Vector3.zero;
	}
}
