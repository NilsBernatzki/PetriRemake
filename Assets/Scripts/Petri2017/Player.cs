using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public event System.Action PlayerDied;

    private Rigidbody2D rig;
    public Vector3 boundsSize;

    [Header("Health")]
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float health;
    public bool dead;

    [SerializeField]
    private float healthRecoverPerSec;
    public bool getDamageOnTouch;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private int updateGraphFrameCount;
    private int updateGraphFrameCounter;

    private float maxMeasuredVeloMag;
    public float currentVeloT;


    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
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
        if (Input.GetKeyDown(KeyCode.D)) {
            GetDamage(10f);
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
            
            if (SwarmManager.singleton.playerIsHunted || SwarmManager.singleton.playerMadeEnemysFlee) {
                Vector3 penaltyPos;
                Bounds myBounds;
                GraphUpdateObject guo;

                if (SwarmManager.singleton.playerIsHunted) {
                    penaltyPos = transform.position + (-transform.up * 0.75f);
                    myBounds = new Bounds(penaltyPos, boundsSize);
                    //DebugPenaltyPos(penaltyPos, Color.red);
                    guo = new GraphUpdateObject(myBounds);
                    guo.addPenalty = 500;
                    guo.updatePhysics = true;
                    AstarPath.active.UpdateGraphs(guo);
                    return;
                }
                
                if (SwarmManager.singleton.playerMadeEnemysFlee) {
                    penaltyPos = transform.position + (transform.up * 0.75f);
                    myBounds = new Bounds(penaltyPos, boundsSize);
                    myBounds.Expand(1.5f);
                    //DebugPenaltyPos(penaltyPos, Color.blue);
                    guo = new GraphUpdateObject(myBounds);
                    guo.addPenalty = 50000;
                    guo.updatePhysics = true;
                    AstarPath.active.UpdateGraphs(guo);
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
        if (!dead) {
            health += healthRecoverPerSec * Time.deltaTime;
        }
        health = Mathf.Clamp(health, 0, maxHealth);
        healthText.text = Mathf.RoundToInt(health).ToString();
    }
    public void GetDamage(float damage) {
        health -= damage;
    }
}
