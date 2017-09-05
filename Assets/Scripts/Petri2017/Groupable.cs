using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ConnectionTest {
    public float id;
    public List<float> connectedIds;
    public bool connected;
}

public class Groupable : MonoBehaviour {
    [Header("Debug")]
    private SpriteRenderer spriteRenderer;
    private Color startCol;
    [HideInInspector]
    public Color inGroupColor;
    [SerializeField]
    private Color leaderColor;
    
    [Space(10)]
    public float id;
    public Enemy enemy;

    public List<Groupable> group = new List<Groupable>();
    public List<Groupable> neighbors = new List<Groupable>();
    public List<Groupable> neighborsInGroup = new List<Groupable>();

    public Groupable leader;
    private int maxGroupCount;

    [SerializeField]
    private int updateGroupFrameCount;
    private int updateGroupFrameCounter;

    private CircleCollider2D triggerCollider;
    private bool reseting;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startCol = spriteRenderer.color;
        inGroupColor = Random.ColorHSV(0.5f, 1, 0.5f, 1, 0.5f, 1, 1, 1);
        enemy = GetComponent<Enemy>();
        triggerCollider = GetComponent<CircleCollider2D>();
        id = Random.Range(0, 100f);
        leader = this;
        group.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
        maxGroupCount = SwarmManager.singleton.currentMaxGroupSize;
        if (!triggerCollider.enabled && !reseting) {
            reseting = true;
            StartCoroutine(ResetCollider());
        }
        if (neighbors.Count == 0 && leader.id != id) {
            ResetMySelf();
        }

