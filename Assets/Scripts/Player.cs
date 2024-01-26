using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask whatIsGround;

    private float originalGravity;
    private float rayDistance = 0.1f;

    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;

    [Header("Health")]
    [SerializeField] private int startingHealth = 100;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 150f;
    private float horizontalValue;
    private bool canMove = true;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9.5f;
    private bool canDoubleJump;

    [Header("Dash")]
    [SerializeField] private float dashingPower = 12.5f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCoolDown = 0.5f;
    private bool canDash = true;
    private bool isDashing;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        originalGravity = rgdb.gravityScale;
        currentHealth = startingHealth;
    }

    void Update()
    {
        if (isDashing || !canMove) return;

        horizontalValue = Input.GetAxis("Horizontal");

        if (horizontalValue < 0)
        {
            FlipSprite(true);
        }

        if (horizontalValue > 0)
        {
            FlipSprite(false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump") || rgdb.velocity.y < 0)
        {
            ResetGravity();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        anim.SetFloat("MoveSpeed", Mathf.Abs(rgdb.velocity.x));
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetBool("TakingKnockback", false);
    }

    private void FixedUpdate()
    {
        if (isDashing || !canMove) return;

        rgdb.velocity = new Vector2(horizontalValue * moveSpeed * Time.deltaTime, rgdb.velocity.y);
    }

    private void FlipSprite(bool direction)
    {
        rend.flipX = direction;
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rgdb.gravityScale = originalGravity * 0.5f;
            rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }
    }

    private void DoubleJump()
    {
        ResetGravity();
        PlayDoubleJumpAnim();
        rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce * 1.4f);
        canDoubleJump = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rgdb.gravityScale = 0f;
        if (rend.flipX)
        {
            rgdb.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
        }
        else
        {
            rgdb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        }
        yield return new WaitForSeconds(dashingTime);
        ResetGravity();
        isDashing = false;
        if (IsGrounded())
        {
            yield return new WaitForSeconds(dashingCoolDown);
        }
        else
        {
            yield return new WaitUntil(IsGrounded);
        }
        canDash = true;
    }

    private void ResetGravity()
    {
        rgdb.gravityScale = originalGravity;
    }

    private bool IsGrounded()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(leftFoot.position, Vector2.down, rayDistance, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast(rightFoot.position, Vector2.down, rayDistance, whatIsGround);

        if (leftHit.collider != null && leftHit.collider.CompareTag("Ground") || rightHit.collider != null && rightHit.collider.CompareTag("Ground"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CanMove()
    {
        canMove = true;
    }

    private void PlayDoubleJumpAnim()
    {
        if (rgdb.velocity.y < -2)
        {
            anim.Play("FallReversedNoSword");
        }
        else
        {
            anim.Play("JumpNoSword");
        }
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
    }

    public void TakeKnockback(float hKnockbackForce, float vKnockbackforce)
    {
        anim.SetBool("TakingKnockback", true);
        canMove = false;
        rgdb.velocity = new Vector2(hKnockbackForce, vKnockbackforce);
        Invoke("CanMove", 0.3f);
    }
}
