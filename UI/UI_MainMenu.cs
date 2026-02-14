using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField]UI_FadeScreen fadeScreen;
    private void Start() {
        if(SaveManager.instance.HasSavedData() == false) {
            continueButton.SetActive(false);
        }
    }
    public void ContinueGame() {
        StartCoroutine(LoadScreenWithFadeEffect(1.5f));
    }
    public void NewGame() {
        SaveManager.instance.DeleteSaveData();
        SaveManager.instance.NewGame();
        StartCoroutine(LoadScreenWithFadeEffect(1.5f));
    }
    public void ExitGame() {
        Debug.Log("Exit game");
    }
    IEnumerator LoadScreenWithFadeEffect(float _delay) {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.UnloadSceneAsync("MainMenu");
        // 改为附加加载，保留 ManagerScene
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
