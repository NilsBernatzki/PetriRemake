using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetForceTentacle : MonoBehaviour {
    private Vector2 force;
	void Start () {
        force = transform.parent.GetComponent<CalculateForce>().force;
        GetComponent<ConstantForce2D>().force = force;

	}
}
