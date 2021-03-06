﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour {

    public event System.Action PlayerDied;
    public event System.Action<string> CollisionWithEnvironment;
    private Rigidbody2D rig;
    public Vector3 boundsSize;

    [Header("Health")]
    [SerializeField]
    private float maxHealth;
    public float health;
    public bool dead;

    public bool heal;
    [SerializeField]
    private float healthRecoverPerSec;
    public float noDamageTimer;
    public float noDamageTime;
    public bool getDamageOnTouch;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private int updateGraphFrameCount;
    private int updateGraphFrameCounter;

    private float maxMeasuredVeloMag;
    public float currentVeloT;

    [SerializeField]
    private PlayerDmgBoltEffect boltFXfield;
    [SerializeField]
    private PlayerDmgBoltEffect boltFXShock;

    [Header("Snack")]
   
    public float snackAngle;
    private MovePlayer movement;
    public Transform aimCircle;
    public List<Enemy> closeEnemies = new List<Enemy>();
    public List<Enemy> enemiesInAngle = new List<Enemy>();
    public bool snacking;

    public bool playerStunned;
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
        movement = GetComponent<MovePlayer>();
        health = maxHealth;
        healthText.text = health.ToString();
        rig.AddForce(Vector2.down);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateHealth();
        if (health <= 0) {
            if (!dead) {
                dead = true;
                FireEventPlayerDied();
            }
            return;
        }
        UpdatePenaltyArea();
        UpdateMaxMeasuredVelo();



        enemiesInAngle.Clear();
        if (closeEnemies.Count > 0) {
            Vector3 pos = transform.position;
            foreach (Enemy e in closeEnemies) {
                if (e) {
                    if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Vector3.forward, e.transform.position - pos)) <= snackAngle) {
                        enemiesInAngle.Add(e);
                    }
                }
                
            }

        }

    }
    
    private void FireEventPlayerDied() {
        if(PlayerDied != null) {
            PlayerDied();
        }
    }
    public Vector3 FuturePosToTarget(Vector3 enemyPos) {
        
        Vector3 futurePos;
        futurePos = rig.position + Vector2.ClampMagnitude( rig.velocity * (Vector3.Distance(rig.position, enemyPos) / (rig.velocity.magnitude + 1)),2f);
        DebugFuturePos(futurePos);
        return futurePos;
    }
    
    private void DebugFuturePos(Vector3 fpos) {
        Vector3 start = transform.position;
        Vector3 dir = fpos - transform.position;
        Debug.DrawRay(start, dir, Color.red,0.2f);
    }

    private void UpdatePenaltyArea() {

        updateGraphFrameCounter++;

        if(WaitedEnoughFrames(ref updateGraphFrameCounter, updateGraphFrameCount)){

            Vector3 penaltyPos = transform.position;

            Bounds resetBounds;
            GraphUpdateObject resetGuo;

            resetBounds = new Bounds(penaltyPos, boundsSize * 10f);
            resetGuo = new GraphUpdateObject(resetBounds);

            resetGuo.resetPenaltyOnPhysics = true;
            resetGuo.requiresFloodFill = false;

            resetGuo.addPenalty = 0;

            AstarPath.active.UpdateGraphs(resetGuo);

            if (SwarmManager.singleton.playerIsHunted || SwarmManager.singleton.playerMadeEnemysFlee) {

                Bounds penaltyBounds; 
                GraphUpdateObject guo;
                               
                if (SwarmManager.singleton.playerIsHunted) {
                    penaltyPos = transform.position + (-transform.up * 0.5f);

                    penaltyBounds = new Bounds(penaltyPos, boundsSize);
                    
                    guo = new GraphUpdateObject(penaltyBounds);
                   
                    guo.resetPenaltyOnPhysics = true;
                    guo.requiresFloodFill = false;
                    
                    guo.addPenalty = 1000;
                    
                    AstarPath.active.UpdateGraphs(guo);
                    return;
                }
                
                if (SwarmManager.singleton.playerMadeEnemysFlee) {

                    penaltyPos = transform.position + (transform.up * 1.5f);
                    penaltyBounds = new Bounds(penaltyPos, boundsSize);
                    penaltyBounds.Expand(2.75f);

                    guo = new GraphUpdateObject(penaltyBounds);
                    guo.resetPenaltyOnPhysics = false;
                    guo.requiresFloodFill = false;

                    guo.addPenalty = 25000;
                    AstarPath.active.UpdateGraphs(guo);

                    penaltyPos = transform.position; ;
                    Bounds penaltyBoundsAtPlayer = new Bounds(penaltyPos, boundsSize);
                    penaltyBoundsAtPlayer.Expand(1.2f);

                    GraphUpdateObject guoPlayer = new GraphUpdateObject(penaltyBoundsAtPlayer);

                    guoPlayer.resetPenaltyOnPhysics = false;
                    guoPlayer.requiresFloodFill = false;

                    guoPlayer.addPenalty = 50000;

                    AstarPath.active.UpdateGraphs(guoPlayer);
                }
                
            }
        }
    }
    private void DebugPenaltyPos(Vector3 penaltyCenter, Color col) {
        Vector3 start = transform.position;
        Vector3 dir = penaltyCenter - transform.position;
        Debug.DrawRay(start, dir, col,0.2f);
    }

    protected bool WaitedEnoughFrames(ref int frameCounter, int frameCount) {
        if (frameCounter >= frameCount) {
            frameCounter = 0;
            return true;
        } else {
            return false;
        }
    }
    private void UpdateMaxMeasuredVelo() {
        float veloMag = rig.velocity.magnitude;
        if (veloMag > maxMeasuredVeloMag) {
            maxMeasuredVeloMag = veloMag;
        }
        currentVeloT = veloMag / maxMeasuredVeloMag;
    }
    private void UpdateHealth() {

        noDamageTimer += Time.deltaTime;
        if(noDamageTimer > noDamageTime) {
            heal = true;
        }

        if (!dead && heal) {
            health += healthRecoverPerSec * Time.deltaTime;
        }
        health = Mathf.Clamp(health, 0, maxHealth);
        healthText.text = Mathf.RoundToInt(health).ToString();


        if (health < 40) {
            healthText.color = new Vector4(255, 255, 0, 1);
            }

        if (health >= 40) { 

            healthText.color = Color.green;
            }
        if (dead == true) {
            healthText.text = "DEAD";
            healthText.color = Color.red;
            }
    }
    public void GetDamage(float damage, Vector3 hitPoint) {

        if (GameManager.singleton.quitGame) {
            health = -1f;
            heal = false;
            noDamageTimer = 0;
            rig.AddForce((transform.position - hitPoint).normalized * 1000f);
            movement.ClampVelocity();
            StartCoroutine(RecoverFromStunned(damage));
            return;
        }
        health -= Mathf.Pow(damage -1,1.5f);
        heal = false;
        noDamageTimer = 0;
        rig.AddForce((transform.position - hitPoint).normalized * damage * 100);
        movement.ClampVelocity();
        StartCoroutine(RecoverFromStunned(damage));
    }
    private IEnumerator RecoverFromStunned(float damage) {
        float dmgT = damage / SwarmManager.singleton.maxGroupSize;
        if (dmgT >= 0.3f) {
            StartCoroutine(boltFXfield.FieldStunEffect(dmgT * 3.5f));
            StartCoroutine(boltFXShock.ShockBoltEffect(dmgT));
            SoundManager.singleton.ClapSoundOnDamage(dmgT);
            yield return new WaitForSeconds(dmgT*2.5f);
        }
        playerStunned = false;
    }
    
    private void OnTriggerEnter2D(Collider2D c) {
        Enemy e = c.transform.parent.GetComponent<Enemy>();
        if (!closeEnemies.Contains(e)) {
            closeEnemies.Add(e);
        }
    }

    private void OnTriggerExit2D(Collider2D c) {
        Enemy e = c.transform.parent.GetComponent<Enemy>();
        if (closeEnemies.Contains(e)) {
            closeEnemies.Remove(e);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Enemy")) {
            Enemy e = collision.collider.transform.parent.GetComponent<Enemy>();
            if(e.isDead && e == movement.closestEnemy) {
                e.GetSnacked();
                movement.closestEnemy = null;
                movement.grabbedEnemy = false;
            }
            return;
        }
        
        //Collision mit Umgebung
        if(CollisionWithEnvironment != null) {
            CollisionWithEnvironment(collision.collider.tag);
        }
    }

}
