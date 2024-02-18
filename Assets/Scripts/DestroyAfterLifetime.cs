using UnityEngine;

public class DestroyAfterLifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
