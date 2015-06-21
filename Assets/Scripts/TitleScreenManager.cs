using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenManager : MonoBehaviour {

	private Animator playButtonAnimator;
	public GameObject playButton;
	public GameObject quitButton;

	void Awake () {
		playButtonAnimator = playButton.GetComponent<Animator> ();
	}

	void Start () {
		StartCoroutine (PlayButton ());
	}

	IEnumerator PlayButton () {
		yield return new WaitForSeconds (1f);
		playButton.transform.FindChild("Text").GetComponent<Text> ().color = new Color (1, 1, 1, 1);
		quitButton.transform.FindChild("Text").GetComponent<Text> ().color = new Color (1, 1, 1, 1);
		playButtonAnimator.SetTrigger ("emphasize");
	}

	void Update () {

	}

	public void StartGame () {
		Application.LoadLevel ("level1");
	}

	public void QuitGame () {
		Application.Quit ();
	}

}
