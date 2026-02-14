using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager {
    public static GameManager instance;
    [SerializeField] public List<CheckPoint> checkPoints = new List<CheckPoint>();
    [SerializeField] private string closestCheckpointID;
    public CheckPoint[] check;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
        SaveManager.instance.RegisterAndLoadData(this);
    }
    private void Start() {
    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            RestartScene();
        }//test
    }

    public void RestartScene() {
        Debug.Log("check point" + this.checkPoints.Count);
        SaveManager.instance.SaveGame();
        SceneManager.UnloadSceneAsync("MainScene");
        StartCoroutine(ReloadGameScene());
    }

    IEnumerator ReloadGameScene() {
        while (SceneManager.GetSceneByName("MainScene").isLoaded)
            yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        yield return op;
        SaveManager.instance.LoadGameData();
        SaveManager.instance.RegisterAndLoadData(this);
        Debug.Log("Scene reloaded,load game");
    }

    public void LoadData(GameData _data) {
        foreach (KeyValuePair<string, bool> pair in _data.checkpointsStorage) {
            foreach (CheckPoint checkPoint in checkPoints) {
                if (checkPoint.checkPointID == pair.Key && pair.Value == true) {
                    checkPoint.ActivateCheckpoint();
                }
            }
        }
        closestCheckpointID = _data.closestCheckpointID;
        Invoke("PlacePlayerAtClosestCheckpoint",.1f);
    }

    private void PlacePlayerAtClosestCheckpoint() {
        Debug.Log("Placing player at closest checkpoint: " + closestCheckpointID);
        foreach (CheckPoint checkPoint in checkPoints) {
            if (closestCheckpointID == checkPoint.checkPointID) {
                PlayerManager.instance.player.transform.position = checkPoint.transform.position;
            }
        }
    }
    public void SaveData(ref GameData _data) {
        Debug.Log("check point" + this.checkPoints.Count);
        if(this.checkPoints.Count == 0) {
            check = FindObjectsOfType<CheckPoint>();
            foreach(CheckPoint item in check) {
                this.checkPoints.Add(item);
            }
        }
        Debug.Log("check point" + this.checkPoints.Count);//这里还是两个

        _data.closestCheckpointID = FindClosestCheckpoint().checkPointID;//这里就没有了
        _data.checkpointsStorage.Clear();
        Debug.Log("Saving closest checkpoint: " + _data.closestCheckpointID);
        foreach (CheckPoint checkpoint in this.checkPoints) {
            _data.checkpointsStorage.Add(checkpoint.checkPointID, checkpoint.activationStatus);
        }
    }
    private CheckPoint FindClosestCheckpoint() {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;
        this.checkPoints.Clear();
        check = FindObjectsOfType<CheckPoint>();

        foreach (CheckPoint item in check) {
            this.checkPoints.Add(item);
        }
        Debug.Log("check point" + this.checkPoints.Count);// 2 checkpoint
        foreach (CheckPoint checkPoint in this.checkPoints) {
            if(checkPoint == null) {
                Debug.Log("no checkpoint");
                continue;
            }
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkPoint.transform.position);
            if (distanceToCheckpoint < closestDistance && checkPoint.activationStatus == true) {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkPoint;
            }
        }
        return closestCheckpoint;
    }
}
