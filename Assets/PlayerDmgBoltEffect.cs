using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class PlayerDmgBoltEffect : MonoBehaviour {

    private LightningBoltShapeConeScript boltScript;
    private LightningFieldScript fieldScript;
    private float timer;
    public bool isField;

    private Player player;

    void Start() {

        if (isField == true) {
            fieldScript = GetComponent<LightningFieldScript>();
        } else {
            boltScript = GetComponent<LightningBoltShapeConeScript>();
        }
        player = GameManager.singleton.Player.GetComponent<Player>();
    }


    void Update() {
        if (player.dead && isField) {
            fieldScript.CountRange.Minimum = 0;
            fieldScript.CountRange.Maximum = Mathf.RoundToInt(2f);
        }
    }

    public IEnumerator ShockBoltEffect(float damageT) {
        boltScript.CountRange.Minimum = Mathf.RoundToInt(5 * damageT);
        boltScript.CountRange.Maximum = Mathf.RoundToInt(15 * damageT);
        yield return new WaitForSeconds(0.1f);
        boltScript.CountRange.Minimum = 0;
        boltScript.CountRange.Maximum = 0;
    }

    public IEnumerator FieldStunEffect(float timer) {
        fieldScript.CountRange.Minimum = 0;
        fieldScript.CountRange.Maximum = Mathf.RoundToInt(timer);
        while(timer > 0) {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, 5);
            fieldScript.CountRange.Maximum = Mathf.RoundToInt(timer);
            yield return null;
        }
        fieldScript.CountRange.Maximum = Mathf.RoundToInt(0);
    }

}
