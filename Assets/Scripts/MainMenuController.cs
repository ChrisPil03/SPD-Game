using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject credits;

    private bool isInCredits;

    private void Start()
    {
        credits.SetActive(false);
    }

    private void Update()
    {
        if (isInCredits && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCredits();
        }

        if(!isInCredits && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(Player.checkpointScenePlayerHasReached);
    }

    public void ShowCredits()
    {
        credits.SetActive(true);
        isInCredits = true;
    }

    public void CloseCredits()
    {
        credits.SetActive(false);
        isInCredits = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
