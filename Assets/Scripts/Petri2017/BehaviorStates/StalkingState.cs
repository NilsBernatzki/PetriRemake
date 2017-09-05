using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingState : State {

    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float nextWaypointDistance;
    [SerializeField]
    protected int updatePathFrameCount;
    protected int updatePathFrameCounter;

    protected bool lostSight;
    // Use this for initialization
    protected override void Start() {
        base.Start();

    }

    // Update is called once per frame
    public override void Behave() {
        if (enemy == null) return;
        base.Behave();

        if (groupable.IsLeader()) {
            if (enemy.playerInSight && lostSight) {
                foreach (Groupable g in groupable.group) {
                    g.enemy.ChangeBehavior(Behavior.detection);
                }
                return;
            }

            foreach (Groupable g in groupable.group) {
                if (g.enemy.playerInSight) {
                    break;
                }
                lostSight = true;
            }
        }

        float dist = Vector3.Distance(currentPosition, playerTransform.position);
        if(dist <= enemy.huntDist && enemy.playerInSight) {
            groupable.leader.enemy.ChangeBehavior(Behavior.hunt);
        }

        if (groupable.leader.enemy.GetCurrentBehavior() == Behavior.idle) {
            if (enemy.playerInSight) {
                groupable.leader.enemy.ChangeBehavior(Behavior.stalking);
            } else {
                enemy.ChangeBehavior(Behavior.idle);
                return;
            }
        }

        if (groupable.IsLeader()) {
            enemy.lastKnownPlayerPos = GetLastKnownPlayerPos();
        }

        if (groupable.leader.enemy.lastKnownPlayerPos != Vector3.zero && Vector3.Distance(groupable.leader.enemy.lastKnownPlayerPos, currentPosition) > 1f) {
            StalkingMovement(speed);
        } else {
            if (groupable.IsLeader()) {
                foreach (Groupable g in groupable.group) {
                    g.enemy.ChangeBehavior(Behavior.idle);
                }
            }
        }
    }

    protected void StalkingMovement(float tempSpeed) {
        updatePathFrameCounter++;
        if (WaitedEnoughFrames(ref updatePathFrameCounter, updatePathFrameCount) || (enemy.path == null && !currentlyCalculatingPath)) {
            updatePathFrameCounter = 0;
            SearchNewPath(currentPosition, groupable.leader.enemy.lastKnownPlayerPos);
        }
        movement.MoveOnPath(enemy.path, enemy.currentWaypointIndex, tempSpeed);
        UpdateWaypointIndex();
    }
    protected Vector3 GetLastKnownPlayerPos() {
        if (enemy.playerInSight) {
            return playerTransform.position;
        } else {
            Vector3 playerPos = Vector3.zero;
            if (SwarmManager.singleton.sightings.Count != 0) {
                for (int i = 0; i < SwarmManager.singleton.sightings.Count; i++) {
                    if (groupable.group.Contains(SwarmManager.singleton.sightings[i].enemy.groupable)) {
                        playerPos = SwarmManager.singleton.sightings[i].playerPos;
                        break;
                    }
                    playerPos = Vector3.zero;
                }
            } else {
                playerPos = Vector3.zero;
            }
            return playerPos;
        }
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
       
    }

    public override void OnEnter() {
        lostSight = false;
    }
    public override void OnExit() {
        enemy.lastKnownPlayerPos = Vector3.zero;
    }
}
