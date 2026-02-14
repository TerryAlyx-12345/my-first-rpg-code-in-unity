using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item Effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private Vector2 newVelocity;
    public override void ExecuteEffect(Transform _responPosition) {

        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;
        if (thirdAttack) {

            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _responPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = newVelocity * player.facingDir;

            Destroy(newIceAndFire, 10);
        }

    }
}
