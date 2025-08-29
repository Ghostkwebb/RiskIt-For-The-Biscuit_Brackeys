using UnityEngine;

public class FloatingTextMesh : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeTime = 1.5f;

    private TextMesh textMesh;
    private float timeElapsed = 0f;
    private Color startColor;

    void Awake()
    {
        // Get the TextMesh component attached to this object
        textMesh = GetComponent<TextMesh>();
        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
    }

    void Update()
    {
        HandleMovementAndFade();
    }

    private void HandleMovementAndFade()
    {
        // Move the text up
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // Calculate the new transparency (alpha)
        timeElapsed += Time.deltaTime;
        float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));

        // Apply the new color with the new alpha
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        // Destroy this GameObject after it has completely faded
        if (timeElapsed > fadeTime)
        {
            Destroy(gameObject);
        }
    }

    // A public method that the Chest script will call to set the message
    public void SetText(string message)
    {
        if (textMesh != null)
        {
            textMesh.text = message;
        }
    }
}