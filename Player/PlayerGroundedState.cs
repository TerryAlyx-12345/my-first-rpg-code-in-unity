using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState {
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }
    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }
    public override void Update() {
        base.Update();
        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.BlackholeUnlocked()) {
            stateMachine.ChangeState(player.blackholeState);
        }
        if (Input.GetKeyDown(KeyCode.Mouse2) && HasNoSword() && player.skill.sword.SwordUnlocked()) {
            stateMachine.ChangeState(player.aimsword);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && player.skill.parry.ParryUnlocked()) {
            stateMachine.ChangeState(player.counterAttack);
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K)) {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Mouse0)) {
            stateMachine.ChangeState(player.primaryAttack);
            return;
        }
    }

    private bool HasNoSword() {
        if (!player.sword) {
            return true;
        }
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}