using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveEnemy : MonoBehaviour {

    private Enemy enemy;
    private Groupable groupable;
    private Rigidbody2D rig;
    public event System.Action ReachedPathEnd;
    //[SerializeField]
    //private bool useBoidBehavior;
    [SerializeField]
    private BoidBehavior boidBehavior;

	// Use this for initialization
	void Start () {
        enemy = GetComponent<Enemy>();
        groupable = GetComponent<Groupable>();
        rig = GetComponent<Rigidbody2D>();
        boidBehavior = GetComponent<BoidBehavior>();
	}
	
    public void MoveOnPath(Path p, int currentWaypointIndex, float speed) {
        if (p == null) return;
        if (currentWaypointIndex >= p.vectorPath.Count) {
            FireReachedPathEnd();
            return;
        }
        MoveInstance(p.vectorPath[currentWaypointIndex], speed);
    }
    
    public void MoveInstance(Vector3 towards, float speed) {
        speed *= GameManager.singleton.gameSpeedMult;
        Vector3 dir = towards - enemy.GetCurrentPosition();

        if (groupable.neighbors.Count > 0 && Random.Range(0f,1f) > 0.35f) {
            Vector3 seperation = boidBehavior.SeparationGroup(groupable);
            Vector3 cohesion = boidBehavior.CohesionGroup(groupable, dir);

            rig.AddForce((Vector2)seperation * SwarmManager.singleton.boidSeperationForce);
            rig.AddForce((Vector2)cohesion * SwarmManager.singleton.boidCohesionForce);
        }

        dir = dir.normalized * speed * Time.fixedDeltaTime * (1 + Mathf.Pow(groupable.leader.group.Count / (SwarmManager.singleton.maxGroupSize * 1.5f),2));
        rig.AddForce((Vector2)dir);
        RotateTowardsVector((Vector3)rig.velocity);
    }

    public void MoveInstance(Vector3 towards, float speed, bool useBoidBehavior) {
        speed *= GameManager.singleton.gameSpeedMult;
        Vector3 dir = towards - enemy.GetCurrentPosition();

        if (useBoidBehavior) {
            if (groupable.neighbors.Count > 0) {
                Vector3 seperation = boidBehavior.SeparationGroup(groupable);
                Vector3 cohesion = boidBehavior.CohesionGroup(groupable, dir);

                rig.AddForce((Vector2)seperation * SwarmManager.singleton.boidSeperationForce);
                rig.AddForce((Vector2)cohesion * SwarmManager.singleton.boidCohesionForce);
            }
        }
        dir = dir.normalized * speed * Time.fixedDeltaTime * (1 + Mathf.Pow(groupable.leader.group.Count / (SwarmManager.singleton.maxGroupSize * 2), 2));
        rig.AddForce((Vector2)dir);
        RotateTowardsVector((Vector3)rig.velocity);
    }

    public void RotateTowardsVector(Vector3 towards) {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(Vector3.forward, towards),10f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, towards),0.15f * rig.velocity.magnitude);
        rig.angularVelocity = 0;
    }
    private void FireReachedPathEnd() {
        if(ReachedPathEnd != null) {
            ReachedPathEnd();
        }
    }
}
