using UnityEngine;
using TMPro; // Add this line to use TextMeshPro

public class PlayerStats : MonoBehaviour
{
    public int coinCount { get; private set; } = 0; // The current number of coins

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinCountText;

    void Start()
    {
        UpdateCoinUI();
    }

    // Call this to add coins
    public void AddCoins(int amount)
    {
        coinCount += amount;
        Debug.Log($"Collected a coin! Total coins: {coinCount}");
        UpdateCoinUI();
    }

    // Call this to try and spend coins
    public bool UseCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            Debug.Log($"Spent {amount} coins. Remaining: {coinCount}");
            UpdateCoinUI();
            return true; // Success
        }

        Debug.Log("Not enough coins!");
        return false; // Failure
    }

    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = $"Coins: {coinCount}";
        }
    }
}