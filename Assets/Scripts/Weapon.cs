using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	private Rigidbody2D rbody;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;

	public Sprite floored;
	public Sprite handled;
	public GameObject ammo;
	public AudioSource shootAudio;
	public bool isKnife = false;

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

	public void Shoot (GameObject shooter, bool isPlayer) {
		GameObject go = Instantiate (ammo, transform.position, transform.rotation * Quaternion.Euler(0, 0, 270)) as GameObject;
		Physics2D.IgnoreCollision(shooter.GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
		go.GetComponent<Ammo>().shotByPlayer = true;
		shootAudio.Play ();
		Destroy (go, isKnife ? 0.1f : 10.0f);
	}
}
