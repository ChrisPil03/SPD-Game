using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox, skipButton, statUpgradesButton;
    [SerializeField] private TMP_Text dialogueText;
    private string dialogue1 = "A swordsman without a sword is not much of a swordsman if you ask me. I will give you this sword, press [Enter] to swing it.";
    private string dialogue2 = "I will help you become stronger, but I need jars of slime and gems to do so.";
    private float textSpeed = 0.05f;
    private bool dialogueActive;
    private bool skipDialogue;
    [HideInInspector] static public bool firstInteraction = true;
    private bool hasAttacked;
    private bool hasOpenedStatUpgrades;

    [SerializeField] private GameObject statUpgradesTable;
    [HideInInspector] public bool canOpenStatUpgrades;
    
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && firstInteraction)
        {
            hasAttacked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueText.text = string.Empty;
            dialogueBox.SetActive(true);
            if (!firstInteraction)
            {
                skipButton.SetActive(true);
            }
            else
            {
                skipButton.SetActive(false);
            }
            statUpgradesButton.SetActive(false);

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
            canOpenStatUpgrades = false;
        }
    }

    private IEnumerator WriteOutText()
    {
        yield return new WaitForSeconds(0.8f);

        if (firstInteraction)
        {
            player.canMove = false;
            player.rgdb.velocity = Vector3.zero;
            player.transform.position = new Vector3(transform.position.x + 3.37f, transform.position.y, 0);
            player.anim.SetFloat("MoveSpeed", 0);
            player.FlipSprite(true);

            foreach (char c in dialogue1)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            Player.hasSword = true;
            player.anim.SetBool("HasSword", true);

            yield return new WaitUntil(EnterPressed);
            dialogueText.text = string.Empty;

            foreach (char c in dialogue2)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
            skipDialogue = false;
            skipButton.SetActive(false);
            statUpgradesButton.SetActive(true);
            canOpenStatUpgrades = true;

            yield return new WaitUntil(FirstTimeStatUpgrades);
            firstInteraction = false;
            player.canMove = true;
        }
        else
        {
            foreach (char c in dialogue2)
            {
                dialogueText.text += c;
                if (!skipDialogue)
                {
                    yield return new WaitForSeconds(textSpeed);
                }
            }
            skipDialogue = false;
            skipButton.SetActive(false);
            statUpgradesButton.SetActive(true);
            canOpenStatUpgrades = true;
        }
        dialogueActive = false;
    }

    private bool EnterPressed()
    {
        if (hasAttacked)
        {
            return true;
        }
        return false;
    }

    private bool FirstTimeStatUpgrades()
    {
        if (hasOpenedStatUpgrades)
        {
            return true;
        }
        return false;
    }

    public void SkipDialogue()
    {
        skipDialogue = true;
    }

    public void OpenStatUpgradesTable()
    {
        statUpgradesTable.SetActive(true);

        if (firstInteraction)
        {
            hasOpenedStatUpgrades = true;
        }
    }

    public void ExitStatUpgradesTable()
    {
        statUpgradesTable.SetActive(false);
    }
}
