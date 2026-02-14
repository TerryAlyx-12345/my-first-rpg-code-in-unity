using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealthBar : UI_HealthBar
{
    protected override void Awake() {

    }

    protected override void Start() {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        myStats = PlayerManager.instance.player.stats;
        myStats.onHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }
    
    protected override void OnDisable() {
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}
