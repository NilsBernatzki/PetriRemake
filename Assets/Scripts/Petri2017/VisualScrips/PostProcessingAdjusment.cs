using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingAdjusment : MonoBehaviour {

    public PostProcessingProfile profile;
    private float currentHealth;
    private Player player;

    private void Start() {
        player = GameManager.singleton.Player.GetComponent<Player>();
        }

    // Update is called once per frame
    void Update () {
        //currentHealth = player.health
		if (profile.vignette.enabled == true) {
            var vignette = profile.vignette.settings;
            vignette.smoothness = 1 - (currentHealth / 100);
            profile.vignette.settings = vignette;
            }
	}
}
