using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinumumDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    public bool playBGM;
    private int bgmIndex;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }
    private void Update() {
        if (!playBGM) {
            StopAllBGM();
        }
        else {
            if(!bgm[bgmIndex].isPlaying) {
                PlayBGM(bgmIndex);
            }
        }
    }
    public void PlaySFX(int _sfxIndex,Transform _source) {
        if(PlayerManager.instance.player == null) {
            return;
        }
        if(_source != null && 
            Vector2.Distance(PlayerManager.instance.player.transform.position,_source.position) > sfxMinumumDistance) {
            return;
        }
        if(_sfxIndex < sfx.Length) {
            sfx[_sfxIndex].pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            if (!sfx[_sfxIndex].isPlaying) {
                sfx[_sfxIndex].Play();
            }
        }
    }
    public void StopSFX(int _sfxIndex) {
        sfx[_sfxIndex].Stop();
    }
    public void PlayerRandomBGM() {
        bgmIndex = UnityEngine.Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int _bgmIndex) {
        bgmIndex = _bgmIndex;
        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    private void StopAllBGM() {
        for (int i = 0; i < bgm.Length; i++) {
            bgm[i].Stop();
        }
    }

}
