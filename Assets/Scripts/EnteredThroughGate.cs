using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteredThroughGate : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playerBlock;

    static private int toriiNumber = 1;

    void Start()
    {
        playerBlock.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerBlock.enabled == false)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().minX = transform.position.x + 8.5f;
            }
            playerBlock.enabled = true;

            if (toriiNumber == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().minY = -15;
            }
        }
    }
}
