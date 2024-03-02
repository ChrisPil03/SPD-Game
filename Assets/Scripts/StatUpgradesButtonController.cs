using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUpgradesButtonController : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText, errorText;
    private Player player;

    [SerializeField] private Button strength, stamina, health, loot, speed, magic;
    [SerializeField] private TMP_Text strengthText, staminaText, healthText;

    [Header("Stat Upgrade costs")]
    [SerializeField] private int jarsOfSlimeNeeded = 30;
    [SerializeField] private int gemsNeeded = 8;

    [Header("Stat Upgrade Amounts")]
    [SerializeField] private int plusDamage = 2;
    [SerializeField] private int minusStaminaCost = 2;
    [SerializeField] private int plusHealthFromPotion = 2;

    private Color32 lockedColor, unlockedColor;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        InitializeButton(strength, "Stat_Strength");
        InitializeButton(stamina, "Stat_Stamina");
        InitializeButton(health, "Stat_Health");
        InitializeButton(loot, "Skill_Locked");
        InitializeButton(speed, "Skill_Locked");
        InitializeButton(magic, "Skill_Locked");

        lockedColor = new Color32(255, 255, 255, 100);
        unlockedColor = new Color32(255, 255, 255, 255);

        loot.GetComponent<Image>().color = lockedColor;
        speed.GetComponent<Image>().color = lockedColor;
        magic.GetComponent<Image>().color = lockedColor;

        strengthText.text = "Strength\r\n+ " + SwordAttack.extraDamage;
        staminaText.text = "Stamina\r\n+ " + StaminaController.minusStaminaCost;
        healthText.text = "Health Potion\r\n+ " + Player.extraHealth;

        infoText.text = "Upgrade stats and become stronger";
        errorText.text = string.Empty;
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("OldManCamp").GetComponent<OldMan>().canOpenStatUpgrades)
        {
            ButtonColorManager();
        }
    }

    private void ButtonColorManager()
    {
        if (Player.jarsOfSlime >= jarsOfSlimeNeeded && Player.gems >= gemsNeeded)
        {
            strength.GetComponent<Image>().color = unlockedColor;
            stamina.GetComponent<Image>().color = unlockedColor;
            health.GetComponent<Image>().color = unlockedColor;

            strengthText.color = unlockedColor;
            staminaText.color = unlockedColor;
            healthText.color = unlockedColor;
        }
        else
        {
            strength.GetComponent<Image>().color = lockedColor;
            stamina.GetComponent<Image>().color = lockedColor;
            health.GetComponent<Image>().color = lockedColor;

            strengthText.color = lockedColor;
            staminaText.color = lockedColor;
            healthText.color = lockedColor;
        }
    }

    private void InitializeButton(Button button, string tag)
    {
        // Add event listeners for OnPointerEnter and OnPointerExit
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((eventData) => { OnPointerEnter(button, tag); });
        button.GetComponent<EventTrigger>().triggers.Add(entryPointerEnter);

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((eventData) => { OnPointerExit(); });
        button.GetComponent<EventTrigger>().triggers.Add(entryPointerExit);
    }

    private void OnPointerEnter(Button button, string tag)
    {
        ShowStatInfo(button);
        ShowStatError(button);
    }

    private void OnPointerExit()
    {
        infoText.text = "Upgrade stats and become stronger";
        errorText.text = "";
    }

    public void UpgradeStat(Button button)
    {
        if (CanUpgradeStat(button))
        {
            if (button.CompareTag("Stat_Strength"))
            {
                SwordAttack.extraDamage += plusDamage;
                strengthText.text = "Strength\r\n+ " + SwordAttack.extraDamage;
            }
            else if (button.CompareTag("Stat_Stamina"))
            {
                StaminaController.minusStaminaCost += minusStaminaCost;
                staminaText.text = "Stamina\r\n+ " + StaminaController.minusStaminaCost;
            }
            else if (button.CompareTag("Stat_Health"))
            {
                Player.extraHealth += plusHealthFromPotion;
                healthText.text = "Health Potion\r\n+ " + Player.extraHealth;
            }

            Player.jarsOfSlime -= jarsOfSlimeNeeded;
            Player.gems -= gemsNeeded;
            player.UpdateJarsOfSLime();
            player.UpdateGemsText();
        }
    }

    private bool CanUpgradeStat(Button button)
    {
        if (Player.jarsOfSlime >= jarsOfSlimeNeeded && Player.gems >= gemsNeeded && !button.CompareTag("Skill_Locked"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ShowStatInfo(Button button)
    {
        if (button.CompareTag("Stat_Strength"))
        {
            infoText.text = "+2 Strength costs:\n| Jars of slime : " + jarsOfSlimeNeeded + "            |" + "\n|           Gems : " + gemsNeeded + "              |";
        }
        else if (button.CompareTag("Stat_Stamina"))
        {
            infoText.text = "+2 Stamina costs:\n| Jars of slime : " + jarsOfSlimeNeeded + "            |" + "\n|           Gems : " + gemsNeeded + "              |";
        }
        else if (button.CompareTag("Stat_Health"))
        {
            infoText.text = "+2 Health potion extra health costs:\n| Jars of slime : " + jarsOfSlimeNeeded + "            |" + "\n|           Gems : " + gemsNeeded + "              |";
        }
        else if (button.CompareTag("Skill_Locked"))
        {
            infoText.text = "Not avaible";
        }
    }

    private void ShowStatError(Button button)
    {
        if (button.CompareTag("Skill_Locked"))
        {
            errorText.text = "-- Reach the camp outside the kingdom's walls to unlock this power upgrade --";
        }
        else if (Player.jarsOfSlime < jarsOfSlimeNeeded || Player.gems < gemsNeeded)
        {
            errorText.text = "-- You don't have enough jars of slime or gems --";
        }
        else
        {
            errorText.text = string.Empty;
        }
    }
}
