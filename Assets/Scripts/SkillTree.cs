using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private GameObject interactPopUp;
    [SerializeField] private GameObject skillTree;
    [SerializeField] private TMP_Text infoText, errorText;

    [HideInInspector] public bool canAccessSkillTree = false;

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
        if (Input.GetKeyDown(KeyCode.E) && canAccessSkillTree)
        {
            infoText.text = "Use skill tokens to learn new skills\r\nAcquire skill tokens by leveling up";
            skillTree.SetActive(true);
        }
    }

    public void ExitSkillTree()
    {
        skillTree.SetActive(false);
        errorText.text = "";
    }
}
