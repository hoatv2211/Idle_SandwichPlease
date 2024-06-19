using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    public AudioSource audioSource;
    public override void InitAwake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [Header("List audioClip UI")]
    public AudioClip[] audioClips;



    /// <summary>
    /// 0. Click - 
    /// </summary>
    /// <param name="type"></param>
    public void PlayFx(int type = 0)
    {
        audioSource.volume = Module.musicFx;
        audioSource.clip = audioClips[type];
        audioSource.Play();
    }

    public void PlayOnCamera(int type = 0)
    {
        AudioSource.PlayClipAtPoint(audioClips[type], Camera.main.transform.position, Module.musicFx);
    }

    public void PlayOnCamera(AudioClip _clip)
    {
        AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, Module.musicFx);
    }


    public bool IsFxPlaying()
    {
        return audioSource.isPlaying;
    }
}
