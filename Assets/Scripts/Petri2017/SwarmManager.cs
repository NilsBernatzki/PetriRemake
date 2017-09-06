using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class PlayerSighting {
    public Enemy enemy;
    public Vector3 playerPos;
    public float timePassed;

    public PlayerSighting(Enemy thisEnemy, Vector3 lastPlayerPosition) {
        enemy = thisEnemy;
        playerPos = lastPlayerPosition;
        timePassed = 0;
    }
}

public class SwarmManager : MonoBehaviour {

    public static SwarmManager singleton;
    [SerializeField]
    private GameObject enemyPrefab;
    
    [Header("Boids")]
    [SerializeField]
    private int amountEnemysOnStart;
    public int currentMaxGroupSize;
    public int maxGroupSize;

    public float boidSeperationForce;
    public float boidSeperationDistance;
    public float boidCohesionForce;

    [Header("Sightings")]
    public List<PlayerSighting> sightings = new List<PlayerSighting>();
    [SerializeField]
    private int maxSightings;
    [SerializeField]
    private float maxSightingTime;

    [Header("Debug")]
    public bool debugGroupsMode;
    private Transform[] positions;
    private List<Groupable> groupables = new List<Groupable>();
    [Space(5)]
    public bool debugEnergyMode;
    public Color fullEnergyColor;
    public Color emptyEnergyColor;
    [Space(5)]
    public bool playerIsHunted;
    public bool playerMadeEnemysFlee;
    [Space(5)]
    public bool debugChargeMode;
    public Color chargedColor;
    public Color unchargedColor;

    private void Awake() {
        singleton = this;
    }
    // Use this for initialization
    void Start () {
        if (debugEnergyMode) {
            debugGroupsMode = false;
        }
        positions = GetComponentsInChildren<Transform>();
        StartCoroutine(EnemySpawner(amountEnemysOnStart));
	}
	
	// Update is called once per frame
	void Update () {
        if (debugEnergyMode) {
            debugGroupsMode = false;
        }
        UpdatePlayerSightings();
	}
    private IEnumerator EnemySpawner(int amount) {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < amount; i++) {
            GameObject enemy = Instantiate(enemyPrefab, (Vector2)positions[Random.Range(0,positions.Length)].position + Random.insideUnitCircle * 2f, Quaternion.identity);
            groupables.Add(enemy.GetComponent<Groupable>());
            yield return new WaitForSeconds(0.2f);
        }
    }
    public Groupable GetGroupableFromID(float id) {
        Groupable thisG = groupables.Find(g => g.id == id);
        return thisG;
    }

    //################ Sightings ################

    public void UpdatePlayerSightings() {
        if(sightings.Count > maxSightings) {
            int diff = sightings.Count - maxSightings;
            sightings.RemoveRange(sightings.Count - diff, diff);
        }
        foreach(var s in sightings) {
            s.timePassed += Time.deltaTime;
        }
        sightings.RemoveAll(IsSightingToOld);
    }
    private bool IsSightingToOld(PlayerSighting s) {
        return s.timePassed >= maxSightingTime;
    }
    public void AddSighting(PlayerSighting newS) {
        sightings.Insert(0, newS);
    }
}
