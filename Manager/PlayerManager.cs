using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    public float lostCurrencyRate = .2f;
    private void Awake() {
        if (instance != null) {
            Destroy(instance.gameObject);
        }
        else {
            instance = this;
        }
    }
    private void Start() {
        SaveManager.instance.RegisterAndLoadData(this);
    }
    public bool HaveEnoughMoney(int _price) {
        if(_price > currency) {
            Debug.Log("No enough mouney");
            return false;
        }

        currency = currency - _price;
        return true;
    }
    public int GetCurrencyAmount() {
        return currency;
    }

    public void LoadData(GameData _data) {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data) {
        _data.currency = this.currency;
    }
}
