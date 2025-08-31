using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrackedFloorManager : MonoBehaviour
{
    public Tilemap floorTilemap { get; private set; }
    public Tilemap crackedFloorTilemap { get; private set; }
    public Tilemap holeCollisionTilemap { get; private set; }
    public Tile holeColliderTile { get; private set; }
    public Tile crackedTileAsset { get; private set; }

    [Header("Tilemaps")]
    [SerializeField] private Tilemap initialFloorTilemap;
    [SerializeField] private Tilemap initialCrackedFloorTilemap;
    [SerializeField] private Tilemap initialHoleCollisionTilemap;

    [Header("Tile Assets")]
    [SerializeField] private Tile initialHoleColliderTile;
    [SerializeField] private Tile initialCrackedTileAsset;
    [SerializeField] public Tile normalFloorTile;

    [Header("Settings")]
    [SerializeField] private int fixCost = 2;

    private Transform playerTransform;
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;

    private HashSet<Vector3Int> steppedOnCrackedTiles = new HashSet<Vector3Int>();
    private Vector3Int lastPlayerCell;
    private bool isInitialized = false; // A flag to prevent errors on the first frame

    void Awake()
    {
        // Set up the public properties for other scripts.
        GameManager.OnPlayerSpawned += InitializePlayerReferences;
        floorTilemap = initialFloorTilemap;
        crackedFloorTilemap = initialCrackedFloorTilemap;
        holeCollisionTilemap = initialHoleCollisionTilemap;
        holeColliderTile = initialHoleColliderTile;
        crackedTileAsset = initialCrackedTileAsset;
    }

    void Start()
    {
        // Also call it once at the start for the initial player.
        InitializePlayerReferences();
    }

    void OnDestroy()
    {
        // Always unsubscribe!
        GameManager.OnPlayerSpawned -= InitializePlayerReferences;
    }


    void Update()
    {
        // If the player is dead, do nothing.
        if (GameManager.CurrentPlayer == null)
        {
            isInitialized = false; // Reset the init flag when the player is gone
            return;
        }

        // If the player is alive but we haven't set up our references yet, do it now.
        if (!isInitialized)
        {
            InitializePlayerReferences();
        }

        // --- All of your existing game logic can now run safely ---
        HandleCrackedFloorLogic();
        HandleFixTileLogic();
    }

    // This method is now called from Update when a new player is detected.
    private void InitializePlayerReferences()
    {
        if (GameManager.CurrentPlayer != null)
        {
            playerTransform = GameManager.CurrentPlayer.transform;
            playerStats = GameManager.CurrentPlayer.GetComponent<PlayerStats>();
            playerMovement = GameManager.CurrentPlayer.GetComponent<PlayerMovement>();
            isInitialized = true;
            // Reset lastPlayerCell here for the new player's position
            lastPlayerCell = crackedFloorTilemap.WorldToCell(playerTransform.position);
        }
    }

    private void HandleCrackedFloorLogic()
    {
        if (playerTransform == null) return; // Safety check
        Vector3Int currentPlayerCell = crackedFloorTilemap.WorldToCell(playerTransform.position);

        if (currentPlayerCell != lastPlayerCell)
        {
            if (crackedFloorTilemap.GetTile(currentPlayerCell) != null && !steppedOnCrackedTiles.Contains(currentPlayerCell))
            {
                steppedOnCrackedTiles.Add(currentPlayerCell);
            }
            if (steppedOnCrackedTiles.Contains(lastPlayerCell))
            {
                BreakTile(lastPlayerCell);
            }
            lastPlayerCell = currentPlayerCell;
        }
    }

    private void HandleFixTileLogic()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FixTileInFront();
        }
    }

    void BreakTile(Vector3Int cell)
    {
        crackedFloorTilemap.SetTile(cell, null);
        holeCollisionTilemap.SetTile(cell, holeColliderTile);
        steppedOnCrackedTiles.Remove(cell);
        AudioManager.Instance.PlaySFX("floor_breaking");
    }

    void FixTileInFront()
    {
        if (playerStats.coinCount < fixCost)
        {
            Debug.Log("Not enough coins to fix the floor!");
            return;
        }

        Vector3Int targetCell = holeCollisionTilemap.WorldToCell(playerTransform.position + (Vector3)playerMovement.lastCardinalDirection);

        if (holeCollisionTilemap.GetTile(targetCell) != null)
        {
            if (playerStats.UseCoins(fixCost))
            {
                AudioManager.Instance.PlaySFX("floor_fixing");
                holeCollisionTilemap.SetTile(targetCell, null);
                floorTilemap.SetTile(targetCell, normalFloorTile);
                steppedOnCrackedTiles.Remove(targetCell);
            }
        }
    }
}