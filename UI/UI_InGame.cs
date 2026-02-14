using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image falskImage;

    [SerializeField] private TextMeshProUGUI currentSouls;


    private SkillManager skills;

    void Start()
    {
        if (playerStats != null) {
            playerStats.onHealthChanged += UpdateHealthUI;
        }
        skills = SkillManager.instance;
    }

    void Update()
    {
        currentSouls.text = PlayerManager.instance.GetCurrencyAmount().ToString("#,#");
        if (Input.GetKeyDown(KeyCode.U) && skills.dash.DashUnlocked()) {
            SetCoolDownOf(dashImage);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.parry.ParryUnlocked()) {
            SetCoolDownOf(parryImage);
        }
        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.CrystalUnlocked()) {
            SetCoolDownOf(crystalImage);
        }
        if (Input.GetKeyDown(KeyCode.Mouse2) && skills.sword.SwordUnlocked()) {
            SetCoolDownOf(swordImage);
        }
        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.BlackholeUnlocked()) {
            SetCoolDownOf(blackholeImage);
        }
        if (Input.GetKeyDown(KeyCode.E) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null) {
            SetCoolDownOf(falskImage);
        }
        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(falskImage, Inventory.instance.flaskCooldown);
    }

    private void UpdateHealthUI() {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }
    private void SetCoolDownOf(Image _image) {
        if(_image.fillAmount <= 0) {
            _image.fillAmount = 1;
        }
    }
    private void CheckCooldownOf(Image _image, float _cooldown) {
        if(_image.fillAmount > 0) {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
        }
    }
}
