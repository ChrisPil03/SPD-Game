using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel1Complete : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogue;

    private int camp = 1;

    public void StartOldManDialogue()
    {
        StartCoroutine(SetDialogueActive());
        dialogue.text = "Dialogue missing";

        Invoke("GoToCamp", 6f);
    }

    private IEnumerator SetDialogueActive()
    {
        yield return new WaitForSeconds(1f);
        dialogueBox.SetActive(true);
    }

    private void GoToCamp()
    {
        Player.keepValues = true;
        Player.changeSceneOnRespawn = true;
        Player.respawnScene = camp;
        SceneManager.LoadScene(camp);
    }
}
