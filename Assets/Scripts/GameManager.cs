using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Texture2D cursor;
	public int ennemyNumber = 1;

	public static GameManager instance { get; private set; }
	
	void Awake () {
		instance = this;
	}

	void Start () {
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
	}

	public void killEnnemy () {
		--ennemyNumber;
		if (ennemyNumber == 0) {
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
