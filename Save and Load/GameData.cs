using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentID;

    public SerializableDictionary<string, bool> checkpointsStorage;
    public string closestCheckpointID;
    public GameData() {
        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentID = new List<string>();
        checkpointsStorage = new SerializableDictionary<string, bool>();
        closestCheckpointID = string.Empty;
    }
}
