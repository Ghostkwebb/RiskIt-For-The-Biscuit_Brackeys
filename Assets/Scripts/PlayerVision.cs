using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVision : MonoBehaviour
{
    [Header("Vision Components")]
    [Tooltip("Drag the 'LightPivot' object FROM THE HIERARCHY here.")]
    [SerializeField] private Transform lightPivot;

    private Rigidbody2D rb;
    private Vector3 mousePosition;

    // Debuff variables
    [SerializeField] private float minVisionAngle = 10f;
    private float initialVisionAngle;
    private Light2D visionLight;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        UpdateLightPivotTransform();
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        // We still get the light from the children of the pivot for the debuff logic
        if (lightPivot != null)
        {
            visionLight = lightPivot.GetComponentInChildren<Light2D>();
            if (visionLight != null)
            {
                initialVisionAngle = visionLight.pointLightOuterAngle;
            }
        }
    }

    private void HandleInput()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // This method now handles BOTH position and rotation in the correct order.
    private void UpdateLightPivotTransform()
    {
        if (lightPivot == null) return;

        lightPivot.position = rb.position;

        Vector2 direction = new Vector2(
            mousePosition.x - rb.position.x,
            mousePosition.y - rb.position.y
        );
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