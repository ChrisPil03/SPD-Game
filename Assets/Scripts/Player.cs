using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private CapsuleCollider2D hitBox;
    [SerializeField] private GameObject statUpgradeTable = null;

    [HideInInspector] public static bool hasAlechemySkill;
    private float originalGravity;
    private float rayDistance = 0.1f;
    private bool takeDamageFromVines;
    private bool takingDamageFromVines;

    [HideInInspector] public bool newSpawnPosition;
    [HideInInspector] static public bool changeSceneOnRespawn;
    [HideInInspector] static public int respawnScene;
    [HideInInspector] static public int checkpointScenePlayerHasReached = 1;

    [HideInInspector] public Rigidbody2D rgdb;
    [HideInInspector] public SpriteRenderer rend;
    [HideInInspector] public Animator anim;
    [HideInInspector] public AudioSource audioSource;
    private Hotbar hotbar;
    private Color originalColor;
    private StaminaController staminaController;

    [Header("Collectables")]
    [SerializeField] private TMP_Text skillTokensSkillTreeText;
    [SerializeField] private TMP_Text skillTokensText;
    [HideInInspector] public static int skillTokens = 0;
    [SerializeField] private GameObject skillTokenAnim;
    [SerializeField] private TMP_Text jarsOfSlimeText;
    [HideInInspector] public static int jarsOfSlime = 0;
    [SerializeField] private TMP_Text gemsText;
    [HideInInspector] public static int gems = 0;

    [Header("Level")]
    [SerializeField] private XPBar xpBar;
    private int xpToLevelUp = 100;
    [HideInInspector] public static int currentXP;
    [HideInInspector] static public int extraXP = 0;

    [Header("Health")]
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private HealthBar healthbar;
    [SerializeField] private TMP_Text healthCounter;
    [HideInInspector] public static int currentHealth;
    private int getHealth = 20;
    [HideInInspector] static public int extraHealth = 0;

    [SerializeField] private GameObject redScreen;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 150f;
    private float originalMoveSpeed;
    private float horizontalValue;
    [HideInInspector] public bool canMove = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9.5f;
    [HideInInspector] static public bool canDoubleJump = false;
    private bool doubleJump;

    [Header("Dash")]
    [SerializeField] private float dashingPower = 12.5f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCoolDown = 0.5f;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public bool isDashing;

    //Combat
    [HideInInspector] static public bool hasSword;

    [Header("Particales")]
    [SerializeField] private GameObject groundParticles;
    [SerializeField] private GameObject healtPotionParticales;

    [Header("SoundEffects")]
    [SerializeField] private AudioClip[] stepsOnGrass;
    [SerializeField] private AudioClip hitFromVines;
    [SerializeField] private AudioClip healthPotion;
    [SerializeField] private AudioClip healthPotionOpen;
    [SerializeField] private AudioClip dashSwooch;
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private AudioClip skillTokenSound;
    [SerializeField] private AudioClip statUpgradesOpened;
    private float futureTimeWalk;
    private float currentTimeWalk;
    private float intervalTimeWalk = 0.4f;
    private int step = 0;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hotbar = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<Hotbar>();
        staminaController = GetComponent<StaminaController>();

        GetComponent<SwordAttack>().canAttack = false;

        skillTokenAnim.SetActive(false);

        originalGravity = rgdb.gravityScale;
        originalColor = rend.material.color;
        originalMoveSpeed = moveSpeed;

        futureTimeWalk = Time.time + intervalTimeWalk;
        currentTimeWalk = Time.time;

        currentHealth = startingHealth;
        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;


        xpBar.UpdateXPBar(currentXP);
        UpdateSkillTokensText();
        UpdateJarsOfSLime();
        UpdateGemsText();

        if (hasSword)
        {
            anim.SetBool("HasSword", true);
        }

        StartCoroutine(Respawn());

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
            staminaController.StaminaDash();
        }

        if (Input.GetKeyDown(KeyCode.R) && hotbar.isFull[0] && currentHealth < startingHealth)
        {
            StartCoroutine(UseHealthPotion());
            hotbar.RemoveItem();
        }

        if (hasSword && IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(AttackingWithSword());
            }
        }

        if (hasAlechemySkill && Input.GetKeyDown(KeyCode.Tab) && statUpgradeTable != null)
        {
            GameObject.FindGameObjectWithTag("OldManCamp").GetComponent<OldMan>().canOpenStatUpgrades = true;
            statUpgradeTable.SetActive(true);

            audioSource.pitch = 0.8f;
            audioSource.PlayOneShot(statUpgradesOpened, 0.08f);
        }

        currentTimeWalk = Time.time;
        if ((currentTimeWalk >= futureTimeWalk) && IsGrounded() && Mathf.Abs(rgdb.velocity.x) > 0.1)
        {
            futureTimeWalk = Time.time + intervalTimeWalk;
            audioSource.PlayOneShot(stepsOnGrass[step], 0.03f);
            step++;
            if (step > 1)
            {
                step = 0;
            }
        }

        if (currentHealth <= 20)
        {
            redScreen.SetActive(true);
        }
        else if (currentHealth > 20 && redScreen.activeSelf)
        {
            redScreen.SetActive(false);
        }

        anim.SetFloat("MoveSpeed", Mathf.Abs(rgdb.velocity.x));
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetBool("TakingKnockback", false);
    }

    private void FixedUpdate()
    {
        if (isDashing || !canMove || PauseMenuController.isPaused) return;

        rgdb.velocity = new Vector2(horizontalValue * moveSpeed * Time.deltaTime, rgdb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            hasSword = true;
            anim.SetBool("HasSword", true);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Vines"))
        {
            takeDamageFromVines = true;
            StartCoroutine(TakeDamageFromVines());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vines"))
        {
            takeDamageFromVines = false;
            takingDamageFromVines = false;
        }
    }

    public void FlipSprite(bool direction)
    {
        // Determine the new scale based on the direction
        Vector3 playerScale = transform.localScale;
        playerScale.x = direction ? -1 : 1; // Flip the player GameObject
        transform.localScale = playerScale;
    }

    private void Jump()
    {
        if (IsGrounded() || takingDamageFromVines)
        {
            rgdb.gravityScale = originalGravity * 0.5f;
            rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce);
            Instantiate(groundParticles, transform.position, groundParticles.transform.rotation);

            if (canDoubleJump)
            {
                doubleJump = true;
            }
        }
        else if (doubleJump)
        {
            staminaController.StaminaDoubleJump();
        }
    }

    public void DoubleJump()
    {
        ResetGravity();
        PlayDoubleJumpAnim();
        audioSource.PlayOneShot(dashSwooch, 0.1f);
        rgdb.velocity = new Vector2(rgdb.velocity.x, jumpForce * 1.2f);
        doubleJump = false;
    }

    public IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetBool("IsDashing", true);
        rgdb.gravityScale = 0f;

        audioSource.PlayOneShot(stepsOnGrass[0], 0.05f);
        audioSource.PlayOneShot(dashSwooch, 0.1f);

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

    private IEnumerator AttackingWithSword()
    {
        moveSpeed *= 0.3f;
        yield return new WaitForSeconds(0.26f);
        if (!canMove)
        {
            CanMove();
        }
        if (moveSpeed != originalMoveSpeed)
        {
            moveSpeed = originalMoveSpeed;
        }
    }

    public void ResetGravity()
    {
        rgdb.gravityScale = originalGravity;
    }

    public bool IsGrounded()
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

    private void CanAttackAgain()
    {
        GetComponent<SwordAttack>().canAttack = true;
    }

    private void PlayDoubleJumpAnim()
    {
        if (rgdb.velocity.y < -2)
        {
            if (hasSword)
            {
                anim.Play("FallReversedWithSword");
            }
            else
            {
                anim.Play("FallReversedNoSword");
            }
        }
        else
        {
            if (hasSword)
            {
                anim.Play("JumpWithSword");
            }
            else
            {
                anim.Play("JumpNoSword");
            }
        }
    }

    private IEnumerator Respawn()
    {
        CameraController cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        if (cameraController.isInBossBattle == true)
        {
            GameObject.FindGameObjectWithTag("StartBossBattle").GetComponent<StartBossBattle>().playerBlock.enabled = false;
            GameObject.FindGameObjectWithTag("Boss").GetComponent<MediumSlimeWithOldMan>().startJumping = false;
            cameraController.isInBossBattle = false;
            GameObject.FindGameObjectWithTag("Boss").GetComponent<MediumSlimeWithOldMan>().currentHealth = 30;
        }

        hitBox.enabled = true;
        rgdb.velocity = Vector2.zero;
        rgdb.gravityScale = originalGravity;
        transform.position = spawnPosition.position;
        FlipSprite(false);

        while (cameraController.transform.position.x >= 5 && !newSpawnPosition)
        {
            yield return null;
        }

        currentHealth = startingHealth;
        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;

        if (hasSword)
        {
            anim.Play("SpawningWithSword");
        }
        else
        {
            anim.Play("SpawningNoSword");
        }

        Invoke("CanAttackAgain", 2f);
        Invoke("CanMove", 2f);
    }

    private IEnumerator Die()
    {
        canMove = false;
        GetComponent<SwordAttack>().canAttack = false;
        yield return new WaitForSeconds(0.31f);
        if (canMove)
        {
            canMove = false;
        }
        hitBox.enabled = false;
        rgdb.gravityScale = 0;
        rgdb.velocity = new Vector2(0, 0.5f);
        if (hasSword)
        {
            anim.Play("DeathWithSword");
        }
        else
        {
            anim.Play("DeathNoSword");
        }
        yield return new WaitForSeconds(1.8f);
        if (!changeSceneOnRespawn)
        {
            StartCoroutine(Respawn());
        }
        else
        {
            SceneManager.LoadScene(respawnScene);
        }
    }

    private IEnumerator UseHealthPotion()
    {
        if (currentHealth + getHealth >= startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth += getHealth + extraHealth;
        }
        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;

        Vector2 particalesPosition = new Vector2(transform.position.x, transform.position.y + 0.25f);
        Instantiate(healtPotionParticales, particalesPosition, healtPotionParticales.transform.rotation);
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        yield return new WaitForSeconds(0.2f);
        audioSource.PlayOneShot(healthPotionOpen, 0.1f);
        audioSource.PlayOneShot(healthPotion, 0.1f);
    }

    private IEnumerator TakeDamageFromVines()
    {
        while (takeDamageFromVines)
        {
            TakeDamage(10);
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(hitFromVines, 0.1f);

            if (IsGrounded() && canMove)
            {
                takingDamageFromVines = true;
                rgdb.velocity = new Vector2(rgdb.velocity.x, 1f);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }

        healthbar.UpdateHealthBar(currentHealth);
        healthCounter.text = currentHealth + "/" + startingHealth;
    }

    private IEnumerator FlashRed()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
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
        if ((currentXP + xpAmount + extraXP) >= xpToLevelUp)
        {
            currentXP = (currentXP + xpAmount + extraXP) % xpToLevelUp;
            skillTokens++;

            StartCoroutine(PlayLevelUpSound());

            skillTokenAnim.SetActive(true);
            skillTokenAnim.GetComponent<Animator>().Play("SkillTokenAnimation");
            Invoke("UpdateSkillTokensText", 1.5f);
        }
        else 
        {
            currentXP += (xpAmount + extraXP);
        }

        xpBar.UpdateXPBar(currentXP);
    }

    private IEnumerator PlayLevelUpSound()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(levelUp, 0.4f);
        yield return new WaitForSeconds(1.5f);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(skillTokenSound, 0.4f);
    }

    public void UpdateSkillTokensText()
    {
        skillTokensSkillTreeText.text = skillTokens.ToString();
        skillTokensText.text = skillTokens.ToString();
        skillTokenAnim.SetActive(false);
    }

    public void UpdateJarsOfSLime()
    {
        jarsOfSlimeText.text = jarsOfSlime.ToString();
    }

    public void UpdateGemsText()
    {
        gemsText.text = gems.ToString();
    }
}
