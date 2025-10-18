using Unity.VisualScripting;
using UnityEngine;
public enum TargetType
{
    Player,
    StaticPoint
}
public class CameraFollow : MonoBehaviour
{


    [SerializeField] private Transform target; // Reference to the player's Transform
    [SerializeField] private float smoothSpeed = 0.3f; // Smoothing speed for camera movement
    [SerializeField] private Vector3 offset; // Offset from the player's position

    private TargetType targetType = TargetType.Player; // Default target type

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (targetType == TargetType.Player)
        {
            FollowTarget();
        }
        else if (targetType == TargetType.StaticPoint)
        {
            Vector3 staticPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            transform.position = staticPosition;
        }
    }

    void FollowTarget()
    {
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget, TargetType newTargetType)
    {
        target = newTarget;
        targetType = newTargetType;
    }
}
