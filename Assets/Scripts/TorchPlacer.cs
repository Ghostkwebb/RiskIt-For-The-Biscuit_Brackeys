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

    private void Initialize()
    {
        playerTransform = transform;
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the player!");
        }

        if (torchPreview == null)
        {
            torchPreview = Instantiate(torchPrefab);
            torchPreview.name = "TorchPreview"; // Give it a consistent name

            torchPreview.GetComponent<Collider2D>().enabled = false;
            torchPreview.GetComponent<Animator>().enabled = false;
            torchPreview.GetComponentInChildren<Light2D>().enabled = false;

            previewRenderer = torchPreview.GetComponent<SpriteRenderer>();
        }

        // Always ensure the preview is hidden when the player spawns
        isPreviewActive = false;
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

        Vector3 playerPosition = GameManager.CurrentPlayerTransform.position;
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
            if (playerStats.UseCoins(torchCost))
            {
                Instantiate(torchPrefab, torchPreview.transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySFX("torch_place");
                isPreviewActive = false;
                torchPreview.SetActive(false);
            }
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider2D adjacentWall = Physics2D.OverlapCircle(position, 0.6f, wallLayer);
        Collider2D placementBlocked = Physics2D.OverlapPoint(position, wallLayer);
        Vector2 directionToTarget = position - playerTransform.position;
        float distanceToTarget = directionToTarget.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, directionToTarget, distanceToTarget, wallLayer);
        Vector3 playerPosition = GameManager.CurrentPlayerTransform.position;
        return adjacentWall != null && placementBlocked == null && hit.collider == null;
    }
}