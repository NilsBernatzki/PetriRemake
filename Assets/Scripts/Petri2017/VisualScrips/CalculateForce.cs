using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateForce : MonoBehaviour {
    public Vector2 force;
    private Vector2 angleVariation;
    private Vector2 timerMinMax;
    private float timer;
    public List<GameObject> children;
    private float angle;

	// Use this for initialization
	void Awake () {

        force = transform.up*-15;
                
        }
    private void Start() {

        angleVariation = ObstacleManager.singleton.angleVariation;
        timerMinMax = ObstacleManager.singleton.timerMinMax;
        }

    private void Update() {
        if (timer <= 0) {
            angle = Random.Range(angleVariation.x, angleVariation.y);
            foreach (GameObject child in children) {
                child.GetComponent<ConstantForce2D>().torque = angle;
                }
            timer = Random.Range(timerMinMax.x, timerMinMax.y);
            }
        timer = timer - Time.deltaTime;
        }
    }  

