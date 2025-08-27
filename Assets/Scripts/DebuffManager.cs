using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [Header("Debuff Settings")]
    [Range(0, 1)]
    [SerializeField] private float debuffChance = 0.5f;

    [Header("Debuff Amounts")]
    [SerializeField] private float speedReductionAmount = 1f;
    [SerializeField] private float visionReductionAmount = 5f;

    [Header("Controller References")]
    [SerializeField] private CoinDrainController coinDrainController; // <-- ADD THIS

    // References
    private PlayerMovement playerMovement;
    private PlayerVision playerVision;
    private MazeManager mazeManager;

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerVision = FindFirstObjectByType<PlayerVision>();
        mazeManager = FindFirstObjectByType<MazeManager>();
    }

    public void TryApplyRandomDebuff()
    {
        if (Random.value < debuffChance)
        {
            ApplyRandomDebuff();
        }
        else
        {
            Debug.Log("No debuff this time!");
        }
    }

    private void ApplyRandomDebuff()
    {
        // Increase the range to include the new debuff (0, 1, 2, or 3)
        int debuffType = Random.Range(0, 4);

        if (debuffType == 0)
        {
            Debug.Log($"Applying Speed Debuff!");
            playerMovement.ReduceSpeed(speedReductionAmount);
        }
        else if (debuffType == 1)
        {
            Debug.Log($"Applying Vision Debuff!");
            playerVision.ReduceVision(visionReductionAmount);
        }
        else if (debuffType == 2)
        {
            mazeManager.BlockPathBehindPlayer();
        }
        else // debuffType == 3
        {
            // Call the new debuff logic from our CoinDrainController
            coinDrainController.ActivateDebuff();
        }
    }

    public void RemoveAllDebuffs()
    {
        Debug.Log("SAFE ZONE REACHED: Removing all debuffs.");

        // Tell each component to reset itself
        playerMovement.ResetSpeed();
        playerVision.ResetVision();
        coinDrainController.DeactivateDebuff();
    }
}