using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudShowPosition : MonoBehaviour {
    private Slider slider;
    private Transform playerTransform;
    public bool isVertical;
	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();
        playerTransform = GameManager.singleton.Player.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (isVertical == true) {
            slider.value = playerTransform.position.y;
            }
        else {
            slider.value = playerTransform.position.x;
            }
	}
}
