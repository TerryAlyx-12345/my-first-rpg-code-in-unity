using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skills
{


    [Header("clone info")]
    [SerializeField] private float attackMultiiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField]private float aggresiveCloneAttackMultiplier;

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleCloneUnlockButton;
    [SerializeField]private float multipleCloneAttackMultiplier;
    [SerializeField] private float chanceToDuplicate = 35;

    [Header("crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadOfCloneUnlockButton;

    private void Awake() {
        SkillManager.instance.RegisterClone(this);
    }
    protected override void Start() {
        base.Start();
    }

    #region unlock region
    public bool CloneAttackUnlocked() => cloneAttackUnlockButton.unlocked;
    public bool AggresiveCloneUnlocked() => aggresiveCloneUnlockButton.unlocked;
    public bool MultipleCloneUnlocked() => multipleCloneUnlockButton.unlocked;
    public bool CrystalInsteadOfCloneUnlocked() => crystalInsteadOfCloneUnlockButton.unlocked;
    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset) {
        if (CrystalInsteadOfCloneUnlocked()) {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetUpClone(_clonePosition, cloneDuration, CloneAttackUnlocked(), _offset, FindClosestEnemy(newClone.transform), MultipleCloneUnlocked(), chanceToDuplicate, player,attackMultiiplier);
    }


    public void CreateCloneWithDelay(Transform _enemyTransform) {
        StartCoroutine(CreateDelayCoroutine(_enemyTransform,new Vector3(2 * player.facingDir,0)));
    }

    private IEnumerator CreateDelayCoroutine(Transform _transform, Vector3 _offset) {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}
