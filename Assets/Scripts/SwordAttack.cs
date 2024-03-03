using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private int normalDamage = 5, heavyDamage = 30, dashDamage = 15;
    [HideInInspector] static public bool hasHeavyAttackSkill;
    [HideInInspector] static public bool hasDashSwordAttackSkill;

    [SerializeField] private PolygonCollider2D polygonColliderAttackOne;
    [SerializeField] private PolygonCollider2D polygonColliderAttackTwo;
    [SerializeField] private PolygonCollider2D polygonColliderAttackHeavyAttack;
    [SerializeField] private PolygonCollider2D polygonColliderAttackDashAttack;

    private Player player;
    private StaminaController staminaController;
    private AudioSource audioSource;

    [HideInInspector] public bool canAttack = true;
    [HideInInspector] static public int extraDamage = 0;
    private int damage;
    private bool isAttacking;
    private float timer = 0;

    private float dashingPower = 18f;
    private float dashingTime = 0.15f;
    private float dashingCoolDown = 0.5f;
    private bool canDashAttack = true;

    [HideInInspector] public bool canHeavyAttack = true;
    private float heavyAttackCoolDown = 1f;

    [SerializeField] private AudioClip normalSwordAttack, heavySwordAttack;

    private void Start()
    {
        player = GetComponent<Player>();
        staminaController = GetComponent<StaminaController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Slime.damageTakenFromSword = damage + extraDamage;
        MediumSlimeWithOldMan.damageTakenFromSword = damage + extraDamage;
        SmallSlimeShell.damageTakenFromSword = damage + extraDamage;

        if (!Player.hasSword || PauseMenuController.isPaused) return;

        if (isAttacking)
        {
            timer += Time.deltaTime;
            if (timer > 0.26f)
            {
                timer = 0;
                isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && canAttack)
        {
            staminaController.StaminaNormalAttack();
        }

        if (Input.GetKeyDown(KeyCode.RightControl) && hasHeavyAttackSkill && canHeavyAttack && canAttack)
        {
            if (player.IsGrounded())
            {
                damage = heavyDamage;
                staminaController.StaminaHeavyAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightShift) && hasDashSwordAttackSkill && canAttack)
        {
            if (player.IsGrounded() && canDashAttack)
            {
                damage = dashDamage;
                staminaController.StaminaDashSwordAttack();
            }
        }
    }

    public void NormalAttack()
    {
        damage = normalDamage;
        if (!isAttacking)
        {
            isAttacking = true;
            player.anim.Play("SwordAttackOne");

        }
        else if (isAttacking && timer > 0.1f)
        {
            player.anim.Play("SwordAttackTwo");

        }

        audioSource.pitch = Random.Range(0.9f, 1.2f);
        audioSource.PlayOneShot(normalSwordAttack, 0.05f);
    }

    public void HeavyAttack()
    {
        player.canMove = false;
        player.rgdb.velocity = Vector2.zero;
        Invoke("PlayerCanMoveAgain", 0.6f);

        canHeavyAttack = false;
        player.anim.Play("HeavySwordAttackGrounded");

        audioSource.pitch = Random.Range(1f, 1.2f);
        audioSource.PlayOneShot(normalSwordAttack, 0.05f);
        audioSource.PlayOneShot(heavySwordAttack, 0.1f);

        Invoke("CanHeavyAttackAgain", heavyAttackCoolDown);
    }

    public IEnumerator DashSwordAttackGrounded()
    {
        player.canMove = false;
        Invoke("PlayerCanMoveAgain", 0.717f);

        canDashAttack = false;
        player.canDash = false;
        player.isDashing = true;
        player.rgdb.gravityScale = 0f;
        player.anim.Play("DashSwordAttackGrounded");

        yield return new WaitForSeconds(0.06f);

        player.GetComponent<CapsuleCollider2D>().enabled = false;
        if (!player.rend.flipX)
        {
            player.rgdb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        }
        else
        {
            player.rgdb.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
        }

        audioSource.pitch = Random.Range(0.8f, 1f);
        audioSource.PlayOneShot(normalSwordAttack, 0.15f);
        audioSource.PlayOneShot(heavySwordAttack, 0.02f);

        yield return new WaitForSeconds(dashingTime);

        player.GetComponent<CapsuleCollider2D>().enabled = true;
        player.rgdb.velocity = Vector2.zero;
        player.ResetGravity();
        player.isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);

        canDashAttack = true;
        player.canDash = true;
    }

    private void PlayerCanMoveAgain()
    {
        if (!player.canMove)
        player.canMove = true;
    }

    private void CanHeavyAttackAgain()
    {
        canHeavyAttack = true;
    }
}
