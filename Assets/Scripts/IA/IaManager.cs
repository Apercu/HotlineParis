using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IaManager : MonoBehaviour {
	
	public float speed = 3.0f;
	public float viewAngle = 100.0f;

	private Vector3 initPosition;
	private CheckPoint checkPoint;
	private CheckPoint firstCheckPoint;
	private bool hasCheckPoints = false;
	private bool isWalking;
	private bool isLooking;

	private bool isGoingToBercail = false;
	private bool hasMoved = false;
	private bool chasing = false;
	private bool hasPlayerInSight = false;
	private Vector3 lastKnowPosition;
	private float lastTimeKnown = -1.0f;
	
	private Animator legs;
	private Animator head;
	private SpriteRenderer alert;
	private PolygonCollider2D sightCollider;
	private List<Vector3> paths = new List<Vector3>();

	private float nextTime = 0.0f;
	private int layerWithoutEnnemies = 1 << 12;

	void Start () {

		if (transform.parent.FindChild("CheckPoints/1")) {
			checkPoint = transform.parent.FindChild("CheckPoints/1").GetComponent<CheckPoint>();
			firstCheckPoint = checkPoint;
			hasCheckPoints = true;
		}

		layerWithoutEnnemies = ~layerWithoutEnnemies;
		
		legs = transform.FindChild("legs").GetComponent<Animator>();
		head = transform.FindChild("head").GetComponent<Animator>();
		alert = transform.FindChild("alert").GetComponent<SpriteRenderer>();

		sightCollider = GetComponent<PolygonCollider2D>();
		initPosition = transform.position;
		isWalking = hasCheckPoints;
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

	IEnumerator blinkAlert () {
		yield return new WaitForSeconds(6);
		for (int i = 0; i < 4; ++i) {
			alert.color = Color.white;
			yield return new WaitForSeconds(0.5f);
			alert.color = Color.clear;
			yield return new WaitForSeconds(0.5f);
		}
	}

	void Update () {

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
			}
		}

		// Chase mode active, IA gonna search the human at all costs.
		if (chasing) {
			hasMoved = true;
			Vector3 pos = lastKnowPosition;

			Vector3 vectorPlayer = PlayerManager.instance.transform.position;
			Vector3 dir = vectorPlayer - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerWithoutEnnemies);

			// On a retrouve le player
			if (hit.collider.tag == "Player") {
				moveTo(vectorPlayer);
				transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - vectorPlayer);
				lastKnowPosition = vectorPlayer;
				isWalking = true;
				isLooking = false;
				lastTimeKnown = -1.0f;
				alert.color = Color.white;
			} else {
				// On l'a pas eu, mais on va a la derniere position pour checker
				if (Vector3.Distance(transform.position, pos) > .1f) {
					moveTo(pos);
					transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - pos);
					isWalking = true;
					isLooking = false;
					lastTimeKnown = -1.0f;
					alert.color = Color.white;
				} else {
					// Passage en mode recherche
					isWalking = false;
					isLooking = true;
					if (lastTimeKnown == -1.0f) {
						lastTimeKnown = Time.time;
						StartCoroutine(blinkAlert());
					}
				}
			}
		}

		// Pendant une chase toute les secondes on ajoute la position dans la liste
		if (!isGoingToBercail && isWalking && chasing && Time.time > nextTime) {
			paths.Add(transform.position);
			nextTime += 1.0f;
		}

		// Apres 10secs on arrete de chercher comme des fous.
		if (!isGoingToBercail && lastTimeKnown != -1.0f && Time.time - lastTimeKnown > 10.0f) {
			chasing = false;
			isLooking = false;
			alert.color = Color.clear;
			if (!isGoingToBercail) {
				speed *= 2.0f;	
			}
			isGoingToBercail = true;
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

		legs.SetBool("isWalking", isWalking);
		head.SetBool("isLooking", isLooking);
	}

	void OnTriggerStay2D (Collider2D obj) {
		if (!chasing && !isGoingToBercail && obj.tag == "Player") {
			hasPlayerInSight = false;

			Vector3 dir = obj.transform.position - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerWithoutEnnemies);
			if (hit.collider.tag == "Player") {
				hasPlayerInSight = true;
				lastKnowPosition = obj.transform.position;
				chasing = true;
			}
		}
	}

	void OnTriggerExit2D (Collider2D obj) {
		if (obj.tag == "Player") {

			hasPlayerInSight = false;
		}
	}
}