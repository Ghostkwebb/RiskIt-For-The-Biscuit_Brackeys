using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int initialMaxHealth = 3;
    public int maxHealth = 3;

    // Public property to let other scripts read the current health
    public int currentHealth { get; private set; }

    // This event will be broadcasted whenever health changes
    public event Action OnHealthChanged;

    void Awake()
    {
        // Start the game with full health
        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
    }

    // Call this method to damage the player
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Clamp the health so it doesn't go below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Announce that the health has changed
        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Call this method to heal the player
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        // Clamp the health so it doesn't go above the max
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Announce that the health has changed
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // For now, we'll just deactivate the player.
        // Later, this could trigger the coin drop and a respawn sequence.
        gameObject.SetActive(false);
    }
}