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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();

        Slime.damageTakenFromSword = damage;
        MediumSlimeWithOldMan.damageTakenFromSword = damage;
    }

    private void Update()
    {
        if (player.flipped && !hasFlippedCollider)
        {
            FlipCollider();
            hasFlippedCollider = true;
        }
        else if (!player.flipped && hasFlippedCollider)
        {
            FlipCollider();
            hasFlippedCollider = false;
        }
        

        if (isAttacking)
        {
            timer += Time.deltaTime;
            if (timer > 0.35f)
            {
                timer = 0;
                isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && Player.hasSword)
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
    }

    void FlipCollider()
    {
        // Flip the collider by inverting the x-scale
        polygonCollider.transform.localScale = new Vector3(
            -polygonCollider.transform.localScale.x,
            polygonCollider.transform.localScale.y,
            polygonCollider.transform.localScale.z
        );
    }
}
