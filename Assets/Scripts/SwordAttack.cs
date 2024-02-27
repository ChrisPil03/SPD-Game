using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private int normalDamage = 5, heavyDamage = 30, dashDamage = 15;
    [HideInInspector] static public bool hasHeavyAttackSkill;
    [HideInInspector] static public bool hasDashSwordAttackSkill;

    private Player player;
    private PolygonCollider2D polygonCollider;
    private AudioSource audioSource;

    private int damage;
    private bool isAttacking;
    private bool hasFlippedCollider;
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
        audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    private void Update()
    {
        Slime.damageTakenFromSword = damage;
        MediumSlimeWithOldMan.damageTakenFromSword = damage;
        SmallSlimeShell.damageTakenFromSword = damage;

        if (!Player.hasSword) return;

        if (player.rend.flipX && !hasFlippedCollider)
        {
            FlipCollider();
            hasFlippedCollider = true;
        }
        else if (!player.rend.flipX && hasFlippedCollider)
        {
            FlipCollider();
            hasFlippedCollider = false;
        }
        

        if (isAttacking)
        {
            timer += Time.deltaTime;
            if (timer > 0.26f)
            {
                timer = 0;
                isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
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

        if (Input.GetKeyDown(KeyCode.RightControl) && hasHeavyAttackSkill && canHeavyAttack)
        {
            if (player.IsGrounded())
            {
                damage = heavyDamage;
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
        }

        if (Input.GetKeyDown(KeyCode.RightShift) && hasDashSwordAttackSkill)
        {
            if (player.IsGrounded() && canDashAttack)
            {
                damage = dashDamage;
                StartCoroutine(DashSwordAttackGrounded());
            }
        }
    }

    private IEnumerator DashSwordAttackGrounded()
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
        player.rgdb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);

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

    private void FlipCollider()
    {
        // Flip the collider by inverting the x-scale
        polygonCollider.transform.localScale = new Vector3(
            -polygonCollider.transform.localScale.x,
            polygonCollider.transform.localScale.y,
            polygonCollider.transform.localScale.z
        );
    }
}
