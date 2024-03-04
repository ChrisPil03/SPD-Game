using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothing = 2f;
    [SerializeField] private float maxX; // Maximum X value for the camera boundary
    [SerializeField] public float minX; // Minimum X value for the camera boundary
    [SerializeField] private float maxY; // Maximum Y value for the camera boundary
    [SerializeField] public float minY; // Minimum Y value for the camera boundary

    [Header("Boss Battle Position")]
    [SerializeField] private float x;
    [SerializeField] private float y;
    [SerializeField] private float z;
    [HideInInspector] public bool isInBossBattle = false;
    private bool inPosition;

    void LateUpdate()
    {
        if (!isInBossBattle)
        {
            // Calculate the desired position for the camera
            Vector3 desiredPosition = new Vector3(
                Mathf.Clamp(playerTransform.position.x, minX, maxX),
                Mathf.Clamp(playerTransform.position.y + 1.5f, minY, maxY),
                transform.position.z
            );

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.deltaTime);

            // Apply the smoothed position to the camera's transform
            transform.position = smoothedPosition;
        }
        else if (isInBossBattle && !inPosition)
        {
            Vector3 bossBattlePosition = new Vector3(x, y, z);

            transform.position = Vector3.Lerp(transform.position, bossBattlePosition, 0.5f * Mathf.Pow(Time.deltaTime, 0.8f));
            
            if (transform.position == bossBattlePosition)
            {
                inPosition = true;
            }
        }

    }
}
