using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.VFX;

public class MediumSlimeWithOldMan : MonoBehaviour
{
    private Rigidbody2D rgdb;
    private SpriteRenderer rend;
    private Animator anim;

    [SerializeField] private GameObject oldMan;
    [SerializeField] private int giveXp = 10;
    [SerializeField] private float smoothing = 2f;
    [HideInInspector] public bool startJumping = false;
    private Color alphaColor;
    private Player player;

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

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        alphaColor = rend.material.color;
        alphaColor.a = 0f;

        currentHealth = startingHealth;
    }

    void Update()
    {
        anim.SetFloat("VerticalSpeed", rgdb.velocity.y);
        anim.SetBool("IsGrounded", IsGrounded());

        if (player.transform.position.x > transform.position.x)
        {
            FlipSprite(false);
        }
        else
        {
            FlipSprite(true);
        }
    }

    private void FixedUpdate()
    {
        if (!canJump || !startJumping) return;

        if (IsGrounded() && !isDead)
        {
            StartCoroutine(RandomJump());
        }
    }

    private void LateUpdate()
    {
        if (isDead)
        {
            Invoke("FloatToPlayer", 0.65f);
            Invoke("FadeAway", 0.65f);
        }
    }

    private void FlipSprite(bool direction)
    {
        rend.flipX = direction;
    }

    private IEnumerator RandomJump()
    {
        canJump = false;
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        if (!isDead && IsGrounded())
        {
            StartCoroutine(Jump(hForce, vForce));
        }
    }

    private IEnumerator Jump(float hForce, float vForce)
    {
        if (player.transform.position.x < transform.position.x && hForce != -hForce)
        {
            hForce = -hForce;
        }
        else if (player.transform.position.x > transform.position.x && hForce != Mathf.Abs(hForce))
        {
            hForce = Mathf.Abs(hForce);
        }

        anim.Play("Jump_WithOldMan");
        yield return new WaitForSeconds(0.3f);
        if (!isDead && IsGrounded())
        {
            rgdb.velocity = new Vector2(hForce, vForce);
            canJump = true;
        }
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
        anim.Play("Defeated_WithOldMan");
        player.GainXP(giveXp);
        Invoke("SpawnOldMan", 0.25f);
        Destroy(gameObject, 4f);
    }

    private void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
        else
        {
            anim.Play("TakingDamage_WithOldMan");
            canJump = false;
            StartCoroutine(Jump(hForce * 2, vForce * 0.75f));
        }
    }

    private void FloatToPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, smoothing * Time.deltaTime);
    }

    private void FadeAway()
    {
        rend.material.color = Color.Lerp(rend.material.color, alphaColor, smoothing * Time.deltaTime);
    }

    private void SpawnOldMan()
    {
        Instantiate(oldMan, transform.position, oldMan.transform.rotation);
        GameObject.FindGameObjectWithTag("SceneManager").GetComponent<TutorialLevel1Complete>().StartOldManDialogue();
    }
}

