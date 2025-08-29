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
    private TeleportManager teleportManager;

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerVision = FindFirstObjectByType<PlayerVision>();
        mazeManager = FindFirstObjectByType<MazeManager>();
        teleportManager = FindFirstObjectByType<TeleportManager>();
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
        int debuffType = Random.Range(0, 5);

        switch (debuffType)
        {
            case 0:
                Debug.Log("Applying Speed Debuff!");
                playerMovement.ReduceSpeed(speedReductionAmount);
                break;
            case 1:
                Debug.Log("Applying Vision Debuff!");
                playerVision.ReduceVision(visionReductionAmount);
                break;
            case 2:
                mazeManager.BlockPathBehindPlayer();
                break;
            case 3:
                coinDrainController.ActivateDebuff();
                break;
            case 4:
                // --- CALL THE NEW DEBUFF ---
                Debug.Log("Applying Teleport Debuff!");
                teleportManager.TeleportPlayerRandomly();
                break;
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