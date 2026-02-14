using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;

    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    public bool unlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;
    private void OnValidate() {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
        SaveManager.instance.RegisterAndLoadData(this);
        SkillManager.instance?.RefreshAllPassiveEffects();
    }
    private void Start() {
        ui = GetComponentInParent<UI>();
        skillImage = GetComponent<Image>();
        skillImage.color = lockedSkillColor;
        if (unlocked) {
            skillImage.color = Color.white;
        }

    }

    private void UnlockSkillSlot() {
        if (PlayerManager.instance.HaveEnoughMoney(skillPrice) == false) {
            return; 
        }
        for (int i = 0; i < shouldBeUnlocked.Length; i++) {
            if (shouldBeUnlocked[i].unlocked == false) {
                Debug.Log("can not unlock skill");// to be modified
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++) {
            if (shouldBeLocked[i].unlocked == true) {
                Debug.Log("can not unlock skill");
                return;
            }
        }
        unlocked = true;
        skillImage.color = Color.white;
        SkillManager.instance.RefreshAllPassiveEffects();
    }
    public void OnPointerEnter(PointerEventData eventData) {
        ui.ui_SkillToptip.ShowToolTip(skillDescription, skillName, skillPrice);

    }

    public void OnPointerExit(PointerEventData eventData) {
        ui.ui_SkillToptip.HideToolTip();
    }

    public void LoadData(GameData _data) {
        if (_data.skillTree.TryGetValue(skillName, out bool value)) {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data) {
        if (_data.skillTree.TryGetValue(skillName,out bool value)) {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else {
            _data.skillTree.Add(skillName,unlocked);
        }
    }
    private void OnDestroy() {
        if (SaveManager.instance != null)
            SaveManager.instance.Unregister(this);
    }
}
