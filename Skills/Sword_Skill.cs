using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType {
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skills {
    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bouceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed = 15;

    [Header("Peirce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int peirceAmount = 0;
    [SerializeField] private float peirceGravity;

    [Header("spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinGravity = 1;

    [Header("skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float returnSpeed = 200;
    [SerializeField] private float freezeTimeDuration = 1f;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    [SerializeField] private UI_SkillTreeSlot volnurableUnlockButton;
    [SerializeField]public float vulnableDuration = 5f;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField]private int numberOfDots;
    [SerializeField]private float spaceBetweenDots;
    [SerializeField]private GameObject dotPrefab;
    [SerializeField]private Transform dotsParent;

    private GameObject[] dots;
    private void Awake() {
        SkillManager.instance.RegisterSword(this);
    }
    protected override void Start() {
        base.Start();
        GenerateDots();
        SetupGravity();

    }

    protected override void Update() {
        if (Input.GetKeyUp(KeyCode.Mouse2)) {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }
        if (Input.GetKey(KeyCode.Mouse2)) {
            for (int i = 0; i < dots.Length; i++) {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    #region unlock region
    public bool SwordUnlocked() => swordUnlockButton.unlocked;
    public bool TimeStopUnlocked() => timeStopUnlockButton.unlocked;
    public bool VulnerableUnlocked() => volnurableUnlockButton.unlocked;
    public bool BounceUnlocked() => bouceUnlockButton.unlocked;
    public bool PierceUnlocked() => pierceUnlockButton.unlocked;
    public bool SpinUnlocked() => spinUnlockButton.unlocked;
    public SwordType CurrentSwordType {
        get {
            if (bouceUnlockButton.unlocked) return SwordType.Bounce;
            if (pierceUnlockButton.unlocked) return SwordType.Pierce;
            if (spinUnlockButton.unlocked) return SwordType.Spin;
            return SwordType.Regular;
        }
    }
    #endregion

    private void SetupGravity() {
        switch (CurrentSwordType) {
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = peirceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
        }
    }

    public void CreatSword() {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScrip = newSword.GetComponent<Sword_Skill_Controller>();

        switch (CurrentSwordType) {
            case SwordType.Bounce:
                newSwordScrip.SetUpBounce(true, bounceAmount, bounceSpeed);
                break;
            case SwordType.Pierce:
                newSwordScrip.SetupPierce(peirceAmount);
                break;
            case SwordType.Spin:
                newSwordScrip.SetUpSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                break;
        }
        newSwordScrip.SetUpSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);
        DotsActive(false);
    }

    #region Aim Region
    public Vector2 AimDirection() {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;
        return direction;
    }


    public void DotsActive(bool _isActive) {
        for (int i = 0; i < dots.Length; i++) {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots() {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++) {
            dots[i] = Instantiate(dotPrefab,player.transform.position,Quaternion.identity,dotsParent);
            dots[i].SetActive(false);
        }
    }
    // 在每次循环中，实例化一个dotPrefab（点的预制体）
    // 实例化的位置是player.transform.position（玩家的位置）
    // 旋转为Quaternion.identity（无旋转）
    // 父物体为dotsParent（一个Transform，用于组织这些点，使层次结构清晰）

    private Vector2 DotsPosition(float t) {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x, 
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
         return position;
    }
    #endregion
}
