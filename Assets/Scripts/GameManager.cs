using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Texture2D cursor;

	void Start () {
		Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
	}

	void Update () {
	
	}
}
