using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[System.Serializable]
public class GameState {
    public int maxScore;
    public int maxEnemies;
    public int currentMaxGroupSize;
}

public class GameManager : MonoBehaviour {
	
	public static GameManager singleton;
    public bool qiutGame;
    public bool goToMenu;
	public GameObject Player;
    public float gameSpeedMult;

    public int score;
    public Text scoreText;

    [Header("GameStates")]
    public GameState currentState;
    public List<GameState> gameStates;
    private int stateCounter = 0;
    void Awake(){
		singleton = this;
        currentState = gameStates[stateCounter];
	}

	// Use this for initialization
	void Start () {
		
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
	}
}
