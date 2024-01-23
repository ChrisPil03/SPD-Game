using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;

    //Groundcheck
    [SerializeField] private Transform ray;
    [SerializeField] private LayerMask whatIsGround;
    private float rayDistance = 0.1f;

    [Header("Jump")]
    [SerializeField] private float vForce = 5f;
    [SerializeField] private float hForce = 2f;
    [SerializeField] private float fallSpeedMultiplier = 1.01f;
    [SerializeField] private float maxFallSpeed = 4;
    private bool canJump = true;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        if (!canJump) return;

        if (IsGrounded())
        {
            StartCoroutine(Jump());
        }

        FallSpeedRegulator();
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

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void FallSpeedRegulator()
    {
        // Higher fall gravity with a cap on maximum fallspeed
        float originalGravity = rgdb.gravityScale;
        if (rgdb.velocity.y < 0 && rgdb.velocity.y >= -maxFallSpeed)
        {
            rgdb.gravityScale *= fallSpeedMultiplier;
        }
        else
        {
            rgdb.gravityScale = originalGravity;
        }
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
    }
}
