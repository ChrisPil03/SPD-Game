using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed, jumpForce, fallMultiplier;
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask whatIsGround;

    private float horizontalValue;
    private float rayDistance = 0.1f;
    private bool doubleJump;

    private Vector2 vecGravity;

    private Rigidbody2D rgdb;
    private SpriteRenderer rend;

    //Dash
    [SerializeField] private float dashingPower, dashingTime, dashingCoolDown;
    private bool canDash = true;
    private bool isDashing;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        horizontalValue = Input.GetAxis("Horizontal");
        vecGravity = new Vector2(0, -Physics2D.gravity.y);

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

        // Fall faster
        if (rgdb.velocity.y < 0)
        {
            rgdb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

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
            rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce);
            doubleJump = true;
        }
        else if (doubleJump)
        {
            rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce * 0.8f);
            doubleJump = false;
        }

    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rgdb.gravityScale;
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
        rgdb.gravityScale = originalGravity;
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
}
