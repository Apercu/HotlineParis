using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour {

	public bool isKnife;
	public float speed = 500.0f;

	private Rigidbody2D rbody;
	
	[HideInInspector]public bool ignoreEnnemies = false;
	[HideInInspector]public bool shotByPlayer = false;

	void Start () {
		rbody = GetComponent<Rigidbody2D> ();
		if (shotByPlayer) {
			gameObject.tag = "AmmoPlayer";
		}
	}
	
	void Update() {
	}
	
	void FixedUpdate() {
		if (!isKnife) {
			rbody.velocity = transform.right * Time.deltaTime * speed;
		}
	}

	void OnCollisionEnter2D (Collision2D obj) {
		if (obj.collider.tag == "Player") {
			obj.collider.GetComponent<PlayerManager>().Die();
		} else if (obj.collider.tag == "Ennemy" && !ignoreEnnemies) {
			obj.collider.GetComponent<IaManager>().die();
		}
		Destroy (gameObject);
	}
}
