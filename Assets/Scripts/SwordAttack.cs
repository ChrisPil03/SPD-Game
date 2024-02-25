using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private Player player;
    private PolygonCollider2D polygonCollider;

    private bool isAttacking;
    private bool hasFlippedCollider;
    private float timer = 0;

    private float dashingPower = 18f;
    private float dashingTime = 0.15f;
    private float dashingCoolDown = 0.5f;
    private bool canDashAttack = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();

        Slime.damageTakenFromSword = damage;
        MediumSlimeWithOldMan.damageTakenFromSword = damage;
    }

    private void Update()
    {
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
            if (!isAttacking)
            {
                isAttacking = true;
                player.anim.Play("SwordAttackOne");
            }
            else if (isAttacking && timer > 0.1f)
            {
                player.anim.Play("SwordAttackTwo");

            }
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            if (player.IsGrounded())
            {
                player.anim.Play("HeavySwordAttackGrounded");
            }
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (player.IsGrounded() && canDashAttack)
            {
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
