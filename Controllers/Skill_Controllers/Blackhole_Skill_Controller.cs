using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKey = true;
    private bool cloneAttackReleased;
    private bool playerCanDisappear = true;


    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createHotKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetUpBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttack, float _cloneAttackCooldown, float _blackholeDuration) {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (SkillManager.instance.clone.CrystalInsteadOfCloneUnlocked()) {
            playerCanDisappear = false;
        }
    }

    public void Update() {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0 || Input.GetKeyDown(KeyCode.S)) {
            blackholeTimer = Mathf.Infinity;
            if (targets.Count > 0){
                ReleaseCloneAttack();
            }
            else {
                FinishBlackholeAbility();
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();
        if (canGrow && !canShrink) {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }
        if (canShrink) {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0) {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack() {
        if(targets.Count <= 0) {
            return;
        }
        cloneAttackReleased = true;
        canCreateHotKey = false;
        if (playerCanDisappear) {
            PlayerManager.instance.player.fx.MakeTransprent(true);
            Debug.Log("Player Disappear");
            playerCanDisappear = false;


        }

    }//only use parameters to flag that the attack is to be released, real operations are in CloneAttackLogic

    private void CloneAttackLogic() {

        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0 && targets.Count > 0) {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = UnityEngine.Random.Range(0, (targets.Count));
            float xOffset = UnityEngine.Random.Range(-2f, 2f);

            if (SkillManager.instance.clone.CrystalInsteadOfCloneUnlocked()) {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;

            if (amountOfAttacks <= 0) {
                Invoke("FinishBlackholeAbility",.9f);
            }
        }
    }
    private void FinishBlackholeAbility() {
        DestroyHotKey();
        canShrink = true;
        cloneAttackReleased = false;
        playerCanExitState = true;//used in PlayerBlackholeState to exit the state
    }//CloneAttackFinish/keycode.S/timeIsOver


    private void DestroyHotKey() {
        if(createHotKey.Count <= 0) {
            return;
        }
        for (int i = 0; i < createHotKey.Count; i++) {
            Destroy(createHotKey[i]);
            Debug.Log("Hotkey Destroyed");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Enemy>() != null) {

            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<Enemy>() != null) {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision) {
        Debug.Log("Create Hotkey");
        if (keyCodeList.Count <= 0) {
            Debug.Log("No more hotkeys available!");
            return;
        }
        if (!canCreateHotKey) {
            Debug.Log("Cannot create hotkey now");
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);

        createHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[UnityEngine.Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_Hotkey_Controller newHotKeyScipt = newHotKey.GetComponent<Blackhole_Hotkey_Controller>();

        newHotKeyScipt.SetupHotKey(choosenKey, collision.transform, this);
        Debug.Log("Hotkey Created");
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
