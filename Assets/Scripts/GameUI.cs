using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    private void Start()
    {
        Player.OnWin += GameWin;
        Guard.OnSpotted += GameLost;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("Level");
        }
    }

    void GameWin()
    {
        if (gameWinUI != null)
        {
            gameWinUI.SetActive(true);
        }
        gameIsOver = true;
    }

    void GameLost()
    {
        if (gameLoseUI != null)
        {
            gameLoseUI.SetActive(true);
        }
        gameIsOver = true;
    }
}
