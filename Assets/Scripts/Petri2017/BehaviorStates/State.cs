using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class State : MonoBehaviour {
    
    protected Vector3 currentPosition;
    protected Transform playerTransform;

    protected Enemy enemy;
    protected Groupable groupable;
    protected MoveEnemy movement;
    protected AStarPathfinding pathfinding;
    
    protected bool currentlyCalculatingPath;

    // Use this for initialization
    protected virtual void Start () {
        enemy = GetComponent<Enemy>();
        movement = GetComponent<MoveEnemy>();
        pathfinding = GetComponent<AStarPathfinding>();
        groupable = GetComponent<Groupable>();
        playerTransform = GameManager.singleton.Player.transform;

        movement.ReachedPathEnd += Movement_ReachedPathEnd;
	}

    protected virtual void Movement_ReachedPathEnd() {
        
    }
    public virtual void OnEnter() {

    }
    public virtual void OnExit() {

    }

    public virtual void Behave() {
        currentPosition = enemy.GetCurrentPosition();
    }
    public virtual void LateBehave() {
        //if (enemy == null) return;
        enemy.rig.velocity = Vector2.ClampMagnitude(enemy.rig.velocity, enemy.maxVelocity);
    }
    public virtual void SetNewPath(Path newPath) {
        enemy.path = newPath;
        enemy.currentWaypointIndex = 0;
        enemy.GetCurrentState().currentlyCalculatingPath = false;
    }

    protected bool WaitedEnoughFrames(ref int frameCounter, int frameCount) {
        if (frameCounter >= frameCount) {
            frameCounter = 0;
            return true;
        } else {
            return false;
        }
    }
}
