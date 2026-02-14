using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skills {

    [SerializeField]private float crystalDuaration = 1.5f;
    [SerializeField]private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;

    [Header("crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadOfCrystalButton;

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalCanExploreButton;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalCanMoveToEnemyButton;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiCrystalButton;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    private void Awake() {
        SkillManager.instance.RegisterCrystal(this);
    }
    protected override void Start() {
        base.Start();
    }
    #region unlock skill region
    public bool CrystalUnlocked() => unlockCrystalButton.unlocked;
    public bool CrystalCanMoveToEnemyUnlocked() => unlockCrystalCanMoveToEnemyButton.unlocked;
    public bool CrystalMirageUnlocked() => unlockCloneInsteadOfCrystalButton.unlocked;
    public bool CrystalCanExploreUnlocked() => unlockCrystalCanExploreButton.unlocked;
    public bool MultiCrystalUnlocked() => unlockMultiCrystalButton.unlocked;
    #endregion

    public override bool CanUseSkill() {
        return base.CanUseSkill();
    }

    public override void UseSkill() {
        base.UseSkill();

        if (CanUseMultiCrystal()) {
            return;
        }

        if (currentCrystal == null) {
            CreateCrystal();
        }

        else {
            if (CrystalCanMoveToEnemyUnlocked()) {
                return;
            }
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            // to be modified now: crystal cannot be throwed want: crystal can be throwed out for ~ seconds
            if (CrystalMirageUnlocked()) {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }


        }
    }

    public void CreateCrystal() {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position + new Vector3(0, 0), Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuaration, CrystalCanExploreUnlocked(), CrystalCanMoveToEnemyUnlocked(), moveSpeed, FindClosestEnemy(currentCrystal.transform),player);
        currentCrystalScript.ChooseRandomEnemy();
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal() {
        if (MultiCrystalUnlocked()) {
            //respawn crystal
            if (crystalLeft.Count > 0) {
                if(crystalLeft.Count == amountOfStacks) {
                    Invoke("ResestAbility", useTimeWindow);
                }
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuaration, CrystalCanExploreUnlocked(), CrystalCanMoveToEnemyUnlocked(), moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if(crystalLeft.Count <= 0) {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                    //refill crystal
                }
                return true;
            }
        }
        return false;
    }
    private void RefilCrystal() {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++) {
            crystalLeft.Add(crystalPrefab);
        }
    }
    private void ResestAbility() {
        if(cooldownTimer > 0) {
            return;
        }
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }

}
