using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    protected bool trigggerCalled;
    protected string anmiboolName;
    protected float stateTimer;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anmiboolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.anmiboolName = _anmiboolName;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }
    public virtual void Enter()
    {
        trigggerCalled = false;
        enemyBase.anim.SetBool(anmiboolName, true);
        rb = enemyBase.rb;
    }
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(anmiboolName, false);
        enemyBase.AssignLastAnimName(anmiboolName);
    }
    public virtual void AnimationFinishTrigger() {
        trigggerCalled = true;
    }
}
