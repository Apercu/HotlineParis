using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	private GameObject player;

	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void LateUpdate () {
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
	}
}
