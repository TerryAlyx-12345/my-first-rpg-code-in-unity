using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LostCurrency : MonoBehaviour,ISaveManager
{
    public static LostCurrency instance;
    public int currencyContain;
    public GameObject lostCurrencyPrefab;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
        SaveManager.instance.RegisterAndLoadData(this);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.GetComponent<Player>() != null) {
            PlayerManager.instance.currency += currencyContain;
            currencyContain = 0;
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
    }
    public void LoadData(GameData _data) {
        currencyContain = _data.lostCurrencyAmount;
        if (currencyContain > 0) {
            this.transform.position = new Vector3(_data.lostCurrencyX, _data.lostCurrencyY);
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, this.transform);
            newLostCurrency.transform.localPosition = Vector3.zero;

        }
    }

    public void SaveData(ref GameData _data) {
        PlayerManager playerManager = PlayerManager.instance;
        _data.lostCurrencyAmount = currencyContain;
        _data.lostCurrencyX = playerManager.player.transform.position.x;
        _data.lostCurrencyY = playerManager.player.transform.position.y;
    }
}
