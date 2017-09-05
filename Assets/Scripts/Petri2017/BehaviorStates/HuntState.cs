using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HuntState : State {

    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float nextWaypointDistance;
    [SerializeField]
    protected float nonViewSpeed;
    [SerializeField]
    protected float nonRangeSpeed;

    [SerializeField]
    protected int updatePathFrameCount;
    protected int updatePathFrameCounter;

    protected override void Start() {
        base.Start();
    }
    public override void Behave() {
        if (enemy == null) return;
        base.Behave();
        //Debug.Log("hunt");
        SwarmManager.singleton.playerIsHunted = true;
        if(enemy.playerInSight && Vector3.Distance(playerTransform.position,currentPosition) <= enemy.nonPathHuntDist) {
            if(groupable.leader.enemy.currentEnergyT <= 0.2) {
                enemy.ChangeBehavior(Behavior.flee);
            } else {
                enemy.ChangeBehavior(Behavior.attack);
            }
            return;
        } else {
            if(groupable.leader.enemy.currentEnergyT <= 0.2) {
                enemy.ChangeBehavior(Behavior.flee);
            }

            HuntMovement();
            
        }
        
    }

    protected void HuntMovement() {
        updatePathFrameCounter++;
        if (WaitedEnoughFrames(ref updatePathFrameCounter, updatePathFrameCount) || (enemy.path == null && !currentlyCalculatingPath)) {
            updatePathFrameCounter = 0;
            SearchNewPath(currentPosition, enemy.player.FuturePosToTarget(currentPosition));
        }
        float actualUsedSpeed;
        if (!enemy.playerInSight) {
            actualUsedSpeed = nonViewSpeed;
        } else if (!enemy.playerInRange) {
            actualUsedSpeed = nonRangeSpeed;
        } else {
            actualUsedSpeed = speed;
        }
        movement.MoveOnPath(enemy.path, enemy.currentWaypointIndex, actualUsedSpeed);
        UpdateWaypointIndex();
    }

    protected void SearchNewPath(Vector3 from, Vector3 to) {
        currentlyCalculatingPath = true;
        pathfinding.StartPath(from, to);
    }

    protected void UpdateWaypointIndex() {
        if (enemy.path == null) return;
        if (enemy.currentWaypointIndex >= enemy.path.vectorPath.Count) {
            return;
        }
        if (Vector3.Distance(currentPosition, enemy.path.vectorPath[enemy.currentWaypointIndex]) < nextWaypointDistance) {
            enemy.currentWaypointIndex++;
            return;
        }
    }

    protected override void Movement_ReachedPathEnd() {
        if(enemy.GetCurrentState() == this) {
            //Debug.Log("reachedend HUNT");
        }
    }
    public override void OnEnter() {
        enemy.path = null;
        currentlyCalculatingPath = false;
        if (groupable.IsLeader()) {
            foreach (Groupable g in groupable.group) {
                g.enemy.ChangeBehavior(Behavior.hunt);
            }
        }
    }
    public override void OnExit() {
        SwarmManager.singleton.playerIsHunted = false;
    }
}
