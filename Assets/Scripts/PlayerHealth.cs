using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int initialMaxHealth = 3;

    public int maxHealth { get; private set; }

    public int currentHealth { get; private set; }

    [Header("Death")]
    [SerializeField] private GameObject coinPouchPrefab;

    private PlayerStats playerStats;
    private GameManager gameManager;

    public static event Action OnPlayerDied;

    void Awake()
    {
        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
    }

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        gameManager = FindFirstObjectByType<GameManager>();
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0) Die();
    }


    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        Debug.Log($"Max health increased to {maxHealth}!");
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        AudioManager.Instance.PlaySFX("player_death");

        // Broadcast the death event so enemies and managers can react.
        OnPlayerDied?.Invoke();

        // --- Coin Pouch Logic ---
        if (playerStats.coinCount > 0)
        {
            GameObject newPouch = Instantiate(coinPouchPrefab, transform.position, Quaternion.identity);
            CoinPouch pouchScript = newPouch.GetComponent<CoinPouch>();
            pouchScript.coinsHeld = playerStats.coinCount;
            newPouch.transform.SetParent(null);
            playerStats.ResetCoins();
        }

        // --- THIS IS THE ONLY RESPAWN CALL ---
        // We already have a reference to the gameManager from Start(), so we use that.
        if (gameManager != null)
        {
            gameManager.RespawnPlayer();
        }
        else
        {
            // Safety net in case the reference is lost.
            FindFirstObjectByType<GameManager>().RespawnPlayer();
        }

        // Finally, destroy this player object.
        Destroy(gameObject);
    }
}