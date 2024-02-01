using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Hotbar hotbar;
    public GameObject healthPotionImage;

    void Start()
    {
        hotbar = GameObject.FindGameObjectWithTag("Player").GetComponent<Hotbar>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < hotbar.slots.Length; i++)
            {
                if (!hotbar.isFull[i])
                {
                    hotbar.isFull[i] = true;
                    Instantiate(healthPotionImage, hotbar.slots[i].transform, false);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
