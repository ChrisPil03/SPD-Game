using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    private AudioSource playerAudioSource;

    [SerializeField] private AudioClip openSkillTreeSound, buttonClick;
    [SerializeField] private GameObject interactPopUp;
    [SerializeField] private GameObject skillTree;
    [SerializeField] private TMP_Text infoText, errorText;

    [HideInInspector] public bool canAccessSkillTree = false;

    private void Start()
    {
        playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && skillTree != null)
        {
            interactPopUp.SetActive(true);
            canAccessSkillTree = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && skillTree != null)
        {
            interactPopUp.SetActive(false);
            canAccessSkillTree = false;
        }
    }

    private void Update()
    {
        if (PauseMenuController.isPaused) return;

        if (Input.GetKeyDown(KeyCode.E) && canAccessSkillTree)
        {
            playerAudioSource.pitch = 0.8f;
            playerAudioSource.PlayOneShot(openSkillTreeSound, 0.08f);
            infoText.text = "Use skill tokens        to learn new skills\r\nAcquire skill tokens by defeating slimes and receiving essence";
            skillTree.SetActive(true);
        }
    }

    public void ExitSkillTree()
    {
        playerAudioSource.PlayOneShot(buttonClick, 0.12f);
        skillTree.SetActive(false);
        errorText.text = "";
    }
}
