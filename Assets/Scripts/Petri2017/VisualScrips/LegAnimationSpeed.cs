using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAnimationSpeed : MonoBehaviour {

    private Animator animator;
    private Rigidbody2D rig2D;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rig2D = GameManager.singleton.Player.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        float speed = rig2D.velocity.magnitude + 0.2f;
        if (speed >= 4) {
            speed = 4;
            }
        animator.speed = speed;
	}
}
