using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

[System.Serializable]
public enum Behavior {
    idle,hunt,detection,stalking,attack,flee
}

public class Enemy : MonoBehaviour {

    [SerializeField]
    protected Behavior currentBehavior;

    protected State currentState;
    [SerializeField]
    protected State idleState;
    [SerializeField]
    protected State huntState;
    [SerializeField]
    protected State detectionState;
    [SerializeField]
    protected State stalkingState;
    [SerializeField]
    protected State attackState;
    [SerializeField]
    protected State fleeState;

    [HideInInspector]
    public Player player;
    protected Transform playerTransform;
    [SerializeField]
    protected LayerMask obstaclesLayer;
    [SerializeField]
    protected LayerMask enemyLayer;

    protected Vector3 currentPosition;

    [HideInInspector]
    public Vector3 currentDestination;

    public Path path;
    [HideInInspector]
    public int currentWaypointIndex;
    [HideInInspector]
    public Groupable groupable;
    protected bool isReset;

    [Header("Energy")]
    [SerializeField]
    protected float maxEnergy;
    protected Color currentEnergyColor;
    public float currentEnergy;
    public float currentEnergyT;

    [SerializeField]
    protected float restorePerSec;
    [SerializeField]
    protected float loosePerSecHunt;
    [SerializeField]
    protected float loosePerSecFlee;
    public float looseOnAttack;
    [SerializeField]
    protected int updateEnergyFrameCount;
    protected int updateEnergyFrameCounter;

    protected Color emptyCol;
    protected Color fullCol;

    //DebugEnergyColor
    protected Color startCol;
    protected SpriteRenderer spriteRenderer;

    [Header("Charge")]
    [SerializeField]
    protected int updateChargeFrameCount;
    protected int updateChargeFrameCounter;

    public float chargemin;
    public float chargemax;
    public float currentCharge;

    public bool charged;

    [Header("Detection")]
    [SerializeField]
    public float detectionMinDistance;
    [SerializeField]
    public float detectionMaxDistance;
    [HideInInspector]
    public Vector3 lastKnownPlayerPos;

    [SerializeField]
    protected int updateSightCount;
    protected int sightFrameCounter;

    public bool playerInRange;
    [SerializeField]
    protected float minSightAngle;
    [SerializeField]
    protected float maxSightAngle;
    public bool playerInSight;
    public bool wallHackActive;

    [Header("Hunt")]
    public float huntDist;
    [SerializeField]
    public float nonPathHuntDist;

    [Header("Damage")]
    [SerializeField]
    protected float maxDamage;
    public float maxVelocity;
    [HideInInspector]
    public Rigidbody2D rig;

    [Header("Flee")]
    public Vector3 fleePosition;

    [Space(10)]
    public bool isDead;
    public bool snackDebug;

    // Use this for initialization
    protected virtual void Start () {
        rig = GetComponent<Rigidbody2D>();
        player = GameManager.singleton.Player.GetComponent<Player>();
        playerTransform = player.transform;
        groupable = GetComponent<Groupable>();
        minSightAngle /= 2;
        maxSightAngle /= 2;
        maxVelocity = maxVelocity / 100f;
        currentState = idleState;

        currentEnergy = maxEnergy;
        emptyCol = SwarmManager.singleton.emptyEnergyColor;
        fullCol = SwarmManager.singleton.fullEnergyColor;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startCol = spriteRenderer.color;
    }
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {
        if (isDead) return;
        
        UpdateCurrentPosition(ref currentPosition);
        CheckForPlayer();
        UpdateEnergy();
        UpdateCharge();
       
	}
    public virtual void ChangeBehavior(Behavior newBehavior) {
        
    }
    public virtual void ChangeBehaviorGroup(Behavior newBehavior) {

    }
    public State GetCurrentState() {
        return currentState;
    }
    public Behavior GetCurrentBehavior() {
        return currentBehavior;
    }
    private void UpdateCurrentPosition(ref Vector3 currentPos) {
        currentPos = transform.position;
    }
    public Vector3 GetCurrentPosition() {
        return currentPosition;
    }

    //############### CheckForPlayer ##############

    protected void CheckForPlayer() {

        float rangeDistance = detectionMinDistance + ((detectionMaxDistance - detectionMinDistance) * player.currentVeloT);
        int updateCount = updateSightCount;

        sightFrameCounter++;
        if (!WaitedEnoughFrames(ref sightFrameCounter, updateCount)) return;

        switch (currentBehavior) {
            case Behavior.idle:
                updateCount = updateSightCount;
                break;
            case Behavior.detection:
                rangeDistance = detectionMinDistance * 2f;
                updateCount = updateSightCount;
                break;
            case Behavior.stalking:
                rangeDistance = detectionMinDistance * 2f;
                updateCount = updateSightCount;
                break;
            case Behavior.hunt:
                rangeDistance = detectionMinDistance *2f;
                updateCount = updateSightCount;
                break;
        }
        Mathf.Clamp(rangeDistance, detectionMinDistance, detectionMaxDistance);

        if (IsCloseEnoughTo(playerTransform.position, rangeDistance)) {
            playerInRange = true;

        } else {
            playerInRange = false;
            playerInSight = false;
            return;
        }

        if(Vector3.Distance(playerTransform.position, currentPosition) > detectionMinDistance) {
            if (!HasPlayerInViewPort()) return;
        }
        
        if (CanSee(playerTransform.position)) {
            playerInSight = true;
            AddSighting();
        } else {
            playerInSight = false;
        }
        if (wallHackActive) {
            AddSighting();
        }
    }
    
    public void AddSighting() {
        if (currentBehavior == Behavior.flee) return;
        PlayerSighting newSighting = new PlayerSighting(this, playerTransform.position);
        SwarmManager.singleton.AddSighting(newSighting);
    }

