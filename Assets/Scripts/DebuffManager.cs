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
    [SerializeField] private CoinDrainController coinDrainController;

    [Header("UI")]
    [SerializeField] private DebuffUIManager uiManager;

    // References
    private PlayerMovement playerMovement;
    private PlayerVision playerVision;
    private MazeManager mazeManager;
    private TeleportManager teleportManager;
    private int speedDebuffStacks = 0;
    private int visionDebuffStacks = 0;

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
            uiManager.ShowTemporaryDebuff("No Debuff!", Color.green);
        }
    }

    private void ApplyRandomDebuff()
    {
        int debuffType = Random.Range(0, 5);

        switch (debuffType)
        {
            case 0: // Speed - Stacking
                speedDebuffStacks++;
                playerMovement.ReduceSpeed(speedReductionAmount);
                uiManager.AddOrUpdateStackingDebuff("Slow", Color.blue, speedDebuffStacks);
                break;
            case 1: // Vision - Stacking
                visionDebuffStacks++;
                playerVision.ReduceVision(visionReductionAmount);
                uiManager.AddOrUpdateStackingDebuff("Narrow Vision", Color.magenta, visionDebuffStacks);
                break;
            case 2: // Block Path - Temporary
                mazeManager.BlockPathBehindPlayer();
                uiManager.ShowTemporaryDebuff("Path Blocked", Color.gray);
                break;
            case 3: // Coin Drain - Stacking (but only 1 stack)
                coinDrainController.ActivateDebuff();
                uiManager.AddOrUpdateStackingDebuff("Coin Drain", Color.yellow, 1);
                break;
            case 4: // Teleport - Temporary
                teleportManager.TeleportPlayerRandomly();
                uiManager.ShowTemporaryDebuff("Teleported", Color.cyan);
                break;
        }
    }

    public void RemoveAllDebuffs()
    {
        Debug.Log("SAFE ZONE REACHED: Removing all debuffs.");

        playerMovement.ResetSpeed();
        playerVision.ResetVision();
        coinDrainController.DeactivateDebuff();

        // --- Clear UI and stacks ---
        speedDebuffStacks = 0;
        visionDebuffStacks = 0;
        uiManager.ClearAllStackingDebuffs();
    }
}