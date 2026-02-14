using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState {
    private float originalGravityScale;
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void Update() {
        base.Update();
        if (xInput != 0 && player.facingDir != xInput) {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if(yInput < 0) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else {
              rb.velocity = new Vector2(0,0);
        }
        if (player.IsGroundedDetected()) {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Space)) {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }
    }
}
