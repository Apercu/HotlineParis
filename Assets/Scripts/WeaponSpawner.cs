using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSpawner : MonoBehaviour {

	public List<GameObject> weapons;

	void Start () {
		Instantiate (weapons [Random.Range (0, weapons.Count)], transform.position, Quaternion.identity);
	}

	void Update () {
		
	}
}
