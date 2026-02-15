using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState {
    public int comboCounter {  get; private set; }
    private float lastTimeAttack;
    private float attackDir;
    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }

    public override void Enter() {
        base.Enter();
        AudioManager.instance.PlaySFX(1, null);
        xInput = 0;
        if (comboCounter > 2 || Time.time >= lastTimeAttack + player.comboWindow)
            comboCounter = 0;
        attackDir = (xInput == 0) ? player.facingDir : xInput;
        player.SetVelocity( attackDir * player.attackMovement[comboCounter].x, player.attackMovement[comboCounter].y);
        player.anim.SetInteger("ComboCounter", comboCounter);
        player.anim.speed = 1;
    }

    public override void Exit() {
        base.Exit();
        player.StartCoroutine("Busyfor", 0.1f);
        lastTimeAttack = Time.time;
        comboCounter++;
        player.anim.speed = 1;
    }

    public override void Update() {
        base.Update();
        player.SetVelocity(attackDir * player.attackMovement[comboCounter].x, player.attackMovement[comboCounter].y);
        if (triggercalled) {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
