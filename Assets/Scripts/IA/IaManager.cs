using UnityEngine;
using System.Collections;

public class IaManager : MonoBehaviour {
	
	public float speed = 3.0f;
	public float viewAngle = 100.0f;

	private Vector3 initPosition;
	private CheckPoint checkPoint;
	private CheckPoint firstCheckPoint;
	private bool hasCheckPoints = false;
	private bool isWalking;
	private bool isLooking;

	private bool hasMoved = false;
	private bool chasing = false;
	private bool hasPlayerInSight = false;
	private Vector3 lastKnowPosition;
	private float lastTimeKnown = -1.0f;
	
	private Animator legs;
	private Animator head;
	private SpriteRenderer alert;
	private PolygonCollider2D sightCollider;

	void Start () {

		if (transform.parent.FindChild("CheckPoints/1")) {
			checkPoint = transform.parent.FindChild("CheckPoints/1").GetComponent<CheckPoint>();
			firstCheckPoint = checkPoint;
			hasCheckPoints = true;
		}
		
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

		// Chase mode active, IA gonna search the human at all costs.
		if (chasing) {
			hasMoved = true;
			Vector3 pos = lastKnowPosition;
				
			int layerMask = 1 << 12;
			layerMask = ~layerMask;

			Vector3 vectorPlayer = PlayerManager.instance.transform.position;
			Vector3 dir = vectorPlayer - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);

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
				if (Vector3.Distance(transform.position, pos) > .01f) {
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

		// Apres 10secs on arrete de chercher comme des fous.
		if (lastTimeKnown != -1.0f && Time.time - lastTimeKnown > 10.0f) {
			chasing = false;
			hasMoved = true;
			isLooking = false;
			alert.color = Color.clear;
		}

		if (!chasing && hasCheckPoints && !hasMoved) {

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

	void OnTriggerEnter2D (Collider2D obj) {
		if (obj.tag == "Player") {
			hasPlayerInSight = false;

			int layerMask = 1 << 12;
			layerMask = ~layerMask;

			Vector3 dir = obj.transform.position - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
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

	/*


	void Update ()
	{
		// If the last global sighting of the player has changed...
		if (lastPlayerSighting.position != previousSighting)
			// ... then update the personal sighting to be the same as the global sighting.
			personalLastSighting = lastPlayerSighting.position;
		
		// Set the previous sighting to the be the sighting from this frame.
		previousSighting = lastPlayerSighting.position;
		
		// If the player is alive...
		if(playerHealth.health > 0f)
			// ... set the animator parameter to whether the player is in sight or not.
			anim.SetBool(hash.playerInSightBool, playerInSight);
		else
			// ... set the animator parameter to false.
			anim.SetBool(hash.playerInSightBool, false);
	}
	*/
}