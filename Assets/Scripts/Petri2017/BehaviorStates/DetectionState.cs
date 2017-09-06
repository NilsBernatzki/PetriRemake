using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionState : State {

    [SerializeField]
    protected float firstDetectionTime;
    protected float firstDetectionTimer;

    [SerializeField]
    protected float wallHackTime;

    protected override void Start() {
        base.Start();

    }

    public override void Behave() {
        if (enemy == null) return;
        base.Behave();
        //Debug.Log("detection");

        switch (groupable.leader.enemy.GetCurrentBehavior()) {
            case Behavior.idle:
                if (enemy.playerInSight) {
                    groupable.leader.enemy.ChangeBehavior(Behavior.detection);
                } else {
                    enemy.ChangeBehavior(Behavior.idle);
                    return;
                }
                break;
            case Behavior.hunt:
                enemy.ChangeBehavior(Behavior.hunt);
                return;
            case Behavior.stalking:
                enemy.ChangeBehavior(Behavior.stalking);
                return;
            case Behavior.flee:
                enemy.ChangeBehavior(Behavior.flee);
                return;
        }

       

        if (groupable.IsLeader()) {
            enemy.lastKnownPlayerPos = GetLastKnownPlayerPos();
        }

        DetectionMovement();

        float dist = Vector3.Distance(currentPosition, playerTransform.position);
        float distanceT = dist / enemy.detectionMaxDistance;
        distanceT = Mathf.Clamp(distanceT, 0.2f, 1);

        float playerVeloT = (1 - enemy.player.currentVeloT);
        playerVeloT = Mathf.Clamp(playerVeloT, 0.2f, 1);

        firstDetectionTimer += Time.deltaTime;
        float minT = Mathf.Clamp(playerVeloT * distanceT, 0.5f, 1);
        if (firstDetectionTimer >= firstDetectionTime * minT) {
            firstDetectionTimer = 0;

            if(groupable.leader.enemy.chargeTimer >= enemy.chargedTime){
                groupable.leader.enemy.charged = true;
            }
            
            //Chose Behavior
            float tGroup = (float)groupable.leader.group.Count / (float)SwarmManager.singleton.maxGroupSize;
            
            if (tGroup >= Random.Range(0.25f,1.25f)) {
                if (groupable.leader.enemy.charged) {
                    groupable.leader.enemy.ChangeBehavior(Behavior.hunt);
                } 
            } else {
                groupable.leader.enemy.ChangeBehavior(Behavior.flee);
            }
        }        
    }

    protected void DetectionMovement() {
        movement.RotateTowardsVector(groupable.leader.enemy.lastKnownPlayerPos - currentPosition);
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

    protected IEnumerator WallHack() {
        float t = 0;
        while(t <= wallHackTime) {
            enemy.wallHackActive = true;
            t += Time.deltaTime;
            yield return null;
        }
        enemy.wallHackActive = false;
    }
    
    public override void OnEnter() {
        if(groupable.leader.id != groupable.id) {
            groupable.leader.enemy.ChangeBehavior(Behavior.detection);
        }
        if (enemy.playerInSight) {
            StartCoroutine(WallHack());
        }
    }
    public override void OnExit() {
        enemy.lastKnownPlayerPos = Vector3.zero;
    }
}
