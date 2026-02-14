using System.Collections;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour {
    private float crystalExistTimer;
    private Player player;

    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private bool canExplore;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplore, bool _canMove, float _moveSpeed, Transform _closestTargets,Player _player) {
        crystalExistTimer = _crystalDuration;
        canExplore = _canExplore;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTargets;
        player = _player;
    }

    public void ChooseRandomEnemy() {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
        if(colliders.Length > 0)
            closestTarget = colliders[Random.Range(0,colliders.Length)].transform;
    }
    private void Update() {
        crystalExistTimer -= Time.deltaTime;
        if (crystalExistTimer < 0) {
            FinishCrystal();
        }
        if (canMove) {
            if(closestTarget == null) {
                return;
            }
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position,closestTarget.position) < 0.5) {
                FinishCrystal();
                canMove = false;
            }
        }
        if (canGrow) {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExploreEvent() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders) {
            if (hit.GetComponent<Enemy>() != null) {
               player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());// to be modified ,now: if player don't have magicDamage, crystal won't cause any damage 

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
                if (equipedAmulet != null) {
                    equipedAmulet.ApplyItemEffects(hit.transform);
                }
            }
        }
    }
    public void StartGrowing() {
        canGrow = true;
    }

    public void FinishCrystal() {
        if (canExplore) {
            anim.SetTrigger("Explore");
        }
        else {
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject); 
}