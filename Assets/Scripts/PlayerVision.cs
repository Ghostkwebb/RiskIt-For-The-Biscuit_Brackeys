using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVision : MonoBehaviour
{
    [Header("Vision Components")]
    [SerializeField] private Transform lightPivot;

    // We need a reference to the movement script to know which way to face.
    private PlayerMovement playerMovement;

    // Debuff variables
    [SerializeField] private float minVisionAngle = 10f;
    private float initialVisionAngle;
    private Light2D visionLight;

    void Start()
    {
        Initialize();
    }

    // We no longer need Update for input.
    // void Update() { }

    void LateUpdate()
    {
        HandleRotation();
    }

    private void Initialize()
    {
        // Get the PlayerMovement component from this same GameObject.
        playerMovement = GetComponent<PlayerMovement>();

        if (lightPivot != null)
        {
            visionLight = lightPivot.GetComponentInChildren<Light2D>();
            if (visionLight != null)
            {
                initialVisionAngle = visionLight.pointLightOuterAngle;
            }
        }
    }

    private void HandleRotation()
    {
        if (lightPivot == null) return;

        Vector2 direction = playerMovement.lastCardinalDirection;

        lightPivot.up = direction;
    }

    // Debuff methods remain unchanged
    public void ReduceVision(float amount)
    {
        if (visionLight != null)
        {
            visionLight.pointLightOuterAngle -= amount;
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
}