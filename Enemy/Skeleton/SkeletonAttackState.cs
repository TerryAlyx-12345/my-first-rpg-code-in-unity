using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState {
    private Enemy_Skeleton enemy;
    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anmiboolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _anmiboolName) {
        this.enemy = enemy;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update() {
        base.Update();
        enemy.SetZeroVelocity();
        if (trigggerCalled) {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
