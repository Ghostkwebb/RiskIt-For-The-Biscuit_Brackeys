using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class TorchPlacer : MonoBehaviour
{
    [Header("Torch Settings")]
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private LayerMask wallLayer;

    [Header("Placement Grid")]
    [SerializeField] private Tilemap floorTilemap;

    private GameObject torchPreview;
    private SpriteRenderer previewRenderer;
    private bool isPreviewActive = false;

    private Transform playerTransform;

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
        playerTransform = transform; // Get the player's own transform

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
        if (!isPreviewActive)
        {
            return;
        }

        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = floorTilemap.WorldToCell(targetWorldPos);
        Vector3 snappedPosition = floorTilemap.GetCellCenterWorld(cellPosition);

        torchPreview.transform.position = snappedPosition;
        previewRenderer.color = IsValidPlacement(snappedPosition) ? Color.white : Color.red;
    }

    private void HandlePlacement()
    {
        if (IsValidPlacement(torchPreview.transform.position))
        {
            Instantiate(torchPrefab, torchPreview.transform.position, Quaternion.identity);

            isPreviewActive = false;
            torchPreview.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot place torch here!");
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        // 1. Is the target spot adjacent to a wall?
        Collider2D adjacentWall = Physics2D.OverlapCircle(position, 0.6f, wallLayer);

        // 2. Is the target spot itself empty (not inside a wall)?
        Collider2D placementBlocked = Physics2D.OverlapPoint(position, wallLayer);

        // 3. Is there a clear line of sight from the player to the target spot?
        Vector2 directionToTarget = position - playerTransform.position;
        float distanceToTarget = directionToTarget.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, directionToTarget, distanceToTarget, wallLayer);

        // It must be near a wall, not in a wall, and the raycast must NOT hit anything.
        return adjacentWall != null && placementBlocked == null && hit.collider == null;
    }
}