using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skills
{
    [Header("Dash")]
    [SerializeField] public UI_SkillTreeSlot dashUnlockButton;

    [Header("Clone on dash")]
    [SerializeField] public UI_SkillTreeSlot cloneOnDashUnlockButton;

    [Header("Clone on arrival")]
    [SerializeField] public UI_SkillTreeSlot cloneOnArrivalUnlockButton;

    private void Awake() {
        SkillManager.instance.RegisterDash(this);
        
    }
    protected override void Start() {
        base.Start();
    }
    public bool DashUnlocked() => dashUnlockButton.unlocked;
    public bool CloneOnDashUnlocked() => cloneOnDashUnlockButton.unlocked;
    public bool CloneOnArrivalUnlocked() => cloneOnArrivalUnlockButton.unlocked;
    public override void UseSkill() {
        base.UseSkill();
    }

    public void CloneOnDash() {
        if (CloneOnDashUnlocked()) {
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
        }
    }
    public void CloneOnArrival() {
        if (CloneOnArrivalUnlocked()) {
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
        }
    }
}
