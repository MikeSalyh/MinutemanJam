using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button QuitButton, ResumeButton, MuteMusicButton, MuteSoundButton;
    public Image soundSprite, musicSprite;
    public Sprite musicOn, musicOff, soundOn, soundOff;

    // Start is called before the first frame update
    void Start()
    {
        ResumeButton.onClick.AddListener(Resume);
        QuitButton.onClick.AddListener(MetagameManager.Instance.QuitToMainMenu);
        MuteMusicButton.onClick.AddListener(ToggleMusic);
        MuteSoundButton.onClick.AddListener(ToggleSound);
    }

    private void ToggleMusic()
    {
        AudioManager.Instance.MusicMuted = !AudioManager.Instance.MusicMuted;
        RefreshAudioSprites();
    }

    private void ToggleSound()
    {
        AudioManager.Instance.soundMuted = !AudioManager.Instance.soundMuted;
        RefreshAudioSprites();
    }

    private void OnEnable()
    {
        RefreshAudioSprites();
    }

    private void RefreshAudioSprites()
    {
        soundSprite.sprite = AudioManager.Instance.soundMuted ? soundOff : soundOn;
        musicSprite.sprite = AudioManager.Instance.MusicMuted ? musicOff : musicOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }
    }

    private void Resume()
    {
        GameManager.Instance.SetPaused(false);
    }
}
