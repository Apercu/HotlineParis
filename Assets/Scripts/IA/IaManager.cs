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

	private bool chasing = false;
	private bool hasPlayerInSight = false;
	private Vector3 playerPosition = Vector3.zero;
	private Vector3 lastKnowPosition;

	private Animator legs;
	private PolygonCollider2D sightCollider;

	void Start () {

		if (transform.parent.FindChild("CheckPoints/1")) {
			checkPoint = transform.parent.FindChild("CheckPoints/1").GetComponent<CheckPoint>();
			firstCheckPoint = checkPoint;
			hasCheckPoints = true;
		}

		legs = transform.FindChild("legs").GetComponent<Animator>();

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

	void Update () {

		if (chasing) {
			transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - playerPosition);
			Vector3 pos = playerPosition != Vector3.zero ? playerPosition : lastKnowPosition;
			if (Vector3.Distance(transform.position, pos) > .1f) {
				moveTo (pos);
			}
		}

		if (!chasing && hasCheckPoints) {

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
		}

		legs.SetBool("isWalking", isWalking);
	}

	void OnTriggerStay2D (Collider2D obj) {
		if (obj.tag == "Player") {
			hasPlayerInSight = false;

			int layerMask = 1 << 12;
			layerMask = ~layerMask;

			Vector3 dir = obj.transform.position - transform.position;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
			if (hit.collider.tag == "Player") {
				hasPlayerInSight = true;
				playerPosition = obj.transform.position;
				lastKnowPosition = playerPosition;
				chasing = true;
			} else {
				playerPosition = Vector3.zero;
			}
		}
	}

	void OnTriggerExit2D (Collider2D obj) {
		if (obj.tag == "Player") {

			if (playerPosition != Vector3.zero) {
				lastKnowPosition = playerPosition;
				playerPosition = Vector3.zero;
			}

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