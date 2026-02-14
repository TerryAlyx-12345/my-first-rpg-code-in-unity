using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder srtike effect", menuName = "Data/Item Effect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{

    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExecuteEffect(Transform _enemyPosition) {
        GameObject newThunderStrke = Instantiate(thunderStrikePrefab,_enemyPosition.position,Quaternion.identity);
        Destroy(newThunderStrke, 1f);
    }
}
