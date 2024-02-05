using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hotbar : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public GameObject healthPotionImage;

    public void AddHealtPotion()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!isFull[i])
            {
                isFull[i] = true;
                Instantiate(healthPotionImage, slots[i].transform, false);

                return;
            }
        }
    }

    public void RemoveItem()
    {
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (isFull[i])
            {
                isFull[i] = false;
                Destroy(slots[i].transform.GetChild(0).gameObject);
                return;
            }
        }
    }
}
