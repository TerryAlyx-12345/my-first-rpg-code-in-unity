using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private float returnSpeed;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;

    [Header("Pierce info")]
    private int pierceAmount;

    [Header("Bounce Info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount; 
    public List<Transform> enemyTarget;
    private int TargetIndex;

    [Header("spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStoppped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void Update() {

        if (isReturning) {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 0.1) {
                player.CatchTheSword();
            }
        }
        if (canRotate) {
            transform.right = rb.velocity;//The red axis of the transform in world space.
        }
        BounceLogic();
        SpinLogic();
    }


    private void DestroyMe() {
        Destroy(gameObject);
    }
    public void SetUpSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed) {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
        Invoke("DestroyMe", 7);
    }
    private void SpinLogic() {
        if (isSpinning) {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStoppped) {
                StopWhenSpinning();
            }
            if (wasStoppped) {
                spinTimer -= Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                if (spinTimer < 0) {
                    isReturning = true;
                    isSpinning = false;
                }
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0) {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders) {
                        SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    private void StopWhenSpinning() {
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if(wasStoppped == false) {
            spinTimer = spinDuration;
            wasStoppped = true;
        }
    }

    private void BounceLogic() {
        if (isBouncing && enemyTarget.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[TargetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[TargetIndex].position) < .1f) {
                SwordSkillDamage(enemyTarget[TargetIndex].GetComponent<Enemy>());

                TargetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0) {
                    isBouncing = false;
                    isReturning = true;
                }
                if (TargetIndex >= enemyTarget.Count) {
                    TargetIndex = 0;
                }
            }
        }
    }


    public void SetUpBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed) {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;
    }

    public void SetUpSpin(bool _isSpinninng, float _maxTravelDistance, float _spinDuration, float _hitCooldown) {
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        isSpinning = _isSpinninng;
        hitCooldown = _hitCooldown;
    }

    public void SetupPierce(int _pierceAmount) {
        pierceAmount = _pierceAmount;
    }
    public void ReturnSword() {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;

        // to be modified sword skill need to set cool down
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isReturning) {
            return;
        }
        if(collision.GetComponent<Enemy>() != null) {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        SetupTargetsForBounce(collision);
        StuckInto(collision);
        //Unity内置的MonoBehaviour消息方法。当挂载该脚本的GameObject上有Collider组件并且开启了IsTrigger时，
        //Unity会在另一个带有Collider的GameObject进入这个触发器范围自动调用它：
    }

    private void SwordSkillDamage(Enemy enemy) {
        if (enemy == null) {
            return;
        }
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(enemyStats);

        if (player.skill.sword.TimeStopUnlocked()) {
            enemy.FreezeTimeFor(freezeTimeDuration);

        }
        if (player.skill.sword.VulnerableUnlocked()) {
            enemyStats.MakeVulnerableFor(SkillManager.instance.sword.vulnableDuration);
        }


        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equipedAmulet != null) {
            equipedAmulet.ApplyItemEffects(enemy.transform);
            Debug.Log("apply amulet effect");
        }
    }

    private void SetupTargetsForBounce(Collider2D collision) {
        if (collision.GetComponent<Enemy>() != null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            foreach (var hit in colliders) {
                if (hit.GetComponent<Enemy>() != null) {
                    enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision) {
        if(pierceAmount > 0 && collision.GetComponent<Enemy>()) {
            pierceAmount--;
            return;
        }
        if (isSpinning) {
            StopWhenSpinning();
            return;
        }
        canRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0) {
            return;
        }
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
