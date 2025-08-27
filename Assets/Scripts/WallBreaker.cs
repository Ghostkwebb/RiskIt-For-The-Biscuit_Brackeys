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
        // First, check if we have enough coins
        if (playerStats.coinCount < coinCost)
        {
            Debug.Log("Not enough coins to break a wall!");
            return;
        }

        // Determine the target position in front of the player
        Vector3 playerPosition = transform.position;
        Vector3 direction = playerMovement.lastMovementDirection;

        // Get the cell of the first tile to break
        Vector3Int targetCell1 = wallTilemap.WorldToCell(playerPosition + direction);

        // Get the cell of the second tile (since the wall is 2 tiles thick)
        Vector3Int targetCell2 = wallTilemap.WorldToCell(playerPosition + direction * 2);

        // Check if there is actually a tile at the target locations
        if (wallTilemap.GetTile(targetCell1) != null || wallTilemap.GetTile(targetCell2) != null)
        {
            // If there's a wall, spend the coins and break it
            if (playerStats.UseCoins(coinCost))
            {
                Debug.Log("Breaking wall!");
                wallTilemap.SetTile(targetCell1, null); // Remove the first tile
                wallTilemap.SetTile(targetCell2, null); // Remove the second tile
            }
        }
        else
        {
            Debug.Log("No wall in front to break.");
        }
    }
}