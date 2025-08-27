using UnityEngine;

public class Coin : MonoBehaviour
{
    private DebuffManager debuffManager;

    void Start()
    {
        debuffManager = FindFirstObjectByType<DebuffManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddCoins(1);
            }
            debuffManager.TryApplyRandomDebuff();
            Destroy(gameObject);
        }
    }
}