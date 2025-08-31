using UnityEngine;

public class Torch : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The cost of the torch, which will be refunded.")]
    [SerializeField] private int coinValue = 2;

    // State
    private bool playerIsNear = false;
    private PlayerStats playerStats;

    void Update()
    {
        HandlePickup();
    }

    private void HandlePickup()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.R))
        {
            RetrieveTorch();
        }
    }

    private void RetrieveTorch()
    {
        // 1. Refund the coins to the player
        if (playerStats != null)
        {
            playerStats.AddCoins(coinValue);
            Debug.Log($"Torch retrieved! Refunded {coinValue} coins.");
        }

        // 2. Play a pickup sound effect
        AudioManager.Instance.PlaySFX("torch_place"); // Make sure to add this sound to your AudioManager

        // 3. Destroy the torch object
        Destroy(gameObject);
    }

    // --- Trigger Detection ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            playerStats = other.GetComponent<PlayerStats>();
            // Optional: Show a UI prompt like "Press R to retrieve"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            playerStats = null;
            // Optional: Hide the UI prompt
        }
    }
}