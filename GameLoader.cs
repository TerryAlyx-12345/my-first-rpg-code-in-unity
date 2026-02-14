using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    void Start() {
        StartCoroutine(LoadGameScene());
    }

    System.Collections.IEnumerator LoadGameScene() {
        yield return null; 

        if (!SceneManager.GetSceneByName("MainMenu").isLoaded) {
            AsyncOperation op = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
            yield return op;
        }
    }
}
