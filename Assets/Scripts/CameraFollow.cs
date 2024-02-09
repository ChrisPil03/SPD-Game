using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    [SerializeField] private float smoothing = 2f;
    [SerializeField] private float maxX; // Maximum X value for the camera boundary
    [SerializeField] private float minX; // Minimum X value for the camera boundary
    [SerializeField] private float maxY; // Maximum Y value for the camera boundary
    [SerializeField] private float minY; // Minimum Y value for the camera boundary

    //private Vector3 velocity = Vector3.zero; // Current velocity, this value is modified by the function every time you call it

    void LateUpdate()
    {
        // Calculate the desired position for the camera
        Vector3 desiredPosition = new Vector3(
            Mathf.Clamp(playerTransform.position.x, minX, maxX),
            Mathf.Clamp(playerTransform.position.y, minY, maxY),
            transform.position.z
        );

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.deltaTime);

        // Apply the smoothed position to the camera's transform
        transform.position = smoothedPosition;
    }
}
