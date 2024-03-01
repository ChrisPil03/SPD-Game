using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossBattle : MonoBehaviour
{
    [SerializeField] public BoxCollider2D playerBlock;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().isInBossBattle = true;
            boss.GetComponent<MediumSlimeWithOldMan>().startJumping = true;
            playerBlock.enabled = true;
            spawnPoint.position = new Vector3(74, 10.5f, 0);

            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().newSpawnPosition = true;
        }
    }
}
