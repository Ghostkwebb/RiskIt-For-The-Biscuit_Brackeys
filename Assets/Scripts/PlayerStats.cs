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
        UpdateCoinUI();
        AudioManager.Instance.PlaySFX("coin_get");
    }

    // Call this to try and spend coins
    public bool UseCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            UpdateCoinUI();
            AudioManager.Instance.PlaySFX("coin_use");
            return true; // Success
        }

        Debug.Log("Not enough coins!");
        return false; // Failure
    }

    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            if (coinCount == 1 || coinCount == 0)
                coinCountText.text = $"Coin: {coinCount}";
            else
                coinCountText.text = $"Coins: {coinCount}";
        }
    }
}