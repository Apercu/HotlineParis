using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	private Rigidbody2D rbody;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;

	public Sprite floored;
	public Sprite handled;
	public GameObject ammo;

	void Awake () {
		rbody = GetComponent<Rigidbody2D> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		if (rbody.velocity.magnitude < 1.0f) {
			boxCollider.isTrigger = true;
		}
	}

	public void AttachToBody () {
		spriteRenderer.sprite = handled;
		spriteRenderer.sortingLayerName = "Player";
	}
	
	public void Drop () {
		spriteRenderer.sprite = floored;
		spriteRenderer.sortingLayerName = "Objects";
	}

	public void Shoot () {
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 pos = (mouse - transform.position).normalized;
		Instantiate (ammo, transform.position + 1.5f * pos, transform.rotation * Quaternion.Euler(0, 0, 270));
	}
}
