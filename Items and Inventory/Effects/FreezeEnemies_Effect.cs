using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze Enemy effect", menuName = "Data/Item Effect/Freeze enemy Effect")]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField]private float duration;
    public override void ExecuteEffect(Transform _transform) {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .1f) {
            return;
        }
        if (!Inventory.instance.CanUseArmor()) {
            return;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 4);
        foreach (var hit in colliders) {
            hit.GetComponent<Enemy>()?.StartCoroutine("FreezeTimeCoroutine",2);
        }
    }
}