        updateGroupFrameCounter += Mathf.RoundToInt(Random.Range(0, 2));
        if (WaitedEnoughFrames(ref updateGroupFrameCounter, updateGroupFrameCount)) {

            UpdateBoidNeighbors();
            if (leader.id == id) {
                UpdateLeader();
            } else {
                UpdateOther();
            }
        }
        DebugGroup();
    }
    private IEnumerator ResetCollider() {
        yield return new WaitForSeconds(1f);
        triggerCollider.enabled = true;
        reseting = false;
    }
    private void UpdateLeader() {

        if (neighbors.Count == 0 && group.Count != 1) {
            group.Clear();
            group.Add(this);
            return;
        }
        for (int i = 0; i < group.Count; i++) {
            if (group[i].leader.id != id) {
                group.RemoveAt(i);
            }
        }

        if (group.Count > SwarmManager.singleton.currentMaxGroupSize) {
            
            while (group.Count > SwarmManager.singleton.currentMaxGroupSize) {
                Groupable farthest = group.OrderByDescending(g => Vector3.Distance(g.transform.position, transform.position)).First();
                farthest.ResetMySelf();
                triggerCollider.enabled = false;
            }
        }
    }
    private void UpdateOther() {
        if (neighbors.Count == 0) return;
        if (group.Count != 1) {
            group.Clear();
            group.Add(this);
        }

        if (neighbors.Contains(leader)) return;
        
        ConnectionTest c = ConnectionToID(leader.id);
        if (c.connected) return;
        float newLeaderID = c.connectedIds.OrderByDescending(id => id).First();
        Groupable newLeader = SwarmManager.singleton.GetGroupableFromID(newLeaderID);

        foreach(float id in c.connectedIds) {
            SwarmManager.singleton.GetGroupableFromID(id).ResetOnNewLeader(newLeader);
        }
    }
    private void UpdateBoidNeighbors() {
        foreach (var g in neighbors) {
            if (g.leader.id == leader.id) {
                if (!neighborsInGroup.Contains(g)) {
                    neighborsInGroup.Add(g);
                }
            } else {
                if (neighborsInGroup.Contains(g)) {
                    neighborsInGroup.Remove(g);
                }
            }
        }
        for (int i = 0; i < neighborsInGroup.Count; i++) {
            if (!leader.group.Contains(neighborsInGroup[i]) || !neighbors.Contains(neighborsInGroup[i])) {
                neighborsInGroup.RemoveAt(i);
            }
        }
    }

    private void ResetMySelf() {
        if (leader.group.Contains(this)) {
            leader.group.Remove(this);
        }
        leader = this;
        group.Clear();
        group.Add(this);
        enemy.ResetPath();
    }

    private void ResetOnNewLeader(Groupable newLeader) {
        if (leader.group.Contains(this)) {
            leader.group.Remove(this);
        }
        leader = newLeader;
        if(leader.id != id) {
            group.Clear();
            group.Add(this);
        } 
        enemy.ResetPath();
    }

    private void OnTriggerEnter2D(Collider2D c) {
        if (!c.CompareTag("Enemy")) return;
        Groupable g = c.GetComponent<Groupable>();
        if (!neighbors.Contains(g)) {
            neighbors.Add(g);
        }
        if (leader.id > g.leader.id && leader.group.Count < maxGroupCount) {
            AddToGroup(g);
        }
    }
    private void AddToGroup(Groupable g) {
        if (leader.group.Contains(g)) return;
        leader.group.Add(g);
        g.leader = leader;
        g.enemy.ResetPath();
    }
    private void OnTriggerExit2D(Collider2D c) {
        if (!c.CompareTag("Enemy")) return;
        Groupable g = c.GetComponent<Groupable>();
        if (neighbors.Contains(g)) {
            neighbors.Remove(g);
        }
    }
    
    private bool IsConnectedToGroupableID(float gID) {
        bool connectedToG = false;
        List<float> tempIDList = new List<float>();
        tempIDList.Add(id);
        FindGroupableRekursive(ref connectedToG, gID, ref tempIDList);
        return connectedToG;
    }
    
    private ConnectionTest ConnectionToID(float gID) {
        ConnectionTest connection;
        connection.id = gID;
        connection.connectedIds = new List<float>();
        connection.connected = false;

        connection.connectedIds.Add(id);
        FindGroupableRekursive(ref connection);
        return connection;
    }

    public void FindGroupableRekursive(ref bool connected,float searchedID, ref List<float> idList) {
        foreach(Groupable g in neighbors) {
            if (connected == true) break;
            if (g.leader.id != leader.id) continue;
            if (idList.Contains(g.id)) continue;
            if (g.id == searchedID) { connected = true; break; };
            idList.Add(g.id);
            g.FindGroupableRekursive(ref connected, searchedID, ref idList);
        }
    }
    
    public void FindGroupableRekursive(ref ConnectionTest connection) {
        foreach (Groupable g in neighbors) {
            if (connection.connected == true) break;
            if (g.leader.id != leader.id) continue;
            if (connection.connectedIds.Contains(g.id)) continue;
            if (g.id == connection.id) { connection.connected = true; break; };
            connection.connectedIds.Add(g.id);
            g.FindGroupableRekursive(ref connection);
        }
    }
    public bool IsLeader() {
        if(id == leader.id) {
            return true;
        } else {
            return false;
        }
    }
    protected bool WaitedEnoughFrames(ref int frameCounter, int frameCount) {
        if (frameCounter >= frameCount) {
            frameCounter = 0;
            return true;
        } else {
            return false;
        }
    }
    public float GetNeighborsRadius() {
        return triggerCollider.radius;
    }

    //############# Debug ##################

    private void DebugGroup() {
        if (SwarmManager.singleton.debugGroupsMode && !SwarmManager.singleton.debugEnergyMode) {
            if (leader == this) {
                ChangeColor(leaderColor);
            } else {
                ChangeColor(leader.inGroupColor);
            }
        } else {
            if (!SwarmManager.singleton.debugEnergyMode) {
                ChangeColor(startCol);
            }
        }
    }
    public void ChangeColor(Color newColor) {
        if(spriteRenderer.color != newColor) {
            spriteRenderer.color = newColor;
        }
    }
}
