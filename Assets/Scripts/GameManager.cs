using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public Texture2D cursor;
	public List<IaManager> ennemies;

	public static GameManager instance { get; private set; }
	
	void Awake () {
		instance = this;
	}

	void Start () {
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
	}

	public void addEnnemy (IaManager ennemy) {
		ennemies.Add(ennemy);
	}

	public void killEnnemy (IaManager ennemy) {
		ennemies.Remove(ennemy);
		if (ennemies.Count == 0) {
			// TODO GAME END
			Debug.Log ("WIN");
		}
	}

	public void gameOver () {
		// TODO
		Debug.Log("Game over");
	}

	void Update () {
	
	}
}
