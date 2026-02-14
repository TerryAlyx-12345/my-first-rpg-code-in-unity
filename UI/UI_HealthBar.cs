using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    protected Entity entity;
    protected RectTransform myTransform;
    protected Slider slider;
    protected CharacterStats myStats;
    protected virtual void Awake() {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
    }

    protected virtual void Start() {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }




    protected virtual void UpdateHealthUI() {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }


    private void FlipUI() => myTransform.Rotate(0, 180, 0);
    protected virtual void OnDisable() {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}
