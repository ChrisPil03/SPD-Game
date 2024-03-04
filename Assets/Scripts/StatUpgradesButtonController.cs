using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUpgradesButtonController : MonoBehaviour
{
    private AudioSource playerAudioSource;

    [SerializeField] private TMP_Text infoText, errorText;
    private Player player;

    [SerializeField] private Button strength, stamina, health, essence, speed, magic;
    [SerializeField] private TMP_Text strengthText, staminaText, healthText, essenceText;

    [Header("Stat Upgrade costs")]
    [SerializeField] private int jarsOfSlimeNeeded = 30;
    [SerializeField] private int gemsNeeded = 8;

    [Header("Stat Upgrade Amounts")]
    [SerializeField] private int plusDamage = 2;
    [SerializeField] private int minusStaminaCost = 2;
    [SerializeField] private int plusHealthFromPotion = 2;
    [SerializeField] private int plusEssence = 1;
    [SerializeField] private int maxPlusDamage = 20;
    [SerializeField] private int maxMinusStaminaCost = 20;
    [SerializeField] private int maxPlusHealth = 20;
    [SerializeField] private int maxEssence = 5;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip upgradeStatSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip statUpgradesOpened;

    private Color32 lockedColor, unlockedColor;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();

        InitializeButton(strength);
        InitializeButton(stamina);
        InitializeButton(health);
        InitializeButton(essence);
        InitializeButton(speed);
        InitializeButton(magic);

        lockedColor = new Color32(255, 255, 255, 100);
        unlockedColor = new Color32(255, 255, 255, 255);

        speed.GetComponent<Image>().color = lockedColor;
        magic.GetComponent<Image>().color = lockedColor;

        strengthText.text = "Strength\r\n+ " + SwordAttack.extraDamage;
        staminaText.text = "Stamina\r\n+ " + StaminaController.minusStaminaCost;
        healthText.text = "Health Potion\r\n+ " + Player.extraHealth;
        essenceText.text = "Essence\r\n+ " + Player.extraXP;

        if (SwordAttack.extraDamage == maxPlusDamage)
        {
            strength.interactable = false;
        }
        if (StaminaController.minusStaminaCost == maxMinusStaminaCost)
        {
            stamina.interactable = false;
        }
        if (Player.extraHealth == maxPlusDamage)
        {
            health.interactable = false;
        }
        if (Player.extraXP == maxEssence)
        {
            essence.interactable = false;
        }

        infoText.text = "Upgrade stats and become stronger";
        errorText.text = string.Empty;

        playerAudioSource.pitch = 0.8f;
        playerAudioSource.PlayOneShot(statUpgradesOpened, 0.08f);
    }

    private void Update()
    {
        if (PauseMenuController.isPaused) return;

        if (GameObject.FindGameObjectWithTag("OldManCamp").GetComponent<OldMan>().canOpenStatUpgrades)
        {
            ButtonColorManager();
        }
    }

    private void ButtonColorManager()
    {
        if (strength.interactable == false)
        {
            strength.GetComponent<Image>().color = unlockedColor;
            strengthText.color = unlockedColor;
        }
        if (health.interactable == false)
        {
            health.GetComponent<Image>().color = unlockedColor;
            healthText.color = unlockedColor;
        }
        if (stamina.interactable == false)
        {
            stamina.GetComponent<Image>().color = unlockedColor;
            staminaText.color = unlockedColor;
        }
        if (essence.interactable == false)
        {
            essence.GetComponent<Image>().color = unlockedColor;
            essenceText.color = unlockedColor;
        }

        if (Player.jarsOfSlime >= jarsOfSlimeNeeded && Player.gems >= gemsNeeded)
        {
            if (strength.interactable == true)
            {
                strength.GetComponent<Image>().color = unlockedColor;
                strengthText.color = unlockedColor;
            }
            if (health.interactable == true)
            {
                health.GetComponent<Image>().color = unlockedColor;
                healthText.color = unlockedColor;
            }
            if (stamina.interactable == true)
            {
                stamina.GetComponent<Image>().color = unlockedColor;
                staminaText.color = unlockedColor;
            }
            if (essence.interactable == true)
            {
                essence.GetComponent<Image>().color = unlockedColor;
                essenceText.color = unlockedColor;
            }
        }
        else
        {
            if (strength.interactable == true)
            {
                strength.GetComponent<Image>().color = lockedColor;
                strengthText.color = lockedColor;
            }
            if (health.interactable == true)
            {
                health.GetComponent<Image>().color = lockedColor;
                healthText.color = lockedColor;
            }
            if (stamina.interactable == true)
            {
                stamina.GetComponent<Image>().color = lockedColor;
                staminaText.color = lockedColor;
            }
            if (essence.interactable == true)
            {
                essence.GetComponent<Image>().color = lockedColor;
                essenceText.color = lockedColor;
            }
        }
    }

    private void InitializeButton(Button button)
    {
        // Add event listeners for OnPointerEnter and OnPointerExit
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((eventData) => { OnPointerEnter(button); });
        button.GetComponent<EventTrigger>().triggers.Add(entryPointerEnter);

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((eventData) => { OnPointerExit(); });
        button.GetComponent<EventTrigger>().triggers.Add(entryPointerExit);
    }

    private void OnPointerEnter(Button button)
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
                if (SwordAttack.extraDamage + plusDamage >= maxPlusDamage)
                {
                    SwordAttack.extraDamage = maxPlusDamage;
                    DisableButton(button);
                }
                else
                {
                    SwordAttack.extraDamage += plusDamage;
                }
                strengthText.text = "Strength\r\n+ " + SwordAttack.extraDamage;
            }
            else if (button.CompareTag("Stat_Stamina"))
            {
                if (StaminaController.minusStaminaCost + minusStaminaCost >= maxMinusStaminaCost)
                {
                    StaminaController.minusStaminaCost = maxMinusStaminaCost;
                    DisableButton(button);
                }
                else
                {
                    StaminaController.minusStaminaCost += minusStaminaCost;
                }
                staminaText.text = "Stamina\r\n+ " + StaminaController.minusStaminaCost;


            }
            else if (button.CompareTag("Stat_Health"))
            {
                if (Player.extraHealth + plusHealthFromPotion >= maxPlusHealth)
                {
                    Player.extraHealth = maxPlusHealth;
                    DisableButton(button);
                }
                else
                {
                    Player.extraHealth += plusHealthFromPotion;
                }
                healthText.text = "Health Potion\r\n+ " + Player.extraHealth;
            }
            else if (button.CompareTag("Stat_Essence"))
            {
                if (Player.extraXP + plusEssence >= maxEssence)
                {
                    Player.extraXP = maxEssence;
                    DisableButton(button);
                }
                else
                {
                    Player.extraXP += plusEssence;
                }
                essenceText.text = "Essence\r\n+ " + Player.extraXP;
            }

            Player.jarsOfSlime -= jarsOfSlimeNeeded;
            Player.gems -= gemsNeeded;
            player.UpdateJarsOfSLime();
            player.UpdateGemsText();

            playerAudioSource.PlayOneShot(upgradeStatSound, 0.3f);
        }
        else
        {
            playerAudioSource.PlayOneShot(errorSound, 0.2f);
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
        else if (button.CompareTag("Stat_Essence"))
        {
            infoText.text = "+1 Essence from defeating slimes, costs:\n| Jars of slime : " + jarsOfSlimeNeeded + "            |" + "\n|           Gems : " + gemsNeeded + "              |";
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

    private void DisableButton(Button button)
    {
        button.interactable = false;
    }
}
