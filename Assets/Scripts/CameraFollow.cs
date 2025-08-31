using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The Rigidbody2D of the object to follow (the Player).")]
    [SerializeField] private Rigidbody2D target;

    [Header("Settings")]
    [Tooltip("How smoothly the camera follows the target. Lower is faster.")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset; //(0, 0, -10)

    void Update()
    {
        // If our target has been destroyed, try to find a new one.
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.GetComponent<Rigidbody2D>();
            }
        }

    }

    void LateUpdate()
    {
        HandleCameraFollow();
    }

    public void SetTarget(Rigidbody2D newTarget)
    {
        target = newTarget;
    }

    private void HandleCameraFollow()
    {
        // Ensure we have a target to follow
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = (Vector3)target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}