    //############# Energy ##################

    protected void UpdateEnergy() {
        switch (currentBehavior) {
            case Behavior.idle:
                currentEnergy += restorePerSec * Time.deltaTime;
                break;
            case Behavior.detection:
                currentEnergy += restorePerSec * Time.deltaTime;
                break;
            case Behavior.stalking:
                currentEnergy += restorePerSec * Time.deltaTime;
                break;
            case Behavior.hunt:
                currentEnergy -= loosePerSecHunt * Time.deltaTime;
                break;
            case Behavior.attack:
                currentEnergy -= loosePerSecHunt * Time.deltaTime;
                break;
            case Behavior.flee:
                currentEnergy -= loosePerSecFlee * Time.deltaTime;
                break;
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        updateEnergyFrameCounter++;
        if (!WaitedEnoughFrames(ref updateEnergyFrameCounter, updateEnergyFrameCount)) return;

        float averageEnergy = currentEnergy;
        foreach(Groupable g in groupable.neighbors) {
            if(g.enemy.GetCurrentBehavior() == GetCurrentBehavior()) {
                averageEnergy += g.enemy.currentEnergy;
            }
        }
        averageEnergy /= (groupable.neighbors.Count + 1);
        currentEnergy =  Mathf.Lerp(currentEnergy, averageEnergy, 0.25f);

        currentEnergyT = currentEnergy / maxEnergy;

        //Colors
        if (SwarmManager.singleton.debugEnergyMode) {
            float t = currentEnergy / maxEnergy;
            
            currentEnergyColor = Color.Lerp(emptyCol, fullCol, t);
            ChangeColor(currentEnergyColor);
        } else {
            if (!SwarmManager.singleton.debugChargeMode && !SwarmManager.singleton.debugGroupsMode) {
                ChangeColor(startCol);
            }
        }
    }

    public void ChangeColor(Color newColor) {
        if (spriteRenderer.color != newColor) {
            spriteRenderer.color = newColor;
        }
    }

    //################ Charge #####################

    protected void UpdateCharge() {

        if (groupable.leader.enemy.charged) {
            charged = true;
        } else {
            charged = false;
        }

        updateChargeFrameCounter++;
        if (!WaitedEnoughFrames(ref updateChargeFrameCounter, updateChargeFrameCount)) return;

        currentCharge = chargemin + (((float)groupable.leader.group.Count - 1) / ((float)SwarmManager.singleton.maxGroupSize -1)) * (chargemax - chargemin);

        //Debug
        if (SwarmManager.singleton.debugChargeMode && !SwarmManager.singleton.debugGroupsMode && !SwarmManager.singleton.debugEnergyMode) {
            if (charged) {
                ChangeColor(SwarmManager.singleton.chargedColor);
            } else {
                ChangeColor(SwarmManager.singleton.unchargedColor);
            }
        } else {
            if(!SwarmManager.singleton.debugGroupsMode && !SwarmManager.singleton.debugEnergyMode) {
                ChangeColor(startCol);
            }
        }
    }

    //################ Damage #####################

    private void OnCollisionEnter2D(Collision2D collision) {

        if (charged) {
            if (collision.collider.CompareTag("Player")) {
                if (!player.playerDamaged) {
                    player.playerDamaged = true;
                    groupable.leader.enemy.ShockPlayer(currentCharge, transform.position);
                }
            }
        }
    }

    public void ShockPlayer(float charge, Vector3 hitpoint) {
        player.GetDamage(charge, hitpoint);
        //Debug.Log("Shock" + "dmg: " + currentCharge);
        foreach (Groupable g in groupable.group) {
            g.enemy.charged = false;
        }
        

    }

    //################ GetSnacked #################

    public void GetSnacked() {
        isDead = true;
        GetComponent<CircleCollider2D>().enabled = false;
        transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
        rig.MovePosition(GameManager.singleton.transform.GetChild(0).position);
        currentState.OnExit();
        Destroy(this.gameObject, 5f);
    }
    //################ Detection ##################

    protected bool IsCloseEnoughTo(Vector3 position, float minDistance) {
        if (Vector3.Distance(position, currentPosition) > minDistance) {
            return false;
        } else {
            return true;
        }
    }

    protected bool CanSee(Vector3 position) {
        //Raycasts on polygonColliders are expensive!!
        Vector3 origin = currentPosition;
        Vector3 direction = position - currentPosition;
        float length = direction.magnitude;

        if (Physics2D.Raycast(origin, direction, length, obstaclesLayer.value)) {
            return false;
        } else {
            return true;
        }
    }
    private bool HasPlayerInViewPort() {
        float angleDelta = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(transform.forward, playerTransform.position - currentPosition));
        
        float currentSightAngle = minSightAngle + ((maxSightAngle - minSightAngle) * player.currentVeloT);
        //Debug.Log("SightAngle: " + currentSightAngle + "AngleDelta: " + angleDelta);
        if (angleDelta <= currentSightAngle) {
            return true;
        } else {
            return false;
        }
    }
    private void DebugSightRay(Vector3 destination) {
        Debug.DrawRay(transform.position, destination - transform.position, Color.red, 0.25f);
    }

    //############## Other #############

    public void ResetPath() {
        if (isReset) return;

        path = null;
        if (groupable.leader.id == groupable.id) {
            isReset = true;
            ResetGroup();
        }
    }
    private void ResetGroup() {
        foreach (Groupable g in groupable.group) {
            g.enemy.ResetPath();
        }
        isReset = false;
    }

    protected bool WaitedEnoughFrames(ref int frameCounter, int frameCount) {
        if (frameCounter >= frameCount) {
            frameCounter = 0;
            return true;
        } else {
            return false;
        }
    }
}
