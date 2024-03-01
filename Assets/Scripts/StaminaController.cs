using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [Header("Stamina Main Parameters")]
    public float playerStamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float normalAttackCost = 20.0f;
    [SerializeField] private float heavyAttackCost = 50.0f;
    [SerializeField] private float dashSwordAttackCost = 80.0f;

    [Header("Stamina Regen Parameters")]
    [Range(0, 50)][SerializeField] private float staminaRegen = 0.05f;

    [Header("Stamina UI Elements")]
    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    private SwordAttack swordAttack;

    private void Start()
    {
        swordAttack = GetComponent<SwordAttack>();

        UpdateStamina(0);
    }

    private void Update()
    {
        if (playerStamina <= maxStamina - 0.0001f)
        {
            playerStamina += staminaRegen * Time.deltaTime;
            UpdateStamina(1);

            if (playerStamina >= maxStamina)
            {
                playerStamina = maxStamina;
                UpdateStamina(0);
            }
        }
    }

    public void StaminaNormalAttack()
    {
        if (playerStamina >= (maxStamina * normalAttackCost / maxStamina))
        {
            playerStamina -= normalAttackCost;
            swordAttack.NormalAttack();
            UpdateStamina(1);
        }
    }

    public void StaminaHeavyAttack()
    {
        if (playerStamina >= (maxStamina * heavyAttackCost / maxStamina))
        {
            playerStamina -= heavyAttackCost;
            swordAttack.HeavyAttack();
            UpdateStamina(1);
        }
    }

    public void StaminaDashSwordAttack()
    {
        if (playerStamina >= (maxStamina * dashSwordAttackCost / maxStamina))
        {
            playerStamina -= dashSwordAttackCost;
            swordAttack.StartCoroutine("DashSwordAttackGrounded");
            UpdateStamina(1);
        }
    }

    private void UpdateStamina(int value)
    {
        staminaProgressUI.fillAmount = playerStamina / maxStamina;

        if (value == 0 && sliderCanvasGroup.alpha != 0)
        {
            sliderCanvasGroup.alpha = 0;
        }
        else if (value == 1 && sliderCanvasGroup.alpha != 1)
        {
            sliderCanvasGroup.alpha = 1;
        }
    }
}
