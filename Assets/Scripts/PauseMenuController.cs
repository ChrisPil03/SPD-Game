using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button pauseButton;
    [SerializeField] private AudioClip ButtonClick;

    [HideInInspector] static public bool isPaused;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        pauseMenu.SetActive(false);
        pauseButton.interactable = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(ButtonClick, 0.8f);

        pauseMenu.SetActive(true);
        pauseButton.interactable = false;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(ButtonClick, 0.8f);

        pauseMenu.SetActive(false);
        pauseButton.interactable = true;
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void BackToMenu()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(ButtonClick, 0.8f);
        GameObject.FindGameObjectWithTag("Hotbar").GetComponent<Hotbar>().RemoveAllItems();
        Time.timeScale = 1f;
        isPaused = false;

        StartCoroutine(BactToMenuAfterTime(0.2f));
    }

    private IEnumerator BactToMenuAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
