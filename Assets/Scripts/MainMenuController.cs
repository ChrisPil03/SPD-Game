using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueText;

    private bool isInCredits;
    static private bool hasStartedNewGame;

    private void Start()
    {
        if (hasStartedNewGame)
        {
            continueButton.interactable = true;
            continueText.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            continueButton.interactable = false;
            continueText.color = new Color32(255, 255, 255, 100);
        }

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

    public void ContinueGame()
    {
        SceneManager.LoadScene(Player.checkpointScenePlayerHasReached);
    }

    public void StartNewGame()
    {
        hasStartedNewGame = true;
        ResetStaticVariables();
        SceneManager.LoadScene(1);
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

    private void ResetStaticVariables()
    {
        Player.hasAlechemySkill = false;
        Player.hasSword = false;
        Player.changeSceneOnRespawn = false;
        Player.checkpointScenePlayerHasReached = 1;
        Player.skillTokens = 0;
        Player.jarsOfSlime = 0;
        Player.gems = 0;
        Player.extraXP = 0;
        Player.currentXP = 0;
        Player.extraHealth = 0;
        Player.canDoubleJump = false;

        StaminaController.minusStaminaCost = 0;

        SwordAttack.hasDashSwordAttackSkill = false;
        SwordAttack.hasHeavyAttackSkill = false;
        SwordAttack.extraDamage = 0;

        OldMan.firstInteraction = true;
    }
}
