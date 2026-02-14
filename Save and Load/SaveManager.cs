using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;


    private GameData gameData;
    [SerializeField]public HashSet<ISaveManager> registeredSaveManagers = new HashSet<ISaveManager>();
    private FileDataHandler dataHandler;
    public GameData GameData => gameData; 


    [ContextMenu("Delete save file")]
    public void DeleteSaveData() {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }

    private void Start() {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        LoadGameData();
    }
    public void RegisterAndLoadData(ISaveManager manager) {
        if (!registeredSaveManagers.Contains(manager)) {
            registeredSaveManagers.Add(manager);
            if (gameData != null) {
                manager.LoadData(gameData);
            }
        }
    }
    public void Unregister(ISaveManager manager) {
        registeredSaveManagers.Remove(manager);
    }

    public void NewGame() {
        gameData = new GameData();
    }

    public void LoadGameData() {
        Debug.Log("Loading game data...");
        gameData = dataHandler.Load();

        if (this.gameData == null) {
            Debug.Log("no saved data found");
            NewGame();
        }
    }
    public void SaveGame() {
        foreach (ISaveManager saveManager in registeredSaveManagers) {
            saveManager.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }
    private void OnApplicationQuit() {
        SaveGame();
    }

    public bool HasSavedData() {
        if(dataHandler.Load() != null) {
            return true;
        }
        return false;
    }
}
