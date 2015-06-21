using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public Texture2D cursor;
	public List<IaManager> ennemies;
	public CanvasGroup diedUi;
	public CanvasGroup winUi;
	public AudioSource musicAudio;
	public AudioSource winAudio;
	public AudioSource loseAudio;

	[HideInInspector] public bool isDead = false;
	[HideInInspector] public bool hasWon = false;

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
		if (isDead) {
			return ;
		}
		winAudio.Play();
		hasWon = true;
	}

	public void killEnnemy (IaManager ennemy) {
		ennemies.Remove(ennemy);
		if (ennemies.Count == 0) {
			win();
		}
	}

	public void gameOver () {
		if (hasWon) {
			return ;
		}
		loseAudio.Play ();
		isDead = true;
	}

	void Update () {
		diedUi.alpha = isDead ? 1 : 0;
		winUi.alpha = hasWon ? 1 : 0;
		if (Input.GetKeyDown(KeyCode.R) && PlayerManager.instance.hasMoved) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
