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

    // References to other components on the player
    private Transform playerTransform;
    private PlayerStats playerStats; // --- NEW: Reference to player's stats

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleInput();
        HandlePreview();
    }

    private void Initialize()
    {
        playerTransform = transform;

        // --- NEW: Get the PlayerStats component ---
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the player!");
        }

        torchPreview = Instantiate(torchPrefab);
        torchPreview.name = "TorchPreview";

        torchPreview.GetComponent<Collider2D>().enabled = false;
        torchPreview.GetComponent<Animator>().enabled = false;
        torchPreview.GetComponentInChildren<Light2D>().enabled = false;

        previewRenderer = torchPreview.GetComponent<SpriteRenderer>();
        torchPreview.SetActive(false);
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
        if (!isPreviewActive) return;

        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = floorTilemap.WorldToCell(targetWorldPos);
        Vector3 snappedPosition = floorTilemap.GetCellCenterWorld(cellPosition);

        torchPreview.transform.position = snappedPosition;
        previewRenderer.color = IsValidPlacement(snappedPosition) ? Color.white : Color.red;
    }

    // --- THIS IS THE UPDATED METHOD ---
    private void HandlePlacement()
    {
        // First, check if the placement location is valid
        if (!IsValidPlacement(torchPreview.transform.position))
        {
            Debug.Log("Cannot place torch here!");
            return; // Exit the method early
        }

        // NEW: Now, check if the player can afford the torch
        if (playerStats.UseCoins(torchCost))
        {
            // If UseCoins returns true, the player had enough and the cost has been deducted
            Debug.Log($"Placed torch! Spent {torchCost} coins.");
            Instantiate(torchPrefab, torchPreview.transform.position, Quaternion.identity);

            // Hide the preview after a successful placement
            isPreviewActive = false;
            torchPreview.SetActive(false);
        }
        else
        {
            // If UseCoins returns false, the player did not have enough coins
            Debug.Log($"Not enough coins to place a torch! Need {torchCost}.");
            // Here you could add a UI message or a sound effect for "can't afford"
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider2D adjacentWall = Physics2D.OverlapCircle(position, 0.6f, wallLayer);
        Collider2D placementBlocked = Physics2D.OverlapPoint(position, wallLayer);
        Vector2 directionToTarget = position - playerTransform.position;
        float distanceToTarget = directionToTarget.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, directionToTarget, distanceToTarget, wallLayer);

        return adjacentWall != null && placementBlocked == null && hit.collider == null;
    }
}