using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretBox : MonoBehaviour
{
    [SerializeField] private GameObject interactionIcon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(false);
        }
    }
}
