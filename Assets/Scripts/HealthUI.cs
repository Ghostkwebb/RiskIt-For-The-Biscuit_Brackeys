using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject heartPrefab;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    // We will store the CanvasGroup of each heart
    private List<CanvasGroup> heartCanvasGroups = new List<CanvasGroup>();

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        playerHealth.OnHealthChanged += UpdateHearts;
        CreateHeartIcons();
        UpdateHearts();
    }

    private void CreateHeartIcons()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        heartCanvasGroups.Clear();

        for (int i = 0; i < playerHealth.maxHealth; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            // Get the CanvasGroup component we added to the prefab
            heartCanvasGroups.Add(newHeart.GetComponent<CanvasGroup>());
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartCanvasGroups.Count; i++)
        {
            // If the current heart index is less than the player's health, make it visible.
            if (i < playerHealth.currentHealth)
            {
                heartCanvasGroups[i].alpha = 1f; // Fully visible
            }
            else // Otherwise, make it invisible.
            {
                heartCanvasGroups[i].alpha = 0f; // Fully transparent
            }
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHearts;
        }
    }
}