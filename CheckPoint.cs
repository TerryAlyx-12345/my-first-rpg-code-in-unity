using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Animator anim;
    public string checkPointID;
    public bool activationStatus;
    private void Awake() {
    }
    private void Start() {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpointID")]
    private void GenerateID() {
        checkPointID = System.Guid.NewGuid().ToString();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>() != null) {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint() {
        activationStatus = true;
        if (anim == null) {
            StartCoroutine(DelayedActivateCheckpoint());
        }
        else {
            anim.SetBool("Active", true);
        }

    }
    private IEnumerator DelayedActivateCheckpoint() {
        yield return null;  // 下一帧 Player 肯定初始化完毕了
        anim.SetBool("Active", true);     // 递归重试
    }
}
