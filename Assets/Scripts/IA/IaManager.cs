using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IaManager : MonoBehaviour {
	
	public float speed = 3.0f;
	public float viewAngle = 100.0f;

	public AudioSource dieAudio;
	public bool noBalls = false;
	 
	// Weapons Handle
	public List<Weapon> weapons;
	public float attackSpeed = 1.0f;
	private Weapon currentWeapon;
	private GameObject holder;

	// Tracking and Positionning
	private Vector3 initPosition;
	private CheckPoint checkPoint;
	private CheckPoint firstCheckPoint;
	private bool hasCheckPoints = false;
	private bool isWalking;
	private bool isLooking;

	private bool isGoingToBercail = false;
	private bool hasMoved = false;
	[HideInInspector] public bool chasing = false;
	private bool hasPlayerInSight = false;
	private Vector3 lastKnowPosition = Vector3.zero;
	private float lastTimeKnown = -1.0f;

	private Coroutine shootRoutine;
	private Coroutine blinkRoutine;
	private ParticleSystem blood;
	private Animator legs;
	private Animator body;
	private SpriteRenderer alert;
	private List<Vector3> paths = new List<Vector3>();

	private float nextTime = 0.0f;
	private float shootTime = 0.0f;
	private int layerWithoutEnnemies = ~((1 << 12) | (1 << 10) | (1 << 11));

	void Start () {

		if (transform.parent.FindChild("CheckPoints/1")) {
			checkPoint = transform.parent.FindChild("CheckPoints/1").GetComponent<CheckPoint>();
			firstCheckPoint = checkPoint;
			hasCheckPoints = true;
		}

		currentWeapon = weapons[Random.Range(0, weapons.Count)];
		holder = transform.FindChild("gunHolder").gameObject;
		holder.GetComponent<SpriteRenderer>().sprite = currentWeapon.handled;

		legs = transform.FindChild("legs").GetComponent<Animator>();
		body = GetComponent<Animator>();
		alert = transform.FindChild("alert").GetComponent<SpriteRenderer>();
		blood = GetComponent<ParticleSystem> ();

		initPosition = transform.position;
		isWalking = hasCheckPoints;

		GameManager.instance.addEnnemy(this);
	}

	void moveTo (Vector3 targetPosition) {
		Vector3 currentPosition = transform.position;

		Vector3 directionOfTravel = targetPosition - currentPosition;
		directionOfTravel.Normalize();
		transform.Translate(
			(directionOfTravel.x * speed * Time.deltaTime),
			(directionOfTravel.y * speed * Time.deltaTime),
			(directionOfTravel.z * speed * Time.deltaTime), Space.World);
	}

	public void die () {
		if (body.GetBool ("isDead")) { return; }

		body.SetBool("isDead", true);
		legs.SetBool("isWalking", false);
		body.SetBool("isLooking", false);
		dieAudio.Play ();
		blood.Play ();
		gameObject.layer = 11;
		gameObject.transform.FindChild("body").GetComponent<SpriteRenderer>().sortingLayerName = "Background"; gameObject.transform.FindChild("body").GetComponent<SpriteRenderer>().sortingOrder = 100;
		gameObject.transform.FindChild("head").GetComponent<SpriteRenderer>().sortingLayerName = "Background"; gameObject.transform.FindChild("head").GetComponent<SpriteRenderer>().sortingOrder = 100;
		gameObject.transform.FindChild("legs").GetComponent<SpriteRenderer>().sortingLayerName = "Background"; gameObject.transform.FindChild("legs").GetComponent<SpriteRenderer>().sortingOrder = 100;
		gameObject.transform.FindChild("alert").GetComponent<SpriteRenderer>().enabled = false;
		Destroy(GetComponent<Collider2D>());
		GameManager.instance.killEnnemy(this);
		holder.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(pauseBlood());
	}

	public void stun () {
		StartCoroutine (doStun());
	}

	IEnumerator doStun () {
		isLooking = false;
		body.SetBool("isStun", true);
		yield return new WaitForSeconds(3);
		body.SetBool("isStun", false);
	}

	void shoot () {
		GameObject go = Instantiate(currentWeapon.ammo, transform.position, transform.rotation * Quaternion.Euler(0, 0, 270)) as GameObject;
		go.GetComponent<Ammo>().ignoreEnnemies = true;
		Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
		AudioSource.PlayClipAtPoint (currentWeapon.shootAudio.clip, transform.position);
		Destroy (go, go.GetComponent<Ammo>().isKnife ? 0.1f : 10.0f);
	}

	IEnumerator startBlood () {
		yield return new WaitForSeconds (0.001f);
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[blood.maxParticles];
		int bloodLength = blood.GetParticles (particles); 
		Debug.Log (bloodLength + " " + blood.maxParticles);
		for (int i = 0; i < bloodLength; ++i) {
			particles[i].velocity += Vector3.up * 100000.01f;
		}
		blood.SetParticles (particles, bloodLength);
	}

	IEnumerator pauseBlood () {
		yield return new WaitForSeconds(0.6f);
		blood.Pause();
	}

	IEnumerator blinkAlert () {
		alert.color = Color.white;
		yield return new WaitForSeconds(6);
		for (int i = 0; i < 4; ++i) {
			alert.color = Color.white;
			yield return new WaitForSeconds(0.5f);
			alert.color = Color.clear;
			yield return new WaitForSeconds(0.5f);
		}
		isLooking = false;
	}

	void Update () {

		if (body.GetBool("isStun") || body.GetBool("isDead") || GameManager.instance.isDead) {
			legs.SetBool("isWalking", false);
			return ;
		}

		isWalking = false;

		// Get to the bercail
		if (isGoingToBercail) {
			// Si il nous reste des paths pour revenir
			if (paths.Count > 0) {

				if (paths.Count > 1) {
					// Si ca raycast avec notre origine on vire tout et on y va direct
					if (!Physics2D.Linecast(transform.position, paths[0], layerWithoutEnnemies)) {
						paths.RemoveRange(1, paths.Count - 1);
					}
				}

				if (Vector3.Distance(transform.position, paths[paths.Count - 1]) > .1f) {
					moveTo(paths[paths.Count - 1]);
					transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - paths[paths.Count - 1]);
				} else {
					paths.RemoveAt(paths.Count - 1);
				}

			} else {
				// Sinon on a fini de retourner a la position originelle
				nextTime = 0.0f;
				hasMoved = false;
				speed /= 2.0f;
				chasing = false;
				lastTimeKnown = -1.0f;
				isGoingToBercail = false;
				isWalking = false;
			}
		}

		if (hasPlayerInSight && noBalls) {
			transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - PlayerManager.instance.transform.position);
		}

		// Chase mode active, IA gonna search the human at all costs.
		if (!noBalls && chasing) {
			hasMoved = true;

			Vector3 vectorPlayer = PlayerManager.instance.transform.position;
			Vector3 dir = vectorPlayer - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerWithoutEnnemies);

			// On a retrouve le player
			if (hit.collider.tag == "Player") {
				bool hasKnife = currentWeapon.GetComponent<Weapon>().isKnife;
				if (!hasKnife && Vector3.Distance(transform.position, vectorPlayer) > 2f
				    || hasKnife && Vector3.Distance(transform.position, vectorPlayer) > 0.5f) {
					moveTo(vectorPlayer);
					isWalking = true;
				}
				transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - vectorPlayer);
				lastKnowPosition = vectorPlayer;
				isLooking = false;
				lastTimeKnown = -1.0f;
				alert.color = Color.white;
			} else {
				// On l'a pas eu, mais on va a la derniere position pour checker
				if (lastKnowPosition != Vector3.zero && Vector3.Distance(transform.position, lastKnowPosition) > 1.0f) {
					moveTo(lastKnowPosition);
					transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - lastKnowPosition);
					isWalking = true;
					isLooking = false;
					lastTimeKnown = -1.0f;
				} else {
					// Passage en mode recherche
					isWalking = false;
					isLooking = true;
					alert.color = Color.white;
					if (lastTimeKnown == -1.0f) {
						lastTimeKnown = Time.time;
						if (blinkRoutine != null) {
							StopCoroutine(blinkRoutine);
						}
						blinkRoutine = StartCoroutine(blinkAlert());
					}
				}
			}
		}

		// Pendant une chase toute les secondes on ajoute la position dans la liste
		if (!noBalls && !isGoingToBercail && isWalking && chasing && Time.time > nextTime) {
			paths.Add(transform.position);
			nextTime += 1.0f;
		}

		// Apres 10secs on arrete de chercher comme des fous.
		if (!noBalls && !isGoingToBercail && lastTimeKnown != -1.0f && Time.time - lastTimeKnown > 10.0f) {
			chasing = false;
			isLooking = false;
			alert.color = Color.clear;
			lastTimeKnown = -1.0f;
			if (!isGoingToBercail) {
				speed *= 2.0f;	
			}
			isGoingToBercail = true;
			lastKnowPosition = Vector3.zero;
		}

		// Suit ses checkPoints
		if (!chasing && hasCheckPoints && !hasMoved && !isGoingToBercail) {

			Vector3 targetPosition = checkPoint ? checkPoint.GetComponent<Transform>().position : initPosition;

			if (!hasPlayerInSight) {
				transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - targetPosition);
			}

			if (Vector3.Distance(transform.position, targetPosition) > .1f) { 
				moveTo(targetPosition);
			} else {
				if (!checkPoint) {
					checkPoint = firstCheckPoint;
				} else {
					checkPoint = (checkPoint.next) ? checkPoint.next : null;
				}
			}

			isWalking = true;
		}

		if (hasPlayerInSight && !isGoingToBercail) {
			if (shootTime == 0.0f || (shootTime > currentWeapon.GetComponent<Weapon>().fireRate / 2.0f)) {
				shoot();
				shootTime = 0.0f;
			}
			shootTime += Time.deltaTime;
		}

		legs.SetBool("isWalking", isWalking);
		body.SetBool("isLooking", isLooking);
	}

	void OnTriggerStay2D (Collider2D obj) {
		if (!isGoingToBercail && obj.tag == "Player") {

			Vector3 dir = obj.transform.position - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerWithoutEnnemies);
			if (hit.collider.tag == "Player") {
				hasPlayerInSight = true;
				lastKnowPosition = obj.transform.position;
				chasing = true;
			} else {
				hasPlayerInSight = false;
			}
		}
	}

	void OnTriggerExit2D (Collider2D obj) {
		if (obj.tag == "Player") {
			hasPlayerInSight = false;
			shootTime = 0.0f;
		}
	}
}