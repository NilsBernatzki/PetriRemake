using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitGameOnTrigger : MonoBehaviour {

    public Text levelText;

    public bool selected;

    public Color selectedTextColor;
    public Color startTextColor;

    private bool waitForPlayerDeath;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.singleton.qiutGame) {
            if (!waitForPlayerDeath) {
                waitForPlayerDeath = true;
                StartCoroutine(WaitForPlayerDeath());
            } else {
                return;
            }
        }
        if (selected) {
            levelText.color = selectedTextColor;
            if (Input.GetButtonDown("Fire1")) {
                GameManager.singleton.qiutGame = true;
                StartCoroutine(SwarmManager.singleton.EnemySpawner(12));
            }
        } else {
            levelText.color = startTextColor;
        }
    }

    private IEnumerator WaitForPlayerDeath() {
        yield return new WaitUntil(() => GameManager.singleton.Player.GetComponent<Player>().dead);
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D c) {
        selected = true;
    }
    private void OnTriggerExit2D(Collider2D c) {
        selected = false;
    }
}
