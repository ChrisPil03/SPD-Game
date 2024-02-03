using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;

    [SerializeField] private int giveXp = 10;
    [SerializeField] private Player player;

    //Groundcheck
    [SerializeField] private Transform ray;
    [SerializeField] private LayerMask whatIsGround;
    private float rayDistance = 0.1f;

    [Header("Health")]
    [SerializeField] private int startingHealth = 20;
    private int currentHealth;
    private bool isDead = false;

    [Header("Jump")]
    [SerializeField] private float vForce = 5f;
    [SerializeField] private float hForce = 2f;
    private bool canJump = true;

    [Header("Combat")]
    [SerializeField] private float giveBounceForce = 7f;
    [SerializeField] private float giveVKnockback = 3f;
    [SerializeField] private float giveHKnockback = 3f;
    [SerializeField] private int damageGiven = 4;
    [SerializeField] private int damageTaken = 10;

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private CapsuleCollider2D capsuleCollider;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentHealth = startingHealth;
    }

    void Update()
    {
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        if (!canJump) return;

        if (IsGrounded() && !isDead)
        {
            StartCoroutine(Jump());
        }
    }

    private void FlipSprite(bool direction)
    {
        rend.flipX = direction;
    }

    private IEnumerator Jump()
    {
        canJump = false;
        anim.Play("SmallSlime_Jump");
        yield return new WaitForSeconds(0.3f);
        rgdb.velocity = new Vector2(hForce, vForce);
        yield return new WaitForSeconds(Random.Range(1f, 8f));
        canJump = true;
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.position, Vector2.down, rayDistance, whatIsGround);

        if (hit.collider != null && hit.collider.CompareTag("Ground") && !isDead)
        {
            boxCollider.enabled = true;
            return true;
        }

        boxCollider.enabled = false;
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBlock"))
        {
            hForce = -hForce;

            if (hForce < 0)
            {
                FlipSprite(true);
            }

            if (hForce > 0)
            {
                FlipSprite(false);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(damageGiven);

            if (collision.transform.position.x > transform.position.x)
            {
                player.TakeKnockback(giveHKnockback, giveVKnockback);
            }
            else
            {
                player.TakeKnockback(-giveHKnockback, giveVKnockback);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.hasSword)
            {
                collision.GetComponent<Animator>().Play("FallReversedWithSword");
            }
            else
            {
                collision.GetComponent<Animator>().Play("FallReversedNoSword");
            }
            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.GetComponent<Rigidbody2D>().velocity.x, giveBounceForce);
            TakeDamage(damageTaken);
        }
    }

    private void Die()
    {
        rgdb.velocity = Vector2.zero;
        rgdb.gravityScale = 0;
        canJump = false;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        anim.Play("SmallSlime_Defeated");
        player.GainXP(giveXp);
        Destroy(gameObject, 1f);
    }

    private void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }
}
