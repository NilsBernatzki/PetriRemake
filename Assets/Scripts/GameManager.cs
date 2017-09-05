using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public static GameManager singleton;

	public GameObject Player;
    public float gameSpeedMult;
    void Awake(){
		singleton = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
