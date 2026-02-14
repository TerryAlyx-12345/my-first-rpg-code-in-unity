using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState {
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }
    int dashDir;
    public override void Enter() {
        dashDir = player.SetDashDir();
        stateTimer = player.dashDuration;
        base.Enter();
        player.skill.dash.CloneOnDash();
    }

    public override void Exit() {
        player.SetVelocity(0, 0);
        player.skill.dash.CloneOnArrival();
        base.Exit();
    }

    public override void Update() {
        base.Update(); 
        player.SetVelocity(dashDir * player.dashSpeed, 0);
        if (stateTimer < 0) {
            stateMachine.ChangeState(player.airState);
        }
    }
}
