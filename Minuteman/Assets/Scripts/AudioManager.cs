using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource src1, src2;
    public static AudioManager Instance;
    public AudioSource musicSource;

    private bool _musicMuted;
    public bool MusicMuted {
        get
        {
            return _musicMuted;
        }
        set
        {
            _musicMuted = value;
            musicSource.volume = value ? 0f : 1f;
        }
    }
    
    public bool soundMuted;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        src1 = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        src2 = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
    }


    public void PlayWobblePitch(AudioClip[] c, float maxValue)
    {
        PlayWobblePitch(c[Random.Range(0, c.Length)], maxValue);
    }

    public void PlayWobblePitch(AudioClip c, float maxValue)
    {
        float wobble = Random.Range(0f, maxValue);
        if (Random.value > 0.5f) wobble *= -1f;
        if(src2 != null)
            src2.pitch = 1 + wobble;
        PlaySound(c, src2);
    }


    public void PlaySound(AudioClip[] c, AudioSource s = null)
    {
        PlaySound(c[Random.Range(0, c.Length)], s);
    }

    public void PlaySound(AudioClip c, AudioSource s = null)
    {
        if (soundMuted) return;
        if (s == null) s = src1;
        if(c != null)
        {
            s.PlayOneShot(c);
        }
    }
}
