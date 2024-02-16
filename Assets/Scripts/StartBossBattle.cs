using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossBattle : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playerBlock;
    [SerializeField] private GameObject boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().isInBossBattle = true;
            boss.GetComponent<MediumSlimeWithOldMan>().startJumping = true;
            playerBlock.enabled = true;
        }
    }
}
