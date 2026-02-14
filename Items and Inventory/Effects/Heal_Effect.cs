using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item Effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPersent; 

    public override void ExecuteEffect(Transform _enemyPosition) {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPersent);

        playerStats.IncreaseHealthBy(healAmount);
    }
}
