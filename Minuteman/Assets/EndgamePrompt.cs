using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgamePrompt : MonoBehaviour
{
    public Button continueButton;
    public bool isVictory;
    // Start is called before the first frame update
    void Start()
    {
        if (isVictory)
        {
            continueButton.onClick.AddListener(MetagameManager.Instance.DoNextLevel);
        }
        else
        {
            continueButton.onClick.AddListener(MetagameManager.Instance.ReloadLevel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
