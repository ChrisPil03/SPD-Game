using UnityEngine;

public class SecretBox : MonoBehaviour
{
    [SerializeField] private GameObject interactionIcon;

    private bool canOpenBox;
    private bool hasBeenOpened;

    [SerializeField] private SpriteRenderer[] childRenderers;

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
        if (Input.GetKeyDown(KeyCode.E) && canOpenBox)
        {
            interactionIcon.SetActive(false);
            hasBeenOpened = true;
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
