using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	private Animator legs;
	private Rigidbody2D rbody;
	private Vector3 acceleration = Vector3.zero;
	private GameObject weaponHovered;
	private GameObject weapon;
	private GameObject gunHolder;
	private ParticleSystem blood;
	private Animator anim;

	public float speed = 8.0f;

	public static PlayerManager instance { get; private set; }

	void Awake () {
		instance = this;
	}

	void Start () {
		legs = gameObject.transform.FindChild("legs").GetComponent<Animator>();
		rbody = gameObject.GetComponent<Rigidbody2D>();

		gunHolder = transform.FindChild("gunHolder").gameObject;
		
		blood = GetComponent<ParticleSystem>();
		anim = GetComponent<Animator>();
	}

	void Update () {

		if (GameManager.instance.isDead) {
			legs.SetBool("isWalking", false);
			return ;
		}

		acceleration = Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");

		legs.SetBool("isWalking", (rbody.velocity.magnitude > 0.0f));

		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - mouse);

		if (Input.GetKeyDown (KeyCode.E) && weaponHovered && weapon == null) {
			PickWeapon ();
		}

		if (Input.GetMouseButtonDown (1) && weapon) {
			ThrowWeapon ();
		}

		if (Input.GetMouseButtonDown (0) && weapon) {
			weapon.GetComponent<Weapon>().Shoot(gameObject, true);
		}
	}

	void PickWeapon () {
		weapon = Instantiate(weaponHovered, gunHolder.transform.position, transform.rotation) as GameObject;
		weapon.GetComponent<Rigidbody2D>().isKinematic = true;
		weapon.transform.SetParent(gunHolder.transform);
		weapon.GetComponent<Weapon>().AttachToBody();
		weapon.tag = "Player";
		Destroy (weaponHovered);
	}

	void ThrowWeapon () {
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 pos = (mouse - transform.position).normalized;
		GameObject go = Instantiate (weapon, transform.position + pos, Quaternion.identity) as GameObject;
		Rigidbody2D rb = go.GetComponent<Rigidbody2D> ();

		Destroy (weapon);
		go.GetComponent<Weapon>().Drop();
		go.tag = "Weapon";
		rb.isKinematic = false;
		go.GetComponent<BoxCollider2D> ().isTrigger = false;
		rb.AddForce ((mouse - transform.position) * 8, ForceMode2D.Impulse);
		rb.AddTorque (5.0f, ForceMode2D.Impulse);
	}

	public void Die () {
		anim.SetBool("isDead", true);
		blood.Play();
		StartCoroutine(pauseBlood());
		Destroy (weapon);
		GameManager.instance.gameOver();
	}
	
	IEnumerator pauseBlood () {
		yield return new WaitForSeconds(0.6f);
		blood.Pause();
	}

	void FixedUpdate () {

		Vector3.Normalize(acceleration);
		acceleration *= speed;
		rbody.velocity = acceleration;
		acceleration = Vector3.zero;

	}

	void OnTriggerEnter2D(Collider2D obj) {
		if (obj.tag == "Weapon") {
			weaponHovered = obj.gameObject;
		}
		if (obj.tag == "Finish") {
			GameManager.instance.win();
		}
	}
	
	void OnTriggerExit2D(Collider2D obj) {
		if (obj.tag == "Weapon") {
			weaponHovered = null;
		}
	}
}
