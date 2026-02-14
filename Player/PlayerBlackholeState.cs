using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerBlackholeState : PlayerState
{
    public float flyTime = .4f;

    private float defaultGravity;
    private bool skillUsed;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName) {
    }

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
    }

    public override void Enter() {
        base.Enter();
        skillUsed = false;
        stateTimer = flyTime;

        defaultGravity = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Exit() {
        base.Exit();
        rb.gravityScale = defaultGravity;
        player.fx.MakeTransprent(false);
        //exit state in blackhole skill controller
    }

    public override void Update() {
        base.Update();
        if (stateTimer > 0) {
            rb.velocity = new Vector2(0, 15);
        }
        if(stateTimer < 0) {
            rb.velocity = new Vector2(0, -.1f);
            if (!skillUsed) {
                Debug.Log("Use Blackhole Skill");
                if (player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }
        if (player.skill.blackhole.SkillComplete() ) {
            stateMachine.ChangeState(player.airState);
        }
    }
}
