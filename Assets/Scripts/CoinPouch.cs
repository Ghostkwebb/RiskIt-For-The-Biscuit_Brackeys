using UnityEngine;

public class CoinPouch : MonoBehaviour
{
    public int coinsHeld = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the player
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // Give the coins back to the player
                playerStats.AddCoins(coinsHeld);
                Debug.Log($"Retrieved coin pouch with {coinsHeld} coins!");

                // Play a success sound
                AudioManager.Instance.PlaySFX("PouchGet"); // Add this sound to AudioManager
            }

            // Destroy the pouch after it has been collected
            Destroy(gameObject);
        }
    }
}