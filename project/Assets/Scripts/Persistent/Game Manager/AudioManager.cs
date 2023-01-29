using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    //播放 音频
    public void Audio_Play(AudioClip clip, AudioSource source = null, bool loop = false)
    {
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }
    //播放 随机音频
    public void Audio_RandomPlay(AudioClip[] clips, AudioSource source = null, bool loop = false)
    {
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.clip = clips[Random.Range(0,clips.Length)];
        source.loop = loop;
        source.Play();
    }

}
