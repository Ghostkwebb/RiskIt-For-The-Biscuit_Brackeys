using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Singleton instance
    public static UIManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinCountText;

    [Header("Health UI")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform healthContainer;
    [SerializeField] private Sprite fullHeartSprite;

    private List<GameObject> heartObjects = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // A public method that any script can call to update the coin text
    public void UpdateCoinText(int count)
    {
        if (coinCountText != null)
        {
            coinCountText.text = $"Coins: {count}";
        }
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // First, ensure we have the correct number of heart icons
        if (heartObjects.Count != maxHealth)
        {
            // Clear old hearts
            foreach (GameObject heart in heartObjects)
            {
                Destroy(heart);
            }
            heartObjects.Clear();

            // Create new hearts
            for (int i = 0; i < maxHealth; i++)
            {
                GameObject newHeart = Instantiate(heartPrefab, healthContainer);
                heartObjects.Add(newHeart);
            }
        }

        // Now, update which hearts are active (visible)
        for (int i = 0; i < heartObjects.Count; i++)
        {
            heartObjects[i].SetActive(i < currentHealth);
        }
    }
}