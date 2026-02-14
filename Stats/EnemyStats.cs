using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats {
    private Enemy enemy;
    private ItemDrop myDropSystem;

    [Header("Level details")]
    [SerializeField]private int level = 1;
    [Range(0f,1f)]
    [SerializeField] private float persentageModifier = .2f;
    protected override void Start() {
        ApplyLevelModifiers();
        base.Start();
        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers() {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(maxHealth);
    }

    private void Modify(Stat _stat) {
        for (int i = 1; i < level; i++) {
            float modiFier = _stat.GetValue() * persentageModifier;
            _stat.AddModifiers(Mathf.RoundToInt(modiFier));
        }
    }
    public override void TakeDamage(int _damage) {
        base.TakeDamage(_damage);
    }

    protected override void Die() {
        base.Die();
        enemy.Die();
        myDropSystem.GenerateDrop();
    }

}
