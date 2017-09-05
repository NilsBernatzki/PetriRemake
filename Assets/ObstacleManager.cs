using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {
    public static ObstacleManager singleton;
    public Vector2 angleVariation;
    public Vector2 timerMinMax;
    void Awake () {
        singleton = this;
	}
	
}
