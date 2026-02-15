using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anmiboolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _anmiboolName, enemy) {
    }

    public override void Enter() {
        base.Enter();
        enemy.SetZeroVelocity();
        stateTimer = enemy.idleTime;
    }

    public override void Exit() {
        base.Exit();
        AudioManager.instance.PlaySFX(7,enemy.transform);
    }

    public override void Update() {
        base.Update();
        if (stateTimer < 0) {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
