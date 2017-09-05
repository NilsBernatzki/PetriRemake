using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State {

    [SerializeField]
    protected float maxSpeed;
    protected float currentSpeed;
    [SerializeField]
    protected float nextWaypointDistance;

    [SerializeField]
    protected float stopFleeMinDist;

    [SerializeField]
    protected int updatePathFrameCount;
    protected int updatePathFrameCounter;
    [SerializeField]
    protected int updateFleePosFrameCount;
    protected int updateFleePosFrameCounter;

    [SerializeField]
    protected float fleeTime;
    protected float fleeTimer;

    protected float energyT = 1f;
    protected override void Start() {
        base.Start();
    }
    public override void Behave() {
        if (enemy == null) return;
        base.Behave();
        //Debug.Log("flee");
        SwarmManager.singleton.playerMadeEnemysFlee = true;
        //Range quit
        Vector3 leaderPos = groupable.leader.enemy.GetCurrentPosition();
        if (Vector3.Distance(leaderPos, playerTransform.position) >= stopFleeMinDist) {
            enemy.ChangeBehavior(Behavior.idle);
            return;
        }
        
        //quit because Leader quitted or new Leader;
        if(groupable.leader.enemy.GetCurrentBehavior() != enemy.GetCurrentBehavior()) {
            enemy.ChangeBehavior(groupable.leader.enemy.GetCurrentBehavior());
            return;
        }
        
        //Leader : find fleePos
        if (groupable.IsLeader()) {
            
            if (enemy.fleePosition == Vector3.zero) {
                enemy.fleePosition = GetFleePosition();
            } else {
                updateFleePosFrameCounter++;
                if (WaitedEnoughFrames(ref updateFleePosFrameCounter, updateFleePosFrameCount)) {
                    enemy.fleePosition = GetFleePosition();
                }
            }
           
        }

        //GetSpeed
        if (enemy.playerInSight || currentSpeed == 0) {
            float  currentSpeedT = 1 - ((Vector3.Distance(leaderPos, playerTransform.position) - enemy.detectionMinDistance * 2) / stopFleeMinDist);
            currentSpeed = maxSpeed * currentSpeedT;
        }

        
        fleeTimer += Time.deltaTime;
        if (fleeTimer >= fleeTime * enemy.currentEnergyT) {
            energyT = Mathf.Lerp(energyT, enemy.currentEnergyT, 0.1f);
            currentSpeed *= energyT;
        }
        currentSpeed =  Mathf.Clamp(currentSpeed, maxSpeed * 0.4f, maxSpeed);

        //Move
        FleeMovement(groupable.leader.enemy.fleePosition, currentSpeed);
    }
    
    protected void FleeMovement(Vector3 towards, float fleeSpeed) {
        if (towards == Vector3.zero) return;
        updatePathFrameCounter++;
        if (WaitedEnoughFrames(ref updatePathFrameCounter, updatePathFrameCount) || (enemy.path == null && !currentlyCalculatingPath)) {
            updatePathFrameCounter = 0;
            SearchNewPath(currentPosition, towards);
        }
        movement.MoveOnPath(enemy.path, enemy.currentWaypointIndex, fleeSpeed);
        UpdateWaypointIndex();
    }

    protected Vector3 GetFleePosition() {

        Vector3 playerPos = playerTransform.position;
        Quaternion playerRot = playerTransform.rotation;

        //Check if player sees me or not
        bool InSightOfPlayer = false;

        if (GetAngleDifference(playerRot, currentPosition - playerPos) <= 90f) {
            InSightOfPlayer = true;
        } else {
            InSightOfPlayer = false;
        }

        Vector3 fleePos = Vector3.zero;
        Vector3 oldFleePos = enemy.fleePosition;
        int maxTries = 100;
        bool foundFleePos = false;

        //Find a point that fits distance and orientation of player to the point
        for (int i = 0; i < maxTries; i++) {

            fleePos = new Vector3(Random.Range(-7, 24), Random.Range(-3, 13), 0);

            //nicht zu nah an Player
            if (Vector3.Distance(playerPos, fleePos) < 3f) continue;
            if(oldFleePos != Vector3.zero) {
                //zu weit auseinander
                if (Vector3.Distance(fleePos, oldFleePos) > 5f) continue;
                //fleePos näher als vorher
                if (Vector3.Distance(playerPos, fleePos) < Vector3.Distance(playerPos, oldFleePos)) continue;
            }
            
            float angleDifference = GetAngleDifference(playerRot, fleePos - playerPos);
            if (InSightOfPlayer) {
                if (angleDifference <= 100) {
                    foundFleePos = true;
                    break;
                }
            } else {
                if(angleDifference >= 80) {
                    foundFleePos = true;
                    break;
                }
            }
        }
        if (!foundFleePos) {
            //Debug.Log("No Point found in flee Behavior -> took random point");
        }
        
        return fleePos;
    }

    protected float GetAngleDifference(Quaternion fromRotation, Vector3 toDirection) {
        return Mathf.Abs(Quaternion.Angle(fromRotation, Quaternion.LookRotation(Vector3.forward, toDirection)));
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
        enemy.fleePosition = Vector3.zero;
    }
    public override void OnEnter() {
        enemy.path = null;
        currentlyCalculatingPath = false;
        enemy.fleePosition = Vector3.zero;
        currentSpeed = 0;
        fleeTimer = 0;
        energyT = 1f;
        if (groupable.IsLeader()) {
            foreach (Groupable g in groupable.group) {
                g.enemy.ChangeBehavior(Behavior.flee);
            }
        }
    }
    public override void OnExit() {
        enemy.path = null;
        enemy.fleePosition = Vector3.zero;
        SwarmManager.singleton.playerMadeEnemysFlee = false;
    }
}
