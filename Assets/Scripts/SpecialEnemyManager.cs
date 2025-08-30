using UnityEngine;

public class SpecialEnemyManager : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private int coinThreshold = 2;

    [Header("Enemy References")]
    [Tooltip("Drag all ConvergingEnemy instances from the Hierarchy into this list.")]
    [SerializeField] private ConvergingEnemyAI[] convergingEnemies;

    private Vector3[] enemyStartPositions;
    private PlayerStats playerStats;
    private bool enemiesAreActive = false;

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

        // Store the starting positions and then disable the entire enemy GameObject.
        enemyStartPositions = new Vector3[convergingEnemies.Length];
        for (int i = 0; i < convergingEnemies.Length; i++)
        {
            if (convergingEnemies[i] == null) continue;

            enemyStartPositions[i] = convergingEnemies[i].transform.position;
            convergingEnemies[i].gameObject.SetActive(false); // Disable the whole object
        }
    }

    private void CheckActivationCondition()
    {
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
                enemy.gameObject.SetActive(true); // Enable the whole object
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