using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject dialogCloud;
    [SerializeField] private TMP_Text dialogText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogBox.SetActive(true);
            dialogCloud.SetActive(true);
            dialogText.text = "Hello";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && dialogCloud != null && dialogBox != null)
        {
            dialogBox.SetActive(false);
            dialogCloud.SetActive(false);
            dialogText.text = "";
        }
    }
}
