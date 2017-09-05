using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarPathfinding : MonoBehaviour {

    private Seeker seeker;
    private Enemy enemy;

    // Use this for initialization
    void Start () {
        seeker = GetComponent<Seeker>();
        enemy = GetComponent<Enemy>();
        seeker.pathCallback += OnPathComplete;
    }
	
    private void OnPathComplete(Path p) {
        // Pfad wird zurückgegeben
        if (!p.error) {
            enemy.GetCurrentState().SetNewPath(p);
        }
    }
    public void StartPath(Vector3 from, Vector3 to) {
        seeker.StartPath(from, to);
    }
    private void OnDisable() {
        seeker.pathCallback -= OnPathComplete;
    }
}
