using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeOutUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeDuration = 1.5f;
    private float timer = 0f;

    void Awake()
    {
        // Ensure there's a CanvasGroup to fade
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        HandleFade();
    }

    private void HandleFade()
    {
        timer += Time.deltaTime;
        canvasGroup.alpha = 1 - (timer / fadeDuration);

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}