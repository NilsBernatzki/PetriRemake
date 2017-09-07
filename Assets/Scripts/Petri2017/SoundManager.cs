using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    private GameObject PlayerObj;
    private Player player;
    [SerializeField]
    private AudioSource ambientAudioSource;
    [SerializeField]
    private AudioSource collisionWithGlassAudioSource;
    [SerializeField]
    private AudioSource collisionWithObstacleAudioSource;

    // Use this for initialization
    void Start () {
        PlayerObj = GameManager.singleton.Player;
        player = PlayerObj.GetComponent<Player>();

        player.CollisionWithEnvironment += OnCollisionWithEnvironment;

	}
	
	// Update is called once per frame
	void Update () {
        UpdateAmbientPitchOnPlayerVelocity();
	}

    private void UpdateAmbientPitchOnPlayerVelocity() {
        float pitch = 1 + player.currentVeloT;
        ambientAudioSource.pitch = Mathf.Clamp(pitch, 1f, 2f);
    }

    private void OnCollisionWithEnvironment(string tag) {
        float playerVelo = player.currentVeloT;
        if (tag == "Glass") {
            collisionWithGlassAudioSource.pitch = Random.Range(0.95f, 1.05f) + (0.3f * playerVelo);
            collisionWithGlassAudioSource.PlayOneShot(collisionWithGlassAudioSource.clip, playerVelo);
        } else {
            collisionWithObstacleAudioSource.pitch = Random.Range(0.45f, 0.55f) + (0.3f * playerVelo);
            collisionWithObstacleAudioSource.PlayOneShot(collisionWithObstacleAudioSource.clip,playerVelo);
        }
    }
}
