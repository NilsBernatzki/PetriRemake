using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class IdleState : State {
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float nextWaypointDistance;

    private Vector3 centerPos;

    protected override void Start() {
        base.Start();
        centerPos = GameManager.singleton.transform.position;
    }
    public override void Behave() {
        if (enemy == null) return;
        base.Behave();
        if (enemy.playerInSight || groupable.leader.enemy.GetCurrentBehavior() == Behavior.detection) {
            enemy.ChangeBehavior(Behavior.detection);
            return;
        }
        //Debug.Log("idle");
        IdleMovemement();
    }
    protected void IdleMovemement() {
        if (enemy.path == null && !currentlyCalculatingPath) {

            if (groupable.leader.id == groupable.id) {
                enemy.currentDestination = GetNewTargetPosition();
            } else {
                enemy.currentDestination = GetPathIndexPos(5);
            }
            SearchNewPath(currentPosition, enemy.currentDestination);
        }

        movement.MoveOnPath(enemy.path, enemy.currentWaypointIndex, speed);
        UpdateWaypointIndex();
    }

    private Vector3 GetNewTargetPosition() {
        
        Vector3 randomPos = centerPos + (Vector3)UnityEngine.Random.insideUnitCircle * 8f;
        return randomPos;
    }

    public Vector3 GetPathIndexPos(int addedIndex) {
        Enemy leaderEnemy = groupable.leader.enemy;
        if (leaderEnemy.path != null) {
            while ((leaderEnemy.currentWaypointIndex + addedIndex) >= leaderEnemy.path.vectorPath.Count) {
                addedIndex--;
            }
            return leaderEnemy.path.vectorPath[leaderEnemy.currentWaypointIndex + addedIndex];
        } else {
            return leaderEnemy.GetCurrentPosition();
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
        if (enemy.GetCurrentState() == this) {
            enemy.path = null;
        }
        if (Vector3.Distance(groupable.leader.enemy.currentDestination, currentPosition) < 1f) {
            groupable.leader.enemy.ResetPath();
        }
    }
    public override void OnEnter() {
        enemy.path = null;
        currentlyCalculatingPath = false;
    }
    public override void OnExit() {
        SwarmManager.singleton.playerIsHunted = false;
    }

}
