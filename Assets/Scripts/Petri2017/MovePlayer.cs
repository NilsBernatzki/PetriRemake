using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MovePlayer : MonoBehaviour {

    private Player player;
    private enum PlayerBehavior { normal,stealth,boost};
    private PlayerBehavior currentBehavior;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float stealthSpeed;
    [SerializeField]
    private float boostSpeed;
   
    private float maxRotationAnglesPerFrame = 10f;
    private Rigidbody2D rig;
    private float maxSpeed;
    [Header("Energy")]
    [SerializeField]
    private float maxEnergy;
    public float currentEnergy;
    [SerializeField]
    private float energyRecoverPerSec;
    [SerializeField]
    private float energyBoostCostPerSec;
    [SerializeField]
    private float blockBoostPercent;
    [SerializeField]
    private bool boostBlocked;

    //DebugEnergy
    [SerializeField]
    private Color emptyCol;
    private Color fullCol;
    [SerializeField]
    private Color blockedCol;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Text boostText;
    private Quaternion boostTextRotation;

    [Header("Snack")]
    public float snackAngle;
    public bool snackCooldown;
    private float snackBoostTimer;
    public float snackBoostTime;
    public Enemy closestEnemy;

    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boostTextRotation = boostText.transform.rotation;
        fullCol = spriteRenderer.color;
        currentEnergy = maxEnergy;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBoostCanvas();
        if (player.dead) return;
        UpdateEnergy();
        UpdateBehavior();

        closestEnemy = null;
        if (Input.GetButton("Fire1")) {
            if (player.enemiesInAngle.Count > 0) {
                snackBoostTimer += Time.deltaTime;
                if(snackBoostTimer <= snackBoostTime) {
                    closestEnemy = player.enemiesInAngle.OrderByDescending(e => Vector3.Distance(e.GetCurrentPosition(), transform.position)).First();
                    SnackMovement(closestEnemy);
                } else {
                    if (!snackCooldown) {
                        snackCooldown = true;
                        StartCoroutine(SnackingCooldown());
                    }
                }

            }
        } 
        if(closestEnemy == null) {
            UpdateMovement();
        }
        ClampVelocity();
    }
    public void ClampVelocity() {
        rig.velocity = Vector2.ClampMagnitude(rig.velocity, maxSpeed / (100f * rig.mass));
    }
    public void SnackMovement(Enemy e) {
        Vector3 dir = e.transform.position - transform.position;
        Vector3 move = GetMovementVec(dir);
        
        MoveInstance(move*2, true);
        RotateInstance(move*2);
    }
   private IEnumerator SnackingCooldown() {
        yield return new WaitForSeconds(2f);
        snackBoostTimer = 0;
        snackCooldown = false;
    }
    private void UpdateMovement() {
        Vector2 input = GetDirection();
        if (input != Vector2.zero) {
            MoveInstance(GetMovementVec(input), true);
            RotateInstance(input);
        }
    }
    private void RotateInstance(Vector2 input) {

        if(rig.velocity != Vector2.zero) {
            Vector3 towards = (Vector3)rig.velocity;
            float rotSpeed = maxRotationAnglesPerFrame;
            if(input != Vector2.zero) {
                towards = (Vector3)(input + rig.velocity);
            } else {
                rotSpeed /= 100f;
            }
            rig.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, 0, rig.rotation), Quaternion.LookRotation(Vector3.forward, towards), maxRotationAnglesPerFrame).eulerAngles.z;
        }
    }
    private void MoveInstance(Vector2 movement, bool useRig) {
        if (useRig) {
            rig.AddForce(movement);
        }
    }

    public Vector2 GetDirection() {
        Vector2 dir;
        dir.x = Input.GetAxisRaw("Horizontal");
        dir.y = Input.GetAxisRaw("Vertical");

        return dir;
    }
    private void UpdateBehavior() {
        float triggerInput = Input.GetAxisRaw("Trigger");

        if(triggerInput < 0 && !boostBlocked) {
            currentBehavior = PlayerBehavior.boost;
        }
        if(triggerInput > 0) {
            currentBehavior = PlayerBehavior.stealth;
        }
        if(triggerInput == 0 || (triggerInput < 0 && boostBlocked)) {
            currentBehavior = PlayerBehavior.normal;
        }
    }
    private Vector2 GetMovementVec(Vector2 dir) {
        Vector2 move;
        float currentSpeed;

        switch (currentBehavior) {
            case PlayerBehavior.boost:
                currentSpeed = boostSpeed * 2f;
                maxSpeed = boostSpeed;
                break;
            case PlayerBehavior.stealth:
                currentSpeed = stealthSpeed;
                maxSpeed = stealthSpeed;
                break;
            default:
                currentSpeed = speed;
                maxSpeed = speed;
                break;
        }
        
        move = dir.normalized * currentSpeed * GameManager.singleton.gameSpeedMult * Time.fixedDeltaTime;
        //rig.velocity = Vector2.ClampMagnitude(rig.velocity,maxSpeed/(100f * rig.mass));
        
        return move;
    }

   

    private void UpdateEnergy() {
        switch (currentBehavior) {
            case PlayerBehavior.normal:
                currentEnergy += energyRecoverPerSec * Time.deltaTime;
                break;
            case PlayerBehavior.stealth:
                currentEnergy += energyRecoverPerSec * Time.deltaTime;
                break;
            case PlayerBehavior.boost:
                currentEnergy -= energyBoostCostPerSec * Time.deltaTime;
                break;
            default:
                break;
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        if(currentEnergy == 0) {
            if (!boostBlocked) {
                boostBlocked = true;
                StartCoroutine(UnlockBoost());
            }
           
        }
        if (!boostBlocked) {
            spriteRenderer.color = Color.Lerp(emptyCol, fullCol, currentEnergy / maxEnergy);
        } else {
            spriteRenderer.color = blockedCol;
        }
    }
    private IEnumerator UnlockBoost() {

        yield return new WaitUntil(() => currentEnergy > maxEnergy * blockBoostPercent / 100f);
        boostBlocked = false;
    }
    private void UpdateBoostCanvas() {
        boostText.text = Mathf.RoundToInt(((currentEnergy / maxEnergy) * 100)).ToString();
        if (boostBlocked) {
            boostText.color = emptyCol;
        } else {
            boostText.color = fullCol;
        }
        Vector3 boostTextPos = transform.position - (transform.up * 0.5f);
        boostText.transform.position = boostTextPos;
        boostText.transform.rotation = boostTextRotation;
    }
}
