using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Hotbar hotbar;

    void Start()
    {
        hotbar = GameObject.FindGameObjectWithTag("Hotbar").GetComponent<Hotbar>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hotbar.AddHealtPotion();
            Destroy(gameObject);
        }
    }
}
