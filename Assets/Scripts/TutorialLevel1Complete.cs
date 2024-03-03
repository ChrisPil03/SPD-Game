using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel1Complete : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogue;

    private string dialogueText = "Thank you good sir. Wait aren't you... So you were summoned here as well. It is dangerous here, let us go to my camp before more slimes appear.";
    private float textSpeed = 0.05f;
    private bool dialogueActive;

    private int camp = 2;

    private void Start()
    {
        dialogue.text = string.Empty;
    }

    public void StartOldManDialogue()
    {
        if (!dialogueActive)
        {
            dialogueActive = true;
            StartCoroutine(SetDialogueActive());
            StartCoroutine(WriteOutText());
        }
        Invoke("GoToCamp", 10f);
    }

    private IEnumerator SetDialogueActive()
    {
        yield return new WaitForSeconds(1f);
        dialogueBox.SetActive(true);
    }

    private IEnumerator WriteOutText()
    {
        yield return new WaitForSeconds(1.2f);
        
        foreach (char c in dialogueText)
        {
            dialogue.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        dialogueActive = false;
    }

    private void GoToCamp()
    {
        Player.keepValues = true;
        Player.changeSceneOnRespawn = true;
        Player.respawnScene = camp;
        Player.checkpointScenePlayerHasReached = camp;
        SceneManager.LoadScene(camp);
    }
}
