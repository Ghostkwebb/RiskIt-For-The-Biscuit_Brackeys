using UnityEngine;
using UnityEngine.Tilemaps;

public class WallBreaker : MonoBehaviour
{
    [Header("Wall Breaking Settings")]
    [SerializeField] private int coinCost = 2;

    [Header("Tilemap References")]
    [Tooltip("The tilemap that contains the walls.")]
    [SerializeField] private Tilemap wallTilemap;

    [Tooltip("The tilemap where floor tiles should be placed.")]
    [SerializeField] private Tilemap floorTilemap;

    [Tooltip("The actual floor tile asset to place.")]
    [SerializeField] private TileBase floorTileAsset;

    // References
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        HandleInteraction();
    }

    private void Initialize()
    {
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void HandleInteraction()
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

        Vector3 direction = playerMovement.lastCardinalDirection;
        Vector3Int targetCell1 = wallTilemap.WorldToCell(transform.position + direction);
        Vector3Int targetCell2 = wallTilemap.WorldToCell(transform.position + direction * 2);

        // We only need to check if the first layer of wall exists to proceed
        if (wallTilemap.GetTile(targetCell1) != null)
        {
            if (playerStats.UseCoins(coinCost))
            {
                Debug.Log("Breaking wall and placing floor!");
                AudioManager.Instance.PlaySFX("wall_breaking");

                // Position 1
                wallTilemap.SetTile(targetCell1, null);
                floorTilemap.SetTile(targetCell1, floorTileAsset);

                // Position 2
                wallTilemap.SetTile(targetCell2, null);
                floorTilemap.SetTile(targetCell2, floorTileAsset);
            }
        }
        else
        {
            Debug.Log("No wall in front to break.");
        }
    }
}