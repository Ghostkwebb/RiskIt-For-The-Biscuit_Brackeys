using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject heartPrefab;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private List<CanvasGroup> heartCanvasGroups = new List<CanvasGroup>();

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (heartCanvasGroups.Count != playerHealth.maxHealth)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            heartCanvasGroups.Clear();

            for (int i = 0; i < playerHealth.maxHealth; i++)
            {
                GameObject newHeart = Instantiate(heartPrefab, transform);
                heartCanvasGroups.Add(newHeart.GetComponent<CanvasGroup>());
            }
        }

        for (int i = 0; i < heartCanvasGroups.Count; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                heartCanvasGroups[i].alpha = 1f;
            }
            else
            {
                heartCanvasGroups[i].alpha = 0f;
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