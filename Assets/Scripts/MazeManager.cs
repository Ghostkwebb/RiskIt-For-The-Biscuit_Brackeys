using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour
{
    [Header("Maze Components")]
    [Tooltip("The tilemap that contains the walls.")]
    [SerializeField] private Tilemap wallTilemap;

    [Tooltip("The tilemap that contains the floor.")]
    [SerializeField] private Tilemap floorTilemap;

    [Tooltip("The actual wall tile asset to be placed.")]
    [SerializeField] private TileBase wallTileAsset;

    // References
    private PlayerMovement playerMovement;
    private Transform playerTransform;

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerTransform = playerMovement.transform;
    }

    public void BlockPathBehindPlayer()
    {
        Vector2 behindDirection = -playerMovement.lastMovementDirection;
        Vector3Int behindCellDirection = new Vector3Int(Mathf.RoundToInt(behindDirection.x), Mathf.RoundToInt(behindDirection.y), 0);

        Vector3Int perpendicularDirection = (Mathf.Abs(behindDirection.y) > 0)
            ? new Vector3Int(1, 0, 0)
            : new Vector3Int(0, 1, 0);

        Vector3Int baseCellLayer1 = wallTilemap.WorldToCell(playerTransform.position + (Vector3)behindDirection);

        Vector3Int[] wallCells = new Vector3Int[6];

        wallCells[0] = baseCellLayer1;
        wallCells[1] = baseCellLayer1 + perpendicularDirection;
        wallCells[2] = baseCellLayer1 - perpendicularDirection;

        wallCells[3] = wallCells[0] + behindCellDirection;
        wallCells[4] = wallCells[1] + behindCellDirection;
        wallCells[5] = wallCells[2] + behindCellDirection;

        BuildWall(wallCells);
    }

    private void BuildWall(Vector3Int[] cells)
    {
        foreach (var cell in cells)
        {
            // 1. Remove the floor tile at this position.
            floorTilemap.SetTile(cell, null);

            // 2. Place the new wall tile.
            wallTilemap.SetTile(cell, wallTileAsset);
        }
        Debug.Log("Path blocked and floor removed!");
    }
}