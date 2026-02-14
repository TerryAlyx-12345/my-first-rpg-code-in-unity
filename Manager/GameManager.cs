using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }
    private void Start() {

    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            RestartScene();
        }//test
    }

    public void RestartScene() {
        SaveManager.instance.SaveGame();
        SceneManager.UnloadSceneAsync("MainScene");
        StartCoroutine(ReloadGameScene());
    }

    IEnumerator ReloadGameScene() {
        while (SceneManager.GetSceneByName("MainScene").isLoaded)
            yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        yield return op;
    }
}
