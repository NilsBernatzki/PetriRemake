using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterEnemy : Enemy {

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void FixedUpdate () {
        if (isDead) return;
        base.FixedUpdate();
        currentState.Behave();
        currentState.LateBehave();
    }
    
    public override void ChangeBehavior(Behavior newBehavior) {
        if (currentBehavior == newBehavior) return;
        currentState.OnExit();

        currentBehavior = newBehavior;
        switch (currentBehavior) {
            case Behavior.idle:
                currentState = idleState;
                break;
            case Behavior.detection:
                currentState = detectionState;
                break;
            case Behavior.stalking:
                currentState = stalkingState;
                break;
            case Behavior.hunt:
                currentState = huntState;
                break;
            case Behavior.attack:
                currentState = attackState;
                break;
            case Behavior.flee:
                currentState = fleeState;
                break;
        }
        ResetPath();
        currentState.OnEnter();
    }

    public override void ChangeBehaviorGroup(Behavior newBehavior) {
        foreach(Groupable g in groupable.leader.group) {
            g.enemy.ChangeBehavior(newBehavior);
        }
    }
}
