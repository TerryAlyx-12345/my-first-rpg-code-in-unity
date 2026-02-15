using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour, ISaveManager 
{
    [SerializeField] public List<CheckPoint> checkPoints;
    [SerializeField] private string closestCheckpointID;
    private void Awake() {
        checkPoints = new List<CheckPoint>(GetComponentsInChildren<CheckPoint>());
        SaveManager.instance.RegisterAndLoadData(this);
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
        Invoke("PlacePlayerAtClosestCheckpoint", .1f);
    }

    public void SaveData(ref GameData _data) {
        CheckPoint closest = FindClosestCheckpoint();
        _data.closestCheckpointID = closest != null ? closest.checkPointID : string.Empty;
        _data.checkpointsStorage.Clear();
        foreach (CheckPoint checkpoint in this.checkPoints) {
            _data.checkpointsStorage.Add(checkpoint.checkPointID, checkpoint.activationStatus);
        }
    }

    private CheckPoint FindClosestCheckpoint() {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;
        foreach (CheckPoint checkPoint in this.checkPoints) {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkPoint.transform.position);
            if (distanceToCheckpoint < closestDistance && checkPoint.activationStatus == true) {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkPoint;
            }
        }
        return closestCheckpoint;
    }
    private void PlacePlayerAtClosestCheckpoint() {
        if (string.IsNullOrEmpty(closestCheckpointID)) return;
        foreach (CheckPoint checkPoint in checkPoints) {
            if (closestCheckpointID == checkPoint.checkPointID) {
                PlayerManager.instance.player.transform.position = checkPoint.transform.position;
                checkPoint.ActivateCheckpoint();
            }
        }
    }

    private void OnDestroy() {
        if (SaveManager.instance != null) {
            SaveManager.instance.Unregister(this);
        }
    }
}
