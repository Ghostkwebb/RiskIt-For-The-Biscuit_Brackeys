using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int initialMaxHealth = 3;

    public int maxHealth { get; private set; }

    public int currentHealth { get; private set; }
    public event Action OnHealthChanged;

    void Awake()
    {
        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        OnHealthChanged?.Invoke();
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        Debug.Log($"Max health increased to {maxHealth}!");
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
    }
}