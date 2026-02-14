using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skills
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0f,1f)]
    [SerializeField] private float restoreHealthAmount;

    [Header("Parry with mirage")]
    [SerializeField]private UI_SkillTreeSlot parryWithMirageUnlockButton;


    private void Awake() {
        SkillManager.instance.RegisterParry(this);
    }
    protected override void Start() {
        base.Start();
    }
    public override void UseSkill() {
        base.UseSkill();
        if (ParryRestoreUnlocked()) {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restoreHealthAmount);
            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    public bool ParryUnlocked() => parryUnlockButton.unlocked;
    public bool ParryRestoreUnlocked() => restoreUnlockButton.unlocked;
    public bool ParryWithMirageUnlocked() => parryWithMirageUnlockButton.unlocked;
    public void MakeMirageOnParry(Transform _respawnTransform) {
        if (ParryWithMirageUnlocked()) {
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
        }
    }
}
