using UnityEngine;

public class SpecialEnemyManager : MonoBehaviour
{
    [Header("Activation")]
    [Tooltip("The number of coins the player needs to activate the enemy.")]
    [SerializeField] private int coinThreshold = 2;

    [Header("References")]
    [Tooltip("Drag the ConvergingEnemy from your scene here.")]
    [SerializeField] private GameObject convergingEnemy;

    // We need to store the enemy's starting position to send it back.
    private Vector3 enemyStartPosition;
    private PlayerStats playerStats;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        CheckActivationCondition();
    }

    private void Initialize()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (convergingEnemy != null)
        {
            // Store the starting position before disabling it.
            enemyStartPosition = convergingEnemy.transform.position;
            convergingEnemy.SetActive(false);
        }
    }

    private void CheckActivationCondition()
    {
        // If we don't have the necessary references, do nothing.
        if (playerStats == null || convergingEnemy == null)
        {
            return;
        }


        if (playerStats.coinCount >= coinThreshold && !convergingEnemy.activeSelf)
        {
            ActivateEnemy();
        }

        else if (playerStats.coinCount < coinThreshold && convergingEnemy.activeSelf)
        {
            DeactivateEnemy();
        }
    }

    private void ActivateEnemy()
    {
        Debug.Log("Coin threshold reached! The special enemy is now hunting you!");
        convergingEnemy.SetActive(true);
    }

    private void DeactivateEnemy()
    {
        Debug.Log("Player is no longer a high-value target. Enemy is retreating.");
        // Deactivate the enemy GameObject.
        convergingEnemy.SetActive(false);
        // Reset its position back to the start for the next time it activates.
        convergingEnemy.transform.position = enemyStartPosition;
    }
}