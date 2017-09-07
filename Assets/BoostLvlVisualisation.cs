using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostLvlVisualisation : MonoBehaviour {

    private new SpriteRenderer renderer;
    private MovePlayer movePlayer;
    private float currentEnergy;

    // Use this for initialization
    void Start() {
        renderer = GetComponent<SpriteRenderer>();
        movePlayer = transform.parent.GetComponent<MovePlayer>();
        }

    // Update is called once per frame
    void Update() {
        currentEnergy = movePlayer.currentEnergy;
        renderer.material.SetFloat("_BoostLevel", currentEnergy / 100);
        }
    }
