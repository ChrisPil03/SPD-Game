using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask whatIsGround;

    private float originalGravity;
    private float rayDistance = 0.1f;

    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;
    private Hotbar hotbar;

    //Skill
    [SerializeField] private TMP_Text SkillTokensText;
    [HideInInspector] public int skillTokens = 0;

    [Header("Level")]
    [SerializeField] private XPBar xpBar;
    [SerializeField] private TMP_Text level;
    private int currentLevel;
    private int xpToLevelUp = 100;
    private int currentXP;

    [Header("Health")]
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private HealthBar healthbar;
    [SerializeField] private TMP_Text healthCounter;
    [HideInInspector] public int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 150f;
    private float horizontalValue;
    private bool canMove = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9.5f;
    [HideInInspector] public bool canJumpHigh = false;
    [HideInInspector] public bool canDoubleJump = false;
    private bool doubleJump;

    [Header("Dash")]
    [SerializeField] private float dashingPower = 12.5f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCoolDown = 0.5f;
    [SerializeField] public bool hasDashSkill = false;
    private bool canDash = true;
    private bool isDashing;

    //Combat
    [HideInInspector] public bool hasSword = false;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hotbar = GetComponent<Hotbar>();

        originalGravity = rgdb.gravityScale;
        currentHealth = startingHealth;

        Invoke("CanMove", 2f);
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

        if ((Input.GetButtonUp("Jump") || rgdb.velocity.y < 0) && canJumpHigh)
        {
            ResetGravity();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && hasDashSkill)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.R) && hotbar.isFull[0] && currentHealth != startingHealth)
        {
            UseHealthPotion();
            hotbar.RemoveItem();
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
            if (canJumpHigh)
            {
                rgdb.gravityScale = originalGravity * 0.5f;
            }
            rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce);

            if (canDoubleJump)
            {
                doubleJump = true;
            }
        }
        else if (doubleJump)
        {
            DoubleJump();
        }
    }

    private void DoubleJump()
    {
        ResetGravity();
        PlayDoubleJumpAnim();
        rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce * 1.4f);
        doubleJump = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetBool("IsDashing", true);
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
        anim.SetBool("IsDashing", false);
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

    private void Respawn()
    {
        canMove = false;
        transform.position = spawnPosition.position;
        rgdb.velocity = Vector2.zero;
        currentHealth = startingHealth;
        anim.Play("SpawningNoSword");
        Invoke("CanMove", 2f);
    }

    private void UseHealthPotion()
    {
        currentHealth = startingHealth;
        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            Respawn();
        }

        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;
    }

    public void TakeKnockback(float hKnockbackForce, float vKnockbackforce)
    {

        if (currentHealth != startingHealth)
        {
            anim.SetBool("TakingKnockback", true);
            canMove = false;
            rgdb.velocity = new Vector2(hKnockbackForce, vKnockbackforce);
            Invoke("CanMove", 0.3f);
        }
    }

    public void GainXP(int xpAmount)
    {
        if ((currentXP + xpAmount) >= xpToLevelUp)
        {
            currentLevel++;
            level.text = "lv."+currentLevel;
            currentXP = (currentXP + xpAmount) % xpToLevelUp;
            skillTokens++;
            UpdateSkillTokensText();
        }
        else 
        {
            currentXP += xpAmount;
        }

        xpBar.UpdateXPBar(currentXP);
    }

    public void UpdateSkillTokensText()
    {
        SkillTokensText.text = skillTokens.ToString();
    }
}
