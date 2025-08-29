using UnityEngine;

public class DebugController : MonoBehaviour
{
    [Header("Manager References")]
    [SerializeField] private DebuffManager debuffManager;
    [SerializeField] private MazeManager mazeManager;
    [SerializeField] private CoinDrainController coinDrainController;
    [SerializeField] private TeleportManager teleportManager;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerVision playerVision;

    [Header("Debuff Values (for testing)")]
    [SerializeField] private float speedReductionAmount = 1f;
    [SerializeField] private float visionReductionAmount = 5f;

    void Update()
    {
        HandleDebugInput();
    }

    private void HandleDebugInput()
    {
        // --- DEBUFF TRIGGERS ---

        // Press '1' to apply the Speed Debuff
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[DEBUG] Applying Speed Debuff.");
            playerMovement.ReduceSpeed(speedReductionAmount);
        }

        // Press '2' to apply the Vision Debuff
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("[DEBUG] Applying Vision Debuff.");
            playerVision.ReduceVision(visionReductionAmount);
        }

        // Press '3' to apply the Block Path Debuff
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("[DEBUG] Applying Block Path Debuff.");
            mazeManager.BlockPathBehindPlayer();
        }

        // Press '4' to apply the Coin Drain Debuff
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("[DEBUG] Applying Coin Drain Debuff.");
            coinDrainController.ActivateDebuff();
        }

        // Press '5' to apply the Teleport Debuff
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("[DEBUG] Applying Teleport Debuff.");
            teleportManager.TeleportPlayerRandomly();
        }

        // --- RESET FUNCTION ---

        // Press '0' to remove all debuffs via the Safe Zone logic
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("[DEBUG] Removing all debuffs.");
            debuffManager.RemoveAllDebuffs();
        }
    }
}