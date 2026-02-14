using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skills {

    [SerializeField] private UI_SkillTreeSlot blackUnlockButton;
    [SerializeField]private float cloneAttackCooldown = .3f;
    [SerializeField]private int amountOfAttacks = 4;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField]private GameObject blackholePrefab;

    [SerializeField]private float maxSize = 5f;
    [SerializeField]private float growSpeed = 1f;
    [SerializeField]private float shrinkSpeed = 5f;

    Blackhole_Skill_Controller  currentBlackhole;
    private void Awake() {
        SkillManager.instance.RegisterBlackhole(this);
    }
    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public bool BlackholeUnlocked() =>blackUnlockButton.unlocked;
    public override bool CanUseSkill() {
        return base.CanUseSkill();
    }

    public override void UseSkill() {
        base.UseSkill();
        GameObject newBlackhole = Instantiate(blackholePrefab,player.transform.position,Quaternion.identity);
        currentBlackhole = newBlackhole.GetComponent<Blackhole_Skill_Controller>();
        currentBlackhole.SetUpBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneAttackCooldown, blackholeDuration);
    }


    public bool SkillComplete() {

        if (!currentBlackhole) {
            return false;
        }
        if (currentBlackhole.playerCanExitState) {
            currentBlackhole = null;
            return true;
        }
        return false;
    }
    public float GetBlackholeRadius() {
        return maxSize / 2;
    }
}
