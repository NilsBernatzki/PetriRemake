using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform playerTransform;

	// Use this for initialization
	void Start () {
        playerTransform = GameManager.singleton.Player.transform;

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(playerTransform.position.x,playerTransform.position.y,transform.position.z);
	}
}
