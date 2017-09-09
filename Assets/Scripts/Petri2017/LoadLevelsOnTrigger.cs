using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevelsOnTrigger : MonoBehaviour {

    public int levelID;
    public Text levelText;

    public bool selected;

    public Color selectedTextColor;
    public Color startTextColor;

	// Use this for initialization
	void Start () {
        startTextColor = levelText.color;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.singleton.qiutGame) return;
        if (selected) {
            levelText.color = selectedTextColor;
            if (Input.GetButtonDown("Fire1")) {
                SceneManager.LoadScene(levelID);
            }
        } else {
            levelText.color = startTextColor;
        }
	}

    private void OnTriggerEnter2D(Collider2D c) {
        selected = true;
    }
    private void OnTriggerExit2D(Collider2D c) {
        selected = false;
    }
}
