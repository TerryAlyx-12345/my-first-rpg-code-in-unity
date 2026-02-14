using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundedState {

    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anmiboolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _anmiboolName, enemy) {
    }

    public override void Enter() {
        base.Enter();
        rb = enemy.rb;
    }

    public override void Exit() {
        base.Exit();
    }

    public override void Update() {
        base.Update();
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);
        if(enemy.IsWallDetected() || !enemy.IsGroundedDetected()) {
            enemy.SetVelocity(0, 0);
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
