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
    public Camera mainCamera;
    [SerializeField]
    private float timeScalePause;
    [SerializeField]
    private float duration;
    private Text zoomScaleText;

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
        scoreText.text = score.ToString();
	    if(score > currentState.maxScore) {
            if(stateCounter < gameStates.Count-1) {
                stateCounter++;
                currentState = gameStates[stateCounter];
            }
        }
        if (Input.GetButtonDown("Start")) {
            pauseMode = !pauseMode;
            ChangePauseMode(pauseMode);
        }
	}

    private void ChangePauseMode(bool pause) {
        if (pause) {

            StartCoroutine(ChangeToPauseModeCo());
        } else {
            Time.timeScale = 1f;
            mainCamera.orthographicSize = 5f;
        }
    }

    private IEnumerator ChangeToPauseModeCo() {
        Time.timeScale = timeScalePause;
        float t = 0;
        while(t < duration) {
            t += 0.01f;
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 15f, t);
            yield return null;
        }
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
