using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour {

    private Enemy enemy;
    [SerializeField]
    private AudioSource constantSound;
    private float startPitch;
    [SerializeField]
    private AudioSource chargedSound;
    [SerializeField]
    private AudioSource startChargeSound;

    [SerializeField]
    private int updateSoundsFrameCount;
    private int updateSoundsFrameCounter;
	// Use this for initialization
	void Start () {
        enemy = transform.parent.GetComponent<Enemy>();
        startPitch = constantSound.pitch;
	}
	
	// Update is called once per frame
	void Update () {
        updateSoundsFrameCounter++;
        if (!WaitedEnoughFrames(ref updateSoundsFrameCounter, updateSoundsFrameCount)) return;
        UpdateConstantSound();
        UpdateChargedSound();
	}

    private void UpdateChargedSound() {
        if (enemy.groupable.leader.enemy.charged) {
            if(!chargedSound.enabled && enemy.groupable.IsLeader()) {
                startChargeSound.PlayOneShot(startChargeSound.clip);
            }
            chargedSound.enabled = true;
        } else {
            chargedSound.enabled = false;
        }
    }

    private void UpdateConstantSound() {
        if (enemy.groupable.IsLeader()) {
            constantSound.enabled = true;
            constantSound.pitch = Mathf.Lerp(startPitch, startPitch + (0.5f * enemy.rig.velocity.magnitude), 0.15f);
        } else {
            constantSound.enabled = false;
        }
    }

    private bool WaitedEnoughFrames(ref int frameCounter, int frameCount) {
        if (frameCounter >= frameCount) {
            frameCounter = 0;
            return true;
        } else {
            return false;
        }
    }
}
