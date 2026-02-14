using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillToptip : UI_ToolTips
{
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    public void ShowToolTip(string _skillDescription, string _skillName, int _price) {
        skillDescription.text = _skillDescription;
        skillName.text = _skillName;
        skillCost.text =  "Cost: " + _price.ToString();

        AdjustPosition();

        gameObject.SetActive(true);
    }

    public void HideToolTip() {
        gameObject.SetActive(false);
    }
}
