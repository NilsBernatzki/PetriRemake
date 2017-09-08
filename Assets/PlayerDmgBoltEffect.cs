using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class PlayerDmgBoltEffect : MonoBehaviour {

    private LightningBoltShapeConeScript boltScript;
    private LightningFieldScript fieldScript;
    private float timer;
    public bool isField;

    void Start () {
        
        if (isField == true) {
            fieldScript = GetComponent<LightningFieldScript>();
            }
        else {
            boltScript = GetComponent<LightningBoltShapeConeScript>();
            }
        
        }
	

	void Update () {
		if (Input.GetKey (KeyCode.Space)) {
            timer = 2f;
            if (isField == false) {
                boltScript.CountRange.Minimum = 5;
                boltScript.CountRange.Maximum = 15;
                }
            }
        else {
            if (isField == false) {
                boltScript.CountRange.Minimum = 0;
                boltScript.CountRange.Maximum = 0;
                }          
            }
        if (timer >= 0) {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, 2);
            }

        if (isField == true) {
            fieldScript.CountRange.Minimum = 0;
            fieldScript.CountRange.Maximum = Mathf.RoundToInt(timer);
            }

        }
}
