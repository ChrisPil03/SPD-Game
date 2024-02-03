using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hotbar : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;

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
