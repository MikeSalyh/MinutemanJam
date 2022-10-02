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
        startButton.onClick.AddListener(GameManager.Instance.StartGame);
    }

    // Update is called once per frame
    void OnEnable()
    {
        enemyCount.text = GameManager.NumberToText(GameManager.Instance.numEnemies) + "  Redcoat" + (GameManager.Instance.numEnemies == 1 ? "" : "s");
        levelCount.text = "Level " + GameManager.NumberToText(GameManager.Instance.levelNumber);
    }
}
