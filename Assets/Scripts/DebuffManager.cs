using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    [Header("Debuff Settings")]
    [Range(0, 1)]
    private float debuffChance = 0.5f;

    [Header("Debuff Amounts")]
    [Tooltip("The amount to reduce player speed by.")]
    private float speedReductionAmount = 1f;

    [Tooltip("The amount to reduce the vision cone angle by.")]
    private float visionReductionAmount = 10f;

    // References to player components
    private PlayerMovement playerMovement;
    private PlayerVision playerVision;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerVision = FindFirstObjectByType<PlayerVision>();
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
        int debuffType = Random.Range(0, 2);

        if (debuffType == 0)
        {
            Debug.Log($"Applying Speed Debuff! Speed reduced by {speedReductionAmount}.");
            playerMovement.ReduceSpeed(speedReductionAmount);
        }
        else
        {
            Debug.Log($"Applying Vision Debuff! Angle reduced by {visionReductionAmount}.");
            playerVision.ReduceVision(visionReductionAmount);
        }
    }
}