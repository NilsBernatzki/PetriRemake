using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwimAnimationSpeed : MonoBehaviour {

    private Animator animator;
    private Rigidbody2D rig2D;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        rig2D = transform.parent.parent.GetComponent<Rigidbody2D>();
        }

    // Update is called once per frame
    void Update() {
        float speed = rig2D.velocity.magnitude + 2f;
        if (speed >= 10) {
            speed = 10;
            }
        animator.speed = speed;
        }
    }
