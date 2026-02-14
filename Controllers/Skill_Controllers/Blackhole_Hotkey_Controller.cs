using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;


    private Transform myEnemies
        ;
    private Blackhole_Skill_Controller blackHole;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackhole) {

        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myEnemies = _myEnemy;
        blackHole = _myBlackhole;


        myHotKey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();

        Debug.Log($"SetupHotKey called - Key: {myHotKey}, SR: {sr}, Text: {myText}, Text Color: {myText.color}");
    }

    private void Update() {
        if (Input.GetKeyDown(myHotKey)) {
            blackHole.AddEnemyToList(myEnemies);
            //Debug.Log("Hotkey Pressed: " + myHotKey);
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
