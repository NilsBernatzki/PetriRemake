using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State {

    [SerializeField]
    protected float nonPathHuntSpeed;

    [SerializeField]
    protected float attackSpeed;

    protected override void Start() {
        base.Start();
    }
    public override void Behave() {
        if (enemy == null) return;
        base.Behave();
        //Debug.Log("attack");
       
        if (!enemy.charged) {
            enemy.ChangeBehavior(Behavior.flee);
        }

        if (enemy.playerInSight && Vector3.Distance(playerTransform.position, currentPosition) <= enemy.nonPathHuntDist) {
            if (enemy.currentEnergy == 0) {
                enemy.ChangeBehavior(Behavior.hunt);
                return;
            }
            movement.MoveInstance(enemy.player.FuturePosToTarget(currentPosition), nonPathHuntSpeed, false);
            DoAttack();
            Debug.DrawRay(currentPosition, playerTransform.position - currentPosition);
        } else {
            enemy.ChangeBehavior(Behavior.hunt);
            return;
        }
    }

    protected void DoAttack() {
        if (UnityEngine.Random.Range(0f, 1f) >= 0.99f) {
            movement.MoveInstance(playerTransform.position, attackSpeed, false);
            enemy.currentEnergy -= enemy.looseOnAttack;
        }
    }
    public override void OnEnter() {
        
    }
    public override void OnExit() {
       
    }
}
