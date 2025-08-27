using UnityEngine;

public class CoinDrainController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds between each coin loss.")]
    [SerializeField] private float drainInterval = 60f;

    // State
    private bool debuffActive = false;
    private float timer = 0f;
    private PlayerStats playerStats;

    void Start()
    {
        InitializeReferences();
    }

    void Update()
    {
        HandleCoinDrain();
    }

    private void InitializeReferences()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    private void HandleCoinDrain()
    {
        // Only run the logic if the debuff is active
        if (!debuffActive)
        {
            return;
        }

        // Increment the timer
        timer += Time.deltaTime;

        // If the timer reaches the interval
        if (timer >= drainInterval)
        {
            // Try to use one coin. The UseCoins method already handles the check.
            if (playerStats.UseCoins(1))
            {
                Debug.Log("Coin drain! You lost 1 coin.");
            }
            else
            {
                Debug.Log("Coin drain! But you have no coins to lose.");
            }

            // Reset the timer for the next interval
            timer = 0f;
        }
    }

    // This method is called by the DebuffManager to turn the drain ON.
    public void ActivateDebuff()
    {
        if (!debuffActive)
        {
            Debug.Log("COIN DRAIN DEBUFF ACTIVATED!");
            debuffActive = true;
            timer = 0f; // Reset timer when the debuff starts
        }
    }

    // This method is called by the DebuffManager to turn the drain OFF.
    public void DeactivateDebuff()
    {
        if (debuffActive)
        {
            Debug.Log("Coin drain has been removed!");
            debuffActive = false;
        }
    }
}