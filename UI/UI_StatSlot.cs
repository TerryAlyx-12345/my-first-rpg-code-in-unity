using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;

    [SerializeField] private StatsType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate() {
        gameObject.name = "Stat - " + statName;
        if(statNameText != null) {
            statNameText.text = statName;
        }

    }

    private void Start() {

        UpdateStatValueUI();
        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI() {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats != null) {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();


            if (statType == StatsType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();
            if(statType == StatsType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
            if(statType == StatsType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();
            if(statType == StatsType.critChance)
               statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();
            if (statType == StatsType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
            if(statType == StatsType.magicRes)
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue()) * 5).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ui.ui_StatTooltip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData) {
        ui.ui_StatTooltip.HideStatToolTip();
    }
}
