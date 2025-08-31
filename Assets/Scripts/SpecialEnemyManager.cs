using UnityEngine;

public class SpecialEnemyManager : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private int coinThreshold = 2;

    [Header("Enemy References")]
    [SerializeField] private ConvergingEnemyAI[] convergingEnemies;

    private Vector3[] enemyStartPositions;
    private PlayerStats playerStats;
    private bool enemiesAreActive = false;

    void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("SpecialEnemyManager: Player died. Deactivating all enemies.");
        DeactivateAllEnemies();
    }


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

        enemyStartPositions = new Vector3[convergingEnemies.Length];
        for (int i = 0; i < convergingEnemies.Length; i++)
        {
            if (convergingEnemies[i] == null) continue;
            enemyStartPositions[i] = convergingEnemies[i].transform.position;
            convergingEnemies[i].gameObject.SetActive(false);
        }
    }

    private void CheckActivationCondition()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (playerStats == null) return;

        if (playerStats.coinCount >= coinThreshold)
        {
            if (!enemiesAreActive)
            {
                ActivateAllEnemies();
            }
        }
        else
        {
            if (enemiesAreActive)
            {
                DeactivateAllEnemies();
            }
        }
    }

    private void ActivateAllEnemies()
    {
        enemiesAreActive = true;
        foreach (ConvergingEnemyAI enemy in convergingEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }

    private void DeactivateAllEnemies()
    {
        enemiesAreActive = false;
        for (int i = 0; i < convergingEnemies.Length; i++)
        {
            if (convergingEnemies[i] == null) continue;

            // Before disabling, reset its position
            convergingEnemies[i].transform.position = enemyStartPositions[i];
            convergingEnemies[i].gameObject.SetActive(false); // Disable the whole object
        }
    }
}