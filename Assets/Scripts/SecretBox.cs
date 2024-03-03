using UnityEngine;

public class SecretBox : MonoBehaviour
{
    [SerializeField] private GameObject interactionIcon;

    private bool canOpenBox;
    private bool hasBeenOpened;

    [SerializeField] private SpriteRenderer[] childRenderers;
    [SerializeField] private GameObject gemsParticales;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(true);
            canOpenBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(false);
            canOpenBox = false;
        }
    }

    private void Update()
    {
        if (PauseMenuController.isPaused) return;

        if (Input.GetKeyDown(KeyCode.E) && canOpenBox)
        {
            Instantiate(gemsParticales, transform.position, gemsParticales.transform.rotation);
            interactionIcon.SetActive(false);
            hasBeenOpened = true;
            Player.gems += Random.Range(2, 4);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UpdateGemsText();
            Destroy(gameObject, 2f);
        }
    }

    private void LateUpdate()
    {
        if (hasBeenOpened)
        {
            FadeAway();
        }
    }

    private void FadeAway()
    {
        foreach (SpriteRenderer r in childRenderers)
        {
            Color alphaColor = r.material.color;
            alphaColor.a = 0f;

            r.material.color = Color.Lerp(r.material.color, alphaColor, 2 * Time.deltaTime);
        }
    }
}
