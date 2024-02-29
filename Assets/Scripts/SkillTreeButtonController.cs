using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeButtonController : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText, errorText;
    private Player player;

    [SerializeField] private Button heavyAttack, dashSwordAttack, doubleJump, twoSwordAttack, mysterySkillOne, mysterySkillTwo;
    private Color32 lockedColor, unlockedColor;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        InitializeButton(heavyAttack, "Skill_HeavyAttack");
        InitializeButton(dashSwordAttack, "Skill_Dash");
        InitializeButton(doubleJump, "Skill_DoubleJump");
        InitializeButton(twoSwordAttack, "Skill_Locked");
        InitializeButton(mysterySkillOne, "Skill_Locked");
        InitializeButton(mysterySkillTwo, "Skill_Locked");

        lockedColor = new Color32(160, 160, 160, 100);
        unlockedColor = new Color32(255, 255, 255, 100);

        twoSwordAttack.GetComponent<Image>().color = lockedColor;
        mysterySkillOne.GetComponent<Image>().color = lockedColor;
        mysterySkillTwo.GetComponent<Image>().color = lockedColor;

        if (SwordAttack.hasDashSwordAttackSkill)
        {
            dashSwordAttack.interactable = false;
        }
        if (SwordAttack.hasHeavyAttackSkill)
        {
            heavyAttack.interactable = false;
        }
        if (Player.canDoubleJump)
        {
            doubleJump.interactable = false;
        }

        infoText.text = "Use skill tokens to learn new skills\r\nAcquire skill tokens by defeating slimes and receiving essence";
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("SkillTree").GetComponent<SkillTree>().canAccessSkillTree)
        {
            ButtonColorManager();
        }
    }

    private void ButtonColorManager()
    {
        if (!Player.hasSword)
        {
            heavyAttack.GetComponent<Image>().color = lockedColor;
            dashSwordAttack.GetComponent<Image>().color = lockedColor;
        }
        if (Player.skillTokens < 1)
        {
            if (Player.hasSword)
            {
                if (!SwordAttack.hasHeavyAttackSkill)
                {
                    heavyAttack.GetComponent<Image>().color = lockedColor;
                }
                if (!SwordAttack.hasDashSwordAttackSkill)
                {
                    dashSwordAttack.GetComponent<Image>().color = lockedColor;
                }
            }
            if (!Player.canDoubleJump)
            {
                doubleJump.GetComponent<Image>().color = lockedColor;
            }
        }
        else
        {
            if (Player.hasSword)
            {
                dashSwordAttack.GetComponent<Image>().color = unlockedColor;
                heavyAttack.GetComponent<Image>().color = unlockedColor;
            }
            doubleJump.GetComponent<Image>().color = unlockedColor;

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
        ShowSkillInfo(button);
        ShowSkillError(button);
    }

    private void OnPointerExit()
    {
        infoText.text = "Use skill tokens to learn new skills\r\nAcquire skill tokens by defeating slimes and receiving essence";
        errorText.text = "";
    }

    public void DisableButton(Button button)
    {
        ShowSkillInfo(button);

        if (!button.CompareTag("Skill_Locked") && CanAcquireSkill(button))
        {
            button.interactable = false;
            AcquireSkill(button);
        }
    }

    private bool CanAcquireSkill(Button button)
    {
        if ((button.CompareTag("Skill_HeavyAttack") && !Player.hasSword) || (button.CompareTag("Skill_TwoSwords") && !Player.hasSword) || (button.CompareTag("Skill_Dash") && !Player.hasSword))
        {
            return false;
        }
        else if (Player.skillTokens >= 1)
        {
            Player.skillTokens--;
            player.UpdateSkillTokensText();
            return true;
        }
        return false;
    }

    private void AcquireSkill(Button button)
    {
        if (button.CompareTag("Skill_DoubleJump"))
        {
            Player.canDoubleJump = true;
        }
        else if (button.CompareTag("Skill_Dash"))
        {
            SwordAttack.hasDashSwordAttackSkill = true;
        }
        else if (button.CompareTag("Skill_HeavyAttack"))
        {
            SwordAttack.hasHeavyAttackSkill = true;
        }
    }

    private void ShowSkillInfo(Button button)
    {
        if (button.CompareTag("Skill_DoubleJump"))
        {
            infoText.text = "Jump once more while in the air by pressing [ Space ] after a jump";
        }
        else if (button.CompareTag("Skill_HeavyAttack"))
        {
            infoText.text = "Do a heavy attack by pressing [ Right Control ]";
        }
        else if (button.CompareTag("Skill_Dash"))
        {
            infoText.text = "Do a dash attack by pressing [ Right Shift ]";
        }
        else if (button.CompareTag("Skill_TwoSwords"))
        {
            infoText.text = "Two swords skill";
        }
        else if (button.CompareTag("Skill_Locked"))
        {
            infoText.text = "Mystery skill";
        }
    }

    private void ShowSkillError(Button button)
    {

        if ((button.CompareTag("Skill_HeavyAttack") && !Player.hasSword) || (button.CompareTag("Skill_TwoSwords") && !Player.hasSword) || (button.CompareTag("Skill_Dash") && !Player.hasSword))
        {
            errorText.text = "-- You need a sword to learn this skill --";
        }
        else if (button.CompareTag("Skill_Locked"))
        {
            errorText.text = "-- This skill is locked --";
        }
        else if (Player.skillTokens < 1 && button.interactable)
        {
            errorText.text = "-- You don't have enough skill tokens to learn this skill --";
        }
        else
        {
            errorText.text = string.Empty;
        }
    }
}
