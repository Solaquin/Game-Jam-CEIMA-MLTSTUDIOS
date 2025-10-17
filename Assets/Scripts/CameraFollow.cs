using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform diverTransform; // Reference to the player's Transform
    [SerializeField] private float smoothSpeed = 0.3f; // Smoothing speed for camera movement
    [SerializeField] private Vector3 offset; // Offset from the player's position

    private void LateUpdate()
    {
        if (diverTransform == null)
            return;

        Vector3 targetPosition = new Vector3(
            diverTransform.position.x + offset.x,
            diverTransform.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
