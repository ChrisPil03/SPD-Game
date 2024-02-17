using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private BoxCollider2D trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(objectToSpawn, transform.position, objectToSpawn.transform.rotation);
            trigger.enabled = false;
        }
    }

}
