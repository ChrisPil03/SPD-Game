using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButtonController : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText, errorText;
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void DisableButton(Button button)
    {
        ShowSkillInfo(button);

        if (!button.CompareTag("Skill_Locked") && CanAcquireSkill(button))
        {
            button.interactable = false;
            AcquireSkill(button);
        }
        else if (button.CompareTag("Skill_Locked"))
        {
            errorText.text = "-- This skill is locked --";
        }
    }

    private bool CanAcquireSkill(Button button)
    {
        if ((button.CompareTag("Skill_HeavyAttack") && !Player.hasSword) || (button.CompareTag("Skill_TwoSwords") && !Player.hasSword))
        {
            errorText.text = "-- You need a sword to learn this skill --";
            return false;
        }
        else if (Player.skillTokens >= 1)
        {
            Player.skillTokens--;
            player.UpdateSkillTokensText();
            return true;
        }
        errorText.text = "-- You need skill tokens to learn new skills --";
        return false;
    }

    private void AcquireSkill(Button button)
    {
        if (button.CompareTag("Skill_DoubleJump"))
        {
            player.canDoubleJump = true;
        }
    }

    private void ShowSkillInfo(Button button)
    {
        if (button.CompareTag("Skill_DoubleJump"))
        {
            infoText.text = "Jump once more while in the air by pressing [space] after a jump";
        }
        else if (button.CompareTag("Skill_HeavyAttack"))
        {
            infoText.text = "Heavy sword attack";
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
}
