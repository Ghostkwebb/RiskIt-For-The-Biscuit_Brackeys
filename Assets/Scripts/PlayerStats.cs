using UnityEngine;
// We no longer need TMPro here

public class PlayerStats : MonoBehaviour
{
    public int coinCount { get; private set; } = 0;


    void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinText(coinCount);
        }
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinText(coinCount);
            AudioManager.Instance.PlaySFX("coin_get");
        }
    }

    public bool UseCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCoinText(coinCount);
                AudioManager.Instance.PlaySFX("coin_use");
            }
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        coinCount = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoinText(coinCount);
        }
    }
}