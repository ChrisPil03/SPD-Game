using System.Collections;
using UnityEngine;

public class SmallSlimeShell : MonoBehaviour
{
    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;
    private AudioSource audioSource;
    private Color originalColor;

    [SerializeField] private GameObject slime;
    [SerializeField] private AudioClip slimeSound;
    [SerializeField] private GameObject slimeParticles;
    private Color alphaColor;
    private Player player;

    //Groundcheck
    [SerializeField] private Transform ray;
    [SerializeField] private LayerMask whatIsGround;
    private float rayDistance = 0.1f;

    [Header("Health")]
    [SerializeField] private int startingHealth = 30;
    private int currentHealth;
    private bool isDead = false;

    [Header("Jump")]
    [SerializeField] private float vForce = 5f;
    [SerializeField] private float hForce = 2f;
    private bool canJump = true;

    [Header("Combat")]
    [SerializeField] private float giveBounceForce = 5f;
    [SerializeField] private float giveVKnockback = 4f;
    [SerializeField] private float giveHKnockback = 3f;
    [SerializeField] private int damageGiven = 10;
    [HideInInspector] static public int damageTakenFromSword;
    private bool canTakeDamage = true;

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private CapsuleCollider2D capsuleCollider;

    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        originalColor = rend.material.color;

        alphaColor = rend.material.color;
        alphaColor.a = 0f;

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
        if (!isDead && IsGrounded())
        {
            anim.Play("Jump");
            yield return new WaitForSeconds(0.3f);
            if (!isDead && IsGrounded())
            {
                Instantiate(slimeParticles, transform.position, slimeParticles.transform.rotation);
                PlaySlimeSound();
                rgdb.velocity = new Vector2(hForce, vForce);
            }
        }
        yield return new WaitForSeconds(Random.Range(1f, 6f));
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
            if (Player.hasSword)
            {
                collision.GetComponent<Animator>().Play("FallReversedWithSword");
            }
            else
            {
                collision.GetComponent<Animator>().Play("FallReversedNoSword");
            }
            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.GetComponent<Rigidbody2D>().velocity.x, giveBounceForce);
            player.TakeDamage(damageGiven);
        }

        if (collision.CompareTag("SwordAttackOne"))
        {
            TakeDamage(damageTakenFromSword);
        }
    }

    private void Die()
    {
        rgdb.velocity = Vector2.zero;
        rgdb.gravityScale = 0;
        canJump = false;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        anim.Play("ShellLost");
        slime.GetComponent<Slime>().canTakeDamage = false;
        Instantiate(slime, transform.position, slime.transform.rotation);
        Destroy(gameObject, 0.25f);
    }

    private void TakeDamage(int damageTaken)
    {
        if (!isDead && canTakeDamage)
        {
            canTakeDamage = false;
            currentHealth -= damageTaken;
            StartCoroutine(FlashRed());
            Instantiate(slimeParticles, transform.position, slimeParticles.transform.rotation);
            PlaySlimeSound();

            if (currentHealth <= 0)
            {
                isDead = true;
                Die();
            }
            else
            {
                anim.Play("TakingDamage");
            }
            Invoke("CanTakeDamageAgain", 0.05f);
        }
    }

    private void CanTakeDamageAgain()
    {
        canTakeDamage = true;
    }

    private IEnumerator FlashRed()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }

    private void PlaySlimeSound()
    {
        audioSource.pitch = Random.Range(0.6f, 1f);
        audioSource.PlayOneShot(slimeSound, 0.5f);
    }
}
