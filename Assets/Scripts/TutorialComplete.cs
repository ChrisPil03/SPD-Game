using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialComplete : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogue;

    private int oldManCampScene = 1;

    public void StartOldManDialogue()
    {
        dialogueBox.SetActive(true);
        dialogue.text = "Thank you!";

        Invoke("ToOldManCamp", 5f);
    }
    private void ToOldManCamp()
    {
        SceneManager.LoadScene(oldManCampScene);
    }
}
