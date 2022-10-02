using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int levelNumber;
    public static GameManager Instance;
    public bool gameOn = false, gamePaused = false;
    public GameObject levelStartBox;
    public GameObject inGameGUI;
    public TMP_Text enemyCount, levelCount;
    public int numEnemies;
    public AudioClip startGameSound, victorySound;
    public GameObject victoryPrompt, defeatPrompt, pausePrompt;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void SetPaused(bool value)
    {
        if (value)
        {
            Time.timeScale = 0f;
            gamePaused = true;
            pausePrompt.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            gamePaused = false;
            pausePrompt.SetActive(false);
        }
    }

    private void Start()
    {
        inGameGUI.SetActive(false);

        numEnemies = GameObject.FindObjectsOfType<Redcoat>().Length;
        //enemyCount.text = NumberToText(numEnemies) + "  Redcoats";
        enemyCount.text = numEnemies.ToString();
        levelCount.text = "Level " + NumberToText(levelNumber);

        levelStartBox.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameOn)
        {
            SetPaused(true);
        }
    }

    public void StartGame()
    {
        gameOn = true;
        levelStartBox.SetActive(false);
        victoryPrompt.SetActive(false);
        defeatPrompt.SetActive(false);
        inGameGUI.SetActive(true);
        AudioManager.Instance.PlaySound(startGameSound);
    }

    public void HandleEnemyDie()
    {
        numEnemies--;
        enemyCount.text = numEnemies.ToString();

        if(numEnemies <= 0)
        {
            //You win!
            StartCoroutine(VictoryCoroutine());
        }
    }

    public void HandlePlayerDie()
    {
        StartCoroutine(DefeatCoroutine());
    }

    private IEnumerator VictoryCoroutine()
    {
        yield return new WaitForSeconds(1f);
        gameOn = false;
        inGameGUI.SetActive(false);
        victoryPrompt.SetActive(true);
        PlayerCharacter.Instance.gameObject.SetActive(false);
        AudioManager.Instance.PlaySound(victorySound);
    }

    private IEnumerator DefeatCoroutine()
    {
        yield return new WaitForSeconds(2f);
        gameOn = false;
        inGameGUI.SetActive(false);
        defeatPrompt.SetActive(true);
        //AudioManager.Instance.PlaySound(victorySound);
    }

    public static string NumberToText(int num)
    {
        switch(num){
            case 0:
                return "Zero";
            case 1:
                return "One";
            case 2:
                return "Two";
            case 3:
                return "Three";
            case 4:
                return "Four";
            case 5:
                return "Five";
            case 6:
                return "Six";
            case 7:
                return "Seven";
            case 8:
                return "Eight";
            case 9:
                return "Nine";
            case 10:
                return "Ten";
            case 11:
                return "Eleven";
            case 12:
                return "Twelve";
            case 13:
                return "Thirteen";
            case 14:
                return "Fourteen";
            case 15:
                return "Fifteen";
            case 16:
                return "Sixteen";
            case 17:
                return "Seventeen";
            case 18:
                return "Eighteen";
            case 19:
                return "Nineteen";
            case 20:
                return "Twenty";
            case 21:
                return "Twenty One";
            case 22:
                return "Twenty Two";
            case 23:
                return "Twenty Three";
            case 24:
                return "Twenty Four";
            case 25:
                return "Twenty Five";
            case 26:
                return "Twenty Six";
            case 27:
                return "Twenty Seven";
            case 28:
                return "Twenty Eight";
            case 29:
                return "Twenty Nine";
            case 30:
                return "Thirty";
            default:
                return "";
        }
    }
}
