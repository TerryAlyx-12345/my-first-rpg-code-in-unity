using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField]private GameObject restartButton;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;
    
    public UI_ItemTooltip ui_ItemTooltip;
    public UI_StatToolTip ui_StatTooltip;
    public UI_CraftWindow ui_CraftWindow;
    public UI_SkillToptip ui_SkillToptip;

    private void Awake() {
        SwitchTo(skillTreeUI);// we make this to assign events on skill tree ststa before we assign events on skill scripts
        fadeScreen.gameObject.SetActive(true);
    }
    void Start() {
        SwitchTo(null);
        SwitchTo(inGameUI);
        ui_ItemTooltip.gameObject.SetActive(false);
        ui_StatTooltip.gameObject.SetActive(false);
        ui_SkillToptip.gameObject.SetActive(false);//to be modified : 刚进去的craftUI是好的，但是切换其他页面之后再去就不行
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.C)) {
            SwitchWithKeyTo(characterUI);
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            SwitchWithKeyTo(skillTreeUI);
        }
        if (Input.GetKeyUp(KeyCode.B)) {
            SwitchWithKeyTo(craftUI);
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            SwitchWithKeyTo(optionsUI);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            bool anyUIOpen = characterUI.activeSelf || skillTreeUI.activeSelf || craftUI.activeSelf || optionsUI.activeSelf;
            if (anyUIOpen) {
                SwitchWithKeyTo(inGameUI);
            }
            else {
                SwitchWithKeyTo(optionsUI);
            }
        }
    }
    public void SwitchTo(GameObject _menu) {
        for (int i = 0; i < transform.childCount; i++) {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;//keep fade screen active 
            if (fadeScreen == false) {
                transform.GetChild(i).gameObject.SetActive(false);

            }
        }

        if (_menu != null) {
            _menu.SetActive(true);
        }

    }

    public void SwitchWithKeyTo(GameObject _menu) {
        if (_menu != null && _menu.activeSelf) {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }
        SwitchTo(_menu);
    }

    private void CheckForInGameUI() {
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).gameObject.activeSelf  && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null) {
                return;
            }
        }
        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen() {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorountine());

    }

    IEnumerator EndScreenCorountine() {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton()=> GameManager.instance.RestartScene();
}