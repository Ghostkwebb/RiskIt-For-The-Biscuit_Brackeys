using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class TorchPlacer : MonoBehaviour
{
    [Header("Torch Settings")]
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private int torchCost = 2;

    [Header("Placement Grid")]
    [SerializeField] private Tilemap floorTilemap;

    private GameObject torchPreview;
    private SpriteRenderer previewRenderer;
    private bool isPreviewActive = false;

    // References to other components ON THIS player object
    private PlayerStats playerStats;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        HandleInput();
        HandlePreview();
    }

    public void InitializeSceneReferences(Tilemap floorMap)
    {
        floorTilemap = floorMap;
    }

    private void Initialize()
    {
        // Get the PlayerStats component from THIS GameObject.
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the player!");
        }

        // The torch preview creation logic is correct.
        if (torchPreview == null)
        {
            // Find a pre-existing preview if one was left behind from a previous player
            GameObject existingPreview = GameObject.Find("TorchPreview");
            if (existingPreview != null)
            {
                torchPreview = existingPreview;
            }
            else
            {
                torchPreview = Instantiate(torchPrefab);
                torchPreview.name = "TorchPreview";
            }

            torchPreview.GetComponent<Collider2D>().enabled = false;
            torchPreview.GetComponent<Animator>().enabled = false;
            torchPreview.GetComponentInChildren<Light2D>().enabled = false;
            previewRenderer = torchPreview.GetComponent<SpriteRenderer>();
        }

        isPreviewActive = false;
        torchPreview.SetActive(false);
    }

    public void SetFloorTilemap(Tilemap floorMap)
    {
        floorTilemap = floorMap;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPreviewActive = !isPreviewActive;
            torchPreview.SetActive(isPreviewActive);
        }

        if (isPreviewActive && Input.GetMouseButtonDown(0))
        {
            HandlePlacement();
        }
    }

    private void HandlePreview()
    {
        if (floorTilemap == null)
        {
            torchPreview.SetActive(false); // Hide preview if we can't place it
            return;
        }

        if (!isPreviewActive) return;

        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = floorTilemap.WorldToCell(targetWorldPos);
        Vector3 snappedPosition = floorTilemap.GetCellCenterWorld(cellPosition);

        torchPreview.transform.position = snappedPosition;
        previewRenderer.color = IsValidPlacement(snappedPosition) ? Color.white : Color.red;
    }

    private void HandlePlacement()
    {
        if (!IsValidPlacement(torchPreview.transform.position))
        {
            Debug.Log("Cannot place torch here!");
            return;
        }

        if (playerStats.UseCoins(torchCost))
        {
            Instantiate(torchPrefab, torchPreview.transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySFX("torch_place");
            isPreviewActive = false;
            torchPreview.SetActive(false);
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider2D adjacentWall = Physics2D.OverlapCircle(position, 0.6f, wallLayer);
        Collider2D placementBlocked = Physics2D.OverlapPoint(position, wallLayer);

        // --- THE FIX: Use this object's transform, not the GameManager's ---
        Vector2 directionToTarget = position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, wallLayer);

        return adjacentWall != null && placementBlocked == null && hit.collider == null;
    }
}