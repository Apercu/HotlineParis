using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public Texture2D cursor;
	public List<IaManager> ennemies;
	public CanvasGroup diedUi;

	[HideInInspector] public bool isDead = false;

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

	public void win () {

	}

	public void killEnnemy (IaManager ennemy) {
		ennemies.Remove(ennemy);
		if (ennemies.Count == 0) {
			win();
		}
	}

	public void gameOver () {
		isDead = true;
	}

	void Update () {
		diedUi.alpha = isDead ? 1 : 0;
		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
