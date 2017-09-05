using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform playerTransform;
    private Vector3 offset;
	// Use this for initialization
	void Start () {
        playerTransform = GameManager.singleton.Player.transform;
        offset = new Vector3(0f, 0f, -1f);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = playerTransform.position + offset;
	}
}
