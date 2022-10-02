using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSounds : MonoBehaviour
{
    public AudioClip corkSound, sandSound, rustleSound, plungerSound, finishSound;


    public void PlayUncork()
    {
        AudioManager.Instance.PlaySound(corkSound);
    }

    public void PlaySand()
    {
        AudioManager.Instance.PlaySound(sandSound);
    }

    public void PlayRustle()
    {
        AudioManager.Instance.PlaySound(rustleSound);
    }

    public void PlayPlunger()
    {
        AudioManager.Instance.PlaySound(plungerSound);
    }

    public void PlayFinish()
    {
        AudioManager.Instance.PlaySound(finishSound);
    }
}
