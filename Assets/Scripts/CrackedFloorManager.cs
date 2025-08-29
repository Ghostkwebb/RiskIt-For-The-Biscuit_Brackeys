using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrackedFloorManager : MonoBehaviour
{
    public Tile crackedTileAsset { get; private set; }
    public Tilemap crackedFloorTilemap { get; private set; }
    public Tilemap floorTilemap { get; private set; }
    public Tilemap holeCollisionTilemap { get; private set; }
    public Tile holeColliderTile { get; private set; }

    [Header("Tilemaps")]
    [SerializeField] private Tilemap initialFloorTilemap;
    [SerializeField] private Tilemap initialCrackedFloorTilemap;
    [SerializeField] private Tilemap initialHoleCollisionTilemap;

    [Header("Tile Assets")]
    [SerializeField] private Tile initialHoleColliderTile;
    [SerializeField] private Tile initialCrackedTileAsset;
    [SerializeField] private Tile normalFloorTile;


    [Header("Settings")]
    [SerializeField] private int fixCost = 2;

    // References
    private Transform playerTransform;
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;

    // To track which tiles have been stepped on
    private HashSet<Vector3Int> steppedOnCrackedTiles = new HashSet<Vector3Int>();
    private Vector3Int lastPlayerCell;

    void Awake()
    {
        crackedTileAsset = initialCrackedTileAsset;
        crackedFloorTilemap = initialCrackedFloorTilemap;
        floorTilemap = initialFloorTilemap;
        holeCollisionTilemap = initialHoleCollisionTilemap;
        holeColliderTile = initialHoleColliderTile;
    }

    void Start()
    {
        // Find player components
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        lastPlayerCell = crackedFloorTilemap.WorldToCell(playerTransform.position);
    }

    void Update()
    {
        Vector3Int currentPlayerCell = crackedFloorTilemap.WorldToCell(playerTransform.position);

        // --- Logic for Breaking Tiles ---
        // Check if the player has moved to a new cell
        if (currentPlayerCell != lastPlayerCell)
        {
            // Check if the new cell has a cracked tile that hasn't been stepped on yet
            if (crackedFloorTilemap.GetTile(currentPlayerCell) != null && !steppedOnCrackedTiles.Contains(currentPlayerCell))
            {
                // Mark it as stepped on, but don't break it yet. It breaks when you step *off* it.
                steppedOnCrackedTiles.Add(currentPlayerCell);
            }

            // Check if the *previous* cell the player was on needs to break
            if (steppedOnCrackedTiles.Contains(lastPlayerCell))
            {
                BreakTile(lastPlayerCell);
            }

            lastPlayerCell = currentPlayerCell;
        }

        // --- Logic for Fixing Tiles ---
        if (Input.GetKeyDown(KeyCode.F)) // Using 'F' for Fix
        {
            FixTileInFront();
        }
    }

    void BreakTile(Vector3Int cell)
    {
        Debug.Log($"Breaking tile at {cell}");
        // Remove the visual cracked tile
        crackedFloorTilemap.SetTile(cell, null);

        // Add an invisible collision tile to the hole map
        holeCollisionTilemap.SetTile(cell, holeColliderTile);

        // Remove the cell from the set so it's no longer tracked
        steppedOnCrackedTiles.Remove(cell);
    }

    void FixTileInFront()
    {
        // Check for coins first
        if (playerStats.coinCount < fixCost)
        {
            Debug.Log("Not enough coins to fix the floor!");
            return;
        }

        Vector3Int targetCell = holeCollisionTilemap.WorldToCell(playerTransform.position + (Vector3)playerMovement.lastCardinalDirection);

        // Check if there is a hole at the target location
        if (holeCollisionTilemap.GetTile(targetCell) != null)
        {
            if (playerStats.UseCoins(fixCost))
            {
                Debug.Log($"Fixing hole at {targetCell}");
                // Remove the invisible collider
                holeCollisionTilemap.SetTile(targetCell, null);
                // Add a normal floor tile back to the main floor
                floorTilemap.SetTile(targetCell, normalFloorTile);
                // Also remove it here to be absolutely sure
                steppedOnCrackedTiles.Remove(targetCell);
            }
        }
    }
}