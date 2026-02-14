using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skills
{
    [Header("Dodge")]
    [SerializeField] UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount = 10;

    [Header("Mirage dodge")]
    [SerializeField] UI_SkillTreeSlot unlockMirageDodgeButton;

    private bool effectApplied = false;
    private void Awake() {
        SkillManager.instance.RegisterDodge(this);
    }
    protected override void Start() {
        base.Start();
    }
    public bool DodgeUnlocked() => unlockDodgeButton.unlocked;
    public bool DodgeMirageUnlocked() => unlockMirageDodgeButton.unlocked;

    private void DodgeEffect() {
        player.stats.evasion.AddModifiers(evasionAmount);
        Inventory.instance?.UpdateSlotUI();
    }
    public void RefreshPassiveEffect() {
        if (player == null) {
            player = PlayerManager.instance?.player;
            if (player == null) {
                //Debug.LogWarning("Player not ready, will retry later.");
                return;
            }
        }
        bool shouldBeApplied = DodgeUnlocked();

        if (shouldBeApplied && !effectApplied) {
            DodgeEffect();
            effectApplied = true;
        }
    }

    public void CreateMirageOnDodge() {
        if (DodgeMirageUnlocked()) {
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
        }
    }
}
