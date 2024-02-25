using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    private string dialogue1 = "A swordsman without a sword is not much of a swordsman if you ask me. I will give you this sword, press [Enter] to swing it.";
    private string dialogue2 = "--[ A lot of important story... ]--";
    private string dialogue3 = "I will help you become stronger, but I need jars of slime to do so.";
    private float textSpeed = 0.05f;
    private bool dialogueActive;
    private bool firstInteraction = true;
    
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueText.text = string.Empty;
            dialogueBox.SetActive(true);
            
            if (!dialogueActive)
            {
                dialogueActive = true;
                StartCoroutine(WriteOutText());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && dialogueBox != null)
        {
            StopCoroutine(WriteOutText());
            dialogueBox.SetActive(false);
            dialogueText.text = string.Empty;
        }
    }

    private IEnumerator WriteOutText()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstInteraction)
        {
            firstInteraction = false;

            foreach (char c in dialogue1)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            Player.hasSword = true;
            player.anim.SetBool("HasSword", true);

            yield return new WaitForSeconds(2);
            dialogueText.text = string.Empty;

            foreach (char c in dialogue2)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }
        else
        {
            foreach (char c in dialogue3)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }
        dialogueActive = false;
    }
}
