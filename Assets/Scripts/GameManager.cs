using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameState {
    public int maxScore;
    public int maxEnemies;
    public int currentMaxGroupSize;
}

public class GameManager : MonoBehaviour {
	
	public static GameManager singleton;
    public bool isMenu;
    public bool quitGame;
    public bool goToMenu;
	public GameObject Player;
    public float gameSpeedMult;

    public int score;
    public Text scoreText;

    [Header("GameStates")]
    public GameState currentState;
    public List<GameState> gameStates;
    private int stateCounter = 0;

    [Header("Pause")]
    public bool pauseMode;
    public bool changing;
    public Camera mainCamera;
    [SerializeField]
    private float timeScalePause;
    [SerializeField]
    private float duration;

    public Text zoomScaleText;
    public Text timeText;
    
    public Text enterMenuText;
    public Color enterMenuCol1;
    public Color enterMenuCol2;

    void Awake(){
		singleton = this;
        currentState = gameStates[stateCounter];
	}

	// Use this for initialization
	void Start () {
        Player.GetComponent<Player>().PlayerDied += OnPlayerDeath;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isMenu) {
            timeText.text = "TIME: " + Time.time.ToString();
            zoomScaleText.text = "SCALE: " + string.Format("{0:0.000}", ((15 - mainCamera.orthographicSize) / 123f + 0.024f));
            scoreText.text = score.ToString();
            if (score > currentState.maxScore) {
                if (stateCounter < gameStates.Count - 1) {
                    stateCounter++;
                    currentState = gameStates[stateCounter];
                }
            }
            if (Input.GetButtonDown("Start")) {
                pauseMode = !pauseMode;
                ChangePauseMode(pauseMode);
            }

            if (pauseMode) {
                enterMenuText.enabled = true;
            } else {
                enterMenuText.enabled = false;
            }

            if(pauseMode && Input.GetButtonDown("Fire1")) {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }
        }
       
	}

    private void ChangePauseMode(bool pause) {
        if (!changing) {
            StartCoroutine(ChangeToPauseModeCo(pause));
        }  
    }

    private IEnumerator ChangeToPauseModeCo(bool pause) {
        changing = true;
        SoundManager.singleton.PlayZoomSound(pause);
        if (pause) {
            Time.timeScale = timeScalePause;
            float t = 0;
            float z = 0;
            while (t < duration) {
                t += 0.01f;
                z += 0.01f;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 15f, z);
                yield return null;
            }
        } else {
            float t = 0;
            float z = 0;
            while (t < duration) {
                t += 0.01f;
                z += 0.01f;
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 5f, z);
                yield return null;
            }
            Time.timeScale = 1f;
        }
        changing = false;
    }

    private void OnPlayerDeath() {
        Player.GetComponent<Player>().PlayerDied -= OnPlayerDeath;
        if (quitGame) return;
        StartCoroutine(WaitForPlayerDeath());
    }

    private IEnumerator WaitForPlayerDeath() {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(0);
    }
}
