using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class EnemyLightningField : MonoBehaviour {

    private Enemy enemy;
    private LightningFieldScript lightningFieldScript;

    [SerializeField]
    private int maxBolts;

    [SerializeField]
    private int updateBoltFrameCount;
    private int updateBoltFrameCounter;

    [SerializeField]
    private int maxGroupSize;

    [SerializeField]
    private float currentDebugGroupSizeT;

    // Use this for initialization
    void Start () {
        enemy = transform.parent.GetComponent<Enemy>();
        lightningFieldScript = GetComponent<LightningFieldScript>();
        maxGroupSize = GameManager.singleton.gameStates[GameManager.singleton.gameStates.Count - 1].currentMaxGroupSize;
    }
	
	// Update is called once per frame
	void Update () {
        updateBoltFrameCounter++;
        if (!WaitedEnoughFrames(ref updateBoltFrameCounter, updateBoltFrameCount)) return;

        UpdateBolts();

	}
    private void UpdateBolts() {

        if (enemy.isDead) {
            lightningFieldScript.enabled = false;
        }
        
        if (enemy.charged) {
            int groupCount = enemy.groupable.leader.group.Count;
            if(groupCount > 1) {
                
                float groupT = (float)groupCount / (float)maxGroupSize;
                lightningFieldScript.CountRange.Minimum = 0;
                lightningFieldScript.CountRange.Maximum = Mathf.RoundToInt(maxBolts * groupT);
                lightningFieldScript.IntervalRange.Maximum = 0.6f - (0.5f * groupT);
                lightningFieldScript.IntervalRange.Minimum = 0.35f - (0.25f * groupT);
                return;
            }
            
        }
        NoBolts();
    }

    private void NoBolts() {
        lightningFieldScript.CountRange.Minimum = 0;
        lightningFieldScript.CountRange.Maximum = 0;
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
