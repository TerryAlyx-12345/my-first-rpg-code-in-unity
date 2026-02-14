using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats {

    private Player player;
    private void Awake() {
        player = GetComponent<Player>();
    }
    protected override void Start() {
        base.Start();
        SkillManager.instance?.RefreshAllPassiveEffects();
    }
    public override void TakeDamage(int _damage) {
        base.TakeDamage(_damage);
    }

    protected override void Die() {
        base.Die();
        player.Die();

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage) {
        base.DecreaseHealthBy(_damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null) {
            currentArmor.ApplyItemEffects(player.transform);
        }
    }

    public override void OnEvasion() {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier) {
        if (TargetCanAvoidAttack(_targetStats)) {
            return;
        }
        int totalDamage = damage.GetValue() + strength.GetValue();
        if(_multiplier > 0) {
            totalDamage = Mathf.RoundToInt(_multiplier * totalDamage);
        }
        if (CanCrit()) {
            totalDamage = CaculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);// REMOVE if you don't want to apply magic hit on prime attack
    }
}
