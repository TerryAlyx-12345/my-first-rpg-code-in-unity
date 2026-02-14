using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity {
    public bool isBusy { get; private set; }

    [Header("move info")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    [SerializeField]public float dashDuration = 0.8f;
    [SerializeField]public float dashSpeed = 10f;
    private float defaultDashSpeed;
    public int dashDir { get; private set; }

    [Header("Wall Slide info")]
    [SerializeField] private float wallSlideSpeed = 2f; // 控制下滑速度
    [SerializeField] public float wallSlideGravityScale = 0f; // 墙滑时的重力缩放

    [Header("comboTime info")]
    [SerializeField]public float comboWindow= 0.5f;

    [Header("Attack details")]
    [SerializeField]public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public SkillManager skill {  get; private set; }
    public GameObject sword{ get; private set; }



    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttack primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimsword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake() {
        base.Awake();
        PlayerManager.instance.player = this;
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimsword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword" );
        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");

    }

    protected override void Start() {
        stateMachine.Initialize(idleState);
        skill = SkillManager.instance;

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
        skill?.RefreshAllPassiveEffects();
    }

    protected override void Update() {
        stateMachine.currentState.Update();
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.CrystalUnlocked()){
            skill.crystal.CanUseSkill();
        }
            ResetPlayer();

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("use Flask");
            Inventory.instance.UseFlask();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration) {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }
    protected override void ReturnDefaultSpeed() {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }
    public void ResetPlayer() {
        Player player = GetComponent<Player>();
        if(player.transform.position.y < -100) {
            player.transform.position = new Vector2(0,0);

        }
    }

    public void AssignNewSword(GameObject _newSword) {
        sword = _newSword;
    }

    public void CatchTheSword() {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator Busyfor(float _second) {
        isBusy = true;
        yield return new WaitForSeconds(_second);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    #region Dash
    public int SetDashDir() => Input.GetAxisRaw("Horizontal") != 0 ? (int)Input.GetAxisRaw("Horizontal") : facingDir;

    public void checkForDashInput() {

        if (skill.dash.DashUnlocked() == false) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.U) && SkillManager.instance.dash.CanUseSkill()) {
            stateMachine.ChangeState(dashState);
        }
    }
    #endregion

    public override void Die() {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}