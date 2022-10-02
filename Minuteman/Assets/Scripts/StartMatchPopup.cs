using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartMatchPopup : MonoBehaviour
{
    public TMP_Text enemyCount, levelCount;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(GameStartManager.Instance.StartGame);
    }

    // Update is called once per frame
    void OnEnable()
    {
        enemyCount.text = GameStartManager.NumberToText(GameStartManager.Instance.numEnemies) + "  Redcoats";
        levelCount.text = "Level " + GameStartManager.NumberToText(GameStartManager.Instance.levelNumber);
    }
}
