using UnityEngine;
using UnityEngine.Tilemaps; // Required for Tilemap operations

public class WallBreaker : MonoBehaviour
{
    [Header("Wall Breaking Settings")]
    [SerializeField] private int coinCost = 2;
    [SerializeField] private Tilemap wallTilemap;

    // References to other components on the player
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BreakWallInFront();
        }
    }

    private void BreakWallInFront()
    {
        if (playerStats.coinCount < coinCost)
        {
            Debug.Log("Not enough coins to break a wall!");
            return;
        }

        // --- THIS IS THE FIX ---
        // Change lastMovementDirection to lastCardinalDirection
        Vector3 direction = playerMovement.lastCardinalDirection;

        Vector3Int targetCell1 = wallTilemap.WorldToCell(transform.position + direction);
        Vector3Int targetCell2 = wallTilemap.WorldToCell(transform.position + direction * 2);

        if (wallTilemap.GetTile(targetCell1) != null || wallTilemap.GetTile(targetCell2) != null)
        {
            if (playerStats.UseCoins(coinCost))
            {
                wallTilemap.SetTile(targetCell1, null);
                wallTilemap.SetTile(targetCell2, null);
            }
        }
        else
        {
            Debug.Log("No wall in front to break.");
        }
    }
}