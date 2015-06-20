using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour {

	public bool isKnife;
	public float speed = 500.0f;

	private Rigidbody2D rbody;

	void Start () {
		rbody = GetComponent<Rigidbody2D> ();
	}
	
	void Update() {
	}
	
	void FixedUpdate() {
		if (!isKnife) {
			rbody.velocity = transform.right * Time.deltaTime * speed;
		} else {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D obj) {
		Destroy (gameObject);
	}
}
