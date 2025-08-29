using System.Collections; // Required for Coroutines
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mine : MonoBehaviour
{
    [Header("Mine Settings")]
    [SerializeField] private float triggerDelay = 1.0f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;

    // References are found automatically
    private Tilemap floorTilemap;
    private Tilemap crackedFloorTilemap;
    private Tilemap holeCollisionTilemap;
    private Tile holeColliderTile;
    private Tile crackedTileAsset;

    private bool hasBeenTriggered = false;
    private Animator animator;

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        animator = GetComponent<Animator>();
        CrackedFloorManager floorManager = FindFirstObjectByType<CrackedFloorManager>();
        if (floorManager != null)
        {
            floorTilemap = floorManager.floorTilemap;
            crackedFloorTilemap = floorManager.crackedFloorTilemap;
            holeCollisionTilemap = floorManager.holeCollisionTilemap;
            holeColliderTile = floorManager.holeColliderTile;
            crackedTileAsset = floorManager.crackedTileAsset;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger once
        if (hasBeenTriggered) return;

        // Check if the trigger was a valid character
        if (other.GetComponent<PlayerHealth>() != null || other.GetComponent<EnemyAI>() != null)
        {
            hasBeenTriggered = true;
            // Start the explosion countdown
            StartCoroutine(ExplosionSequence());
        }
    }

    // A Coroutine allows us to create timed sequences
    private IEnumerator ExplosionSequence()
    {
        Debug.Log("Mine has been triggered! Countdown started...");

        // Optional: Speed up the ticking animation to give a visual warning
        if (animator != null)
        {
            animator.speed = 3.0f;
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(triggerDelay);

        // --- EXPLOSION LOGIC ---
        Explode();
    }

    private void Explode()
    {
        Debug.Log("BOOM!");

        // --- VISUAL AND TERRAIN EFFECTS ---
        Vector3 explosionCenter = transform.position;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, explosionCenter, Quaternion.identity);
        }

        Vector3Int centerCell = floorTilemap.WorldToCell(explosionCenter);
        CreateTerrainDamage(centerCell);

        // --- AREA OF EFFECT DAMAGE ---
        // Find all colliders within a 1.5 unit radius (which covers a 3x3 tile area)
        Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(explosionCenter, 1.5f);

        foreach (Collider2D col in objectsInBlastRadius)
        {
            // Check for a player and kill them
            PlayerHealth player = col.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // We can deal a huge amount of damage to ensure death
                player.TakeDamage(999);
            }

            // Check for a basic enemy and kill it
            if (col.GetComponent<EnemyAI>() != null)
            {
                Destroy(col.gameObject);
            }

            // Check for a special enemy and kill it
            if (col.GetComponent<ConvergingEnemyAI>() != null)
            {
                // It's better to deactivate the special enemy so its manager knows it's gone
                col.gameObject.SetActive(false);
            }
        }

        // Finally, destroy the mine object itself
        Destroy(gameObject);
    }

    private void CreateTerrainDamage(Vector3Int centerCell)
    {
        // Loop through the 3x3 grid
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int currentCell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);

                if (x == 0 && y == 0) // Center tile
                {
                    floorTilemap.SetTile(currentCell, null);
                    crackedFloorTilemap.SetTile(currentCell, null);
                    holeCollisionTilemap.SetTile(currentCell, holeColliderTile);
                }
                else // Outer tiles
                {
                    if (Random.value > 0.5f && floorTilemap.GetTile(currentCell) != null && crackedTileAsset != null)
                    {
                        floorTilemap.SetTile(currentCell, null);
                        crackedFloorTilemap.SetTile(currentCell, crackedTileAsset);
                    }
                }
            }
        }
    }
}