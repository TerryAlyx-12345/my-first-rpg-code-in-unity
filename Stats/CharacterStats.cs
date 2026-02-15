using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public enum StatsType {
    strength,
    agility,
    intelligence,
    vitality,

    damage,
    critChance,
    critPower,

    health,
    armor,
    evasion,
    magicRes,

    fireDamage,
    iceDamage,
    lightningDamage
}

public class CharacterStats : MonoBehaviour {
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;//1 point increase damage by 1 and crit.power by 1%
    public Stat agility;//1 point increase evasion by 1% and crit.chance by 1%
    public Stat intelligence;//1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality;//1 point increase health by 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgniting;   //do damage over time
    public bool isChilled;    //reduce armor 20%
    public bool isShocked;    //reduce accuracy by 20%

    [SerializeField] private float alimentsDuration = 4;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    [SerializeField] private GameObject shockStrikePrefab;
    [SerializeField] private int shockDamage;

    public int currentHealth;
    public System.Action onHealthChanged;

    public bool isDead {  get; private set; } = false;
    private bool isVulnerable;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }
    protected virtual void Update() {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;
        if (ignitedTimer < 0) {
            isIgniting = false;
        }
        if (chilledTimer < 0) {
            isChilled = false;
        }
        if (shockedTimer < 0) {
            isShocked = false;
        }
        if (isIgniting) {
            ApplyIgniteDamage();
        }

    }
    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableForCoroutine(_duration));
    private IEnumerator VulnerableForCoroutine(float _duration) {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statModify) {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statModify));

    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statModify) {
        _statModify.AddModifiers(_modifier);
        Inventory.instance.UpdateSlotUI();

        yield return new WaitForSeconds(_duration);

        _statModify.RemoveModifiers(_modifier);
        Inventory.instance.UpdateSlotUI();
    }

    public virtual void DoDamage(CharacterStats _targetStats) { 
        if (TargetCanAvoidAttack(_targetStats)) {
            return;
        }
        int baseDamage = damage.GetValue();
        int strengthBonus = strength.GetValue();
        int totalDamage = damage.GetValue() + strength.GetValue();
        if (CanCrit()) {
            totalDamage = CaculateCritDamage(totalDamage);        
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);// REMOVE if you don't want to apply magic hit on prime attack
    }// do damage caculation using stats methods
    public virtual void TakeDamage(int _damage) {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && isDead == false)
            Die();
    }// really decrease health and judge when to die
    protected virtual void DecreaseHealthBy(int _damage) {
        if (isVulnerable) {
            _damage = Mathf.RoundToInt(_damage * 1.1f);
        }
        currentHealth -= _damage;
        if (onHealthChanged != null) onHealthChanged();
    }

    public virtual void IncreaseHealthBy(int _amount) {
        currentHealth += _amount;
        if(currentHealth > GetMaxHealthValue()) {
            currentHealth = GetMaxHealthValue();
        } 
        if(onHealthChanged!= null) onHealthChanged();
    }
    protected virtual void Die() {
        isDead = true;
    }

    #region Magical Damage and Aliments
    public virtual void DoMagicalDamage(CharacterStats _targetStat) {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStat, totalMagicalDamage);

        _targetStat.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_lightningDamage, _iceDamage, _fireDamage) <= 0) {
            return;
        }

        AttemptToApplyAliments(_targetStat, _fireDamage, _iceDamage, _lightningDamage);

    }

    private void AttemptToApplyAliments(CharacterStats _targetStat, int _fireDamage, int _iceDamage, int _lightningDamage) {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;
        while (!canApplyChill && !canApplyIgnite && !canApplyShock) {
            float randomNumber = UnityEngine.Random.value;
            if (randomNumber <= 1f / 3 && _fireDamage > 0) {
                canApplyIgnite = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (randomNumber > 1f / 3 && randomNumber <= 2f / 3 && _iceDamage > 0) {
                canApplyChill = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (randomNumber > 2f / 3 && _lightningDamage > 0) {
                canApplyShock = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }
        if (canApplyIgnite) {
            _targetStat.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }
        if (canApplyShock) {
            _targetStat.SetupShockDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        }
        _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAliments(bool _ignite, bool _chill, bool _shock) {
        bool canApplyIgnite = !isIgniting && !isChilled && !isShocked;
        bool canApplyChill = !isIgniting && !isChilled && !isShocked;
        bool canApplyShock = !isIgniting && !isChilled;

        if (_ignite && canApplyIgnite) {
            isIgniting = _ignite;
            ignitedTimer = alimentsDuration;

            fx.IgniteFxFor(alimentsDuration);
        }
        if (_chill && canApplyChill) {
            isChilled = _chill;
            chilledTimer = alimentsDuration;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, alimentsDuration);

            fx.ChillFxFor(alimentsDuration);
        }
        if (_shock && canApplyShock) {
            if (!isShocked) {
                ApplyShock(_shock);
            }
            else {
                if (GetComponent<Player>() != null) {
                    return;
                }
                HitNearestTargetWithShockStrike();

            }
        }

    }

    public void ApplyShock(bool _shock) {
        if (isShocked) {
            return;
        }
        isShocked = _shock;
        shockedTimer = alimentsDuration;

        fx.ShockFxFor(alimentsDuration);
    }
    private void ApplyIgniteDamage() {
        if (igniteDamageTimer < 0) {
            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
                Die();
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    private void HitNearestTargetWithShockStrike() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders) {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1) {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance) {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null) {
                closestEnemy = transform;
            }
        }
        if (closestEnemy != null) {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>()?.Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(int _damage) =>igniteDamage = _damage;
    public void SetupShockDamage(int _damage) =>shockDamage = _damage;
    #endregion

    #region Stat Caculations

    public virtual void OnEvasion() {

    }
    protected bool TargetCanAvoidAttack(CharacterStats _targetStats) {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (isShocked)
            totalEvasion += 20;
        if (UnityEngine.Random.Range(0, 100) < totalEvasion) {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage) {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    private int CheckTargetResistance(CharacterStats _targetStat, int totalMagicalDamage) {
        totalMagicalDamage -= _targetStat.magicResistance.GetValue() + (_targetStat.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }
    protected bool CanCrit() {
        int totalCritChance = critChance.GetValue() + agility.GetValue();
        if(UnityEngine.Random.Range(0,100) <= totalCritChance) {
            return true;
        }
        return false;
    }
    protected int CaculateCritDamage(int _damage) {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() {
        return maxHealth.GetValue() + (vitality.GetValue() * 5);
    }
    #endregion

    public Stat GetStat(StatsType _statsType) {
        if (_statsType == StatsType.strength) return strength;
        else if (_statsType == StatsType.agility) return agility;
        else if (_statsType == StatsType.intelligence) return intelligence;
        else if (_statsType == StatsType.vitality) return vitality;
        else if (_statsType == StatsType.damage) return damage;
        else if (_statsType == StatsType.critChance) return critChance;
        else if (_statsType == StatsType.critPower) return critPower;
        else if (_statsType == StatsType.health) return maxHealth;
        else if (_statsType == StatsType.armor) return armor;
        else if (_statsType == StatsType.evasion) return evasion;
        else if (_statsType == StatsType.magicRes) return magicResistance;
        else if (_statsType == StatsType.fireDamage) return fireDamage;
        else if (_statsType == StatsType.iceDamage) return iceDamage;
        else if (_statsType == StatsType.lightningDamage) return lightningDamage;

        return null;
    }
}