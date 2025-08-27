using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVision : MonoBehaviour
{
    private Light2D visionLight;
    private Vector3 mousePosition;

    [SerializeField] private float minVisionAngle = 10f;
    private float initialVisionAngle;

    void Start()
    {
        visionLight = GetComponentInChildren<Light2D>();
        if (visionLight != null)
        {
            initialVisionAngle = visionLight.pointLightOuterAngle;
        }
    }

    void Update()
    {
        GetMousePosition();
    }

    void FixedUpdate()
    {
        LightCone();
    }

    public void ReduceVision(float amount)
    {
        if (visionLight != null)
        {
            visionLight.pointLightOuterAngle -= amount;
            // Clamp the angle so it doesn't get too small
            if (visionLight.pointLightOuterAngle < minVisionAngle)
            {
                visionLight.pointLightOuterAngle = minVisionAngle;
            }
        }
    }

    public void ResetVision()
    {
        if (visionLight != null)
        {
            Debug.Log("Vision has been reset!");
            visionLight.pointLightOuterAngle = initialVisionAngle;
        }
    }

    private void GetMousePosition()
    {
        // Get the mouse position in world space
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void LightCone()
    {
        // Rotate the vision cone to face the mouse cursor
        if (visionLight != null)
        {
            Vector2 direction = new Vector2(
                mousePosition.x - transform.position.x,
                mousePosition.y - transform.position.y
            );

            transform.up = direction;
        }
    }
}