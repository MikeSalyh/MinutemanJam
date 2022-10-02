using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetagameManager : MonoBehaviour
{
    public static MetagameManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int currentLevel = 1;

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        SceneManager.LoadScene("Level" + currentLevel);
    }

    public void DoNextLevel()
    {
        ReloadLevel();
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevel);
    }

    public void QuitToMainMenu()
    {

    }
}
