using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState {
// Start is called before the first frame update
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void Update() {
        base.Update();
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
        if(player.IsGroundedDetected()){
            stateMachine.ChangeState(player.idleState);
        }
    }
}
