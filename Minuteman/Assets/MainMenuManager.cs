using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public AudioClip startSound;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(DoNewGame);  
    }

    void DoNewGame()
    {
        AudioManager.Instance.PlaySound(startSound);
        MetagameManager.Instance.NewGame();
    }
}
