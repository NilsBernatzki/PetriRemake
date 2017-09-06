using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidBehavior : MonoBehaviour {

    //zieht Gruppe auseinander
    public Vector3 SeparationGroup(Groupable g) {

        Vector3 separationForce = Vector3.zero;
        Vector3 pos = transform.position;
        float rad = g.GetNeighborsRadius();
        float minDist = SwarmManager.singleton.boidSeperationDistance;
        Vector3 dir;
        float dist;

        foreach (Groupable thisG in g.neighbors) {
            if (!thisG) continue;
            dir = pos - thisG.transform.position;
            dist = dir.magnitude;
            if(dist < minDist) {
                separationForce += dir * (1 - dist / rad);
            }
        }
        return separationForce.normalized;
    }
    //hält Gruppe zusammen
    public Vector3 CohesionGroup(Groupable g, Vector3 movementDir) {

        Vector3 centreForce = Vector3.zero;

        foreach (Groupable thisG in g.neighborsInGroup) {
            if (!thisG) continue;
            centreForce += thisG.transform.position;
        }
        if(g.neighborsInGroup.Count != 0) {
            centreForce = (centreForce / g.neighborsInGroup.Count)  - transform.position;
        } 
        //centreForce = centreForce / g.group.Count + movementDir;
        return centreForce.normalized;
    }

    /*
    //verbindet beide Forces und führt Kraft aus
    void MergeForces() {

        if (Random.Range(0, Swarm.singleton.BoidUse) < 1) {

            Vector3 direction;
            speed = Swarm.singleton.EnemySpeed;

            if (mySwarm.Count > 0) {

                Vector3 centreOfGroup = Cohesion();
                Vector3 separationFromGroup = Separation();

                float distToMember = Swarm.singleton.enemyAvoidDistance;

                direction = (centreOfGroup + separationFromGroup * distToMember) - this.transform.position;
                Vector2 direction2D = (Vector2)direction;

                myRigidbody.AddForce(direction2D.normalized * speed * Time.deltaTime);
            }

        //Wenn Random nicht eingetroffen dann geh kurzer Impuls zu einer RandomPosition 
        else {
                Vector3 vrandom = RandomPoint() - transform.position;
                Vector2 vrandom2D = new Vector2(vrandom.x, vrandom.y);
                myRigidbody.AddForce(vrandom2D.normalized * speed / 50 * Time.deltaTime);
            }

        }
    }
    */
}

