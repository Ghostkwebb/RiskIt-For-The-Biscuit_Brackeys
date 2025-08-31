using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static PlayerHealth CurrentPlayer { get; private set; }
    public int RequiredCoinsToWin { get; private set; }
    public static event Action OnPlayerSpawned;

    [Header("Game Goal")]
    [Tooltip("The number of coins the player needs to open the escape door.")]
    [SerializeField] private int requiredCoinsToWin = 50;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startPoint;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 2.0f;
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private GameObject winScreenPanel;

    private CameraFollow mainCameraFollow;

    void Awake()
    {
        mainCameraFollow = FindFirstObjectByType<CameraFollow>();
        RequiredCoinsToWin = requiredCoinsToWin;
    }

    void Start()
    {
        mainCameraFollow = FindFirstObjectByType<CameraFollow>();

        PlayerHealth[] existingPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        foreach (var p in existingPlayers)
        {
            Destroy(p.gameObject);
        }

        SpawnPlayer();
    }

    public void AttemptToWin()
    {
        if (CurrentPlayer == null) return; // Can't win if dead

        PlayerStats playerStats = CurrentPlayer.GetComponent<PlayerStats>();
        if (playerStats.coinCount >= requiredCoinsToWin)
        {
            // --- WIN CONDITION MET ---
            Debug.Log("YOU WIN! You have escaped the maze.");
            if (winScreenPanel != null)
            {
                winScreenPanel.SetActive(true);
            }

            // Freeze the game
            Time.timeScale = 0f;

            // Disable player input
            CurrentPlayer.GetComponent<PlayerMovement>().enabled = false;
        }
        else
        {
            // --- WIN CONDITION NOT MET ---
            int needed = RequiredCoinsToWin - playerStats.coinCount;
            Debug.Log($"Door is locked! You need {needed} more coins.");

        }
    }

    private void SpawnPlayer()
    {
        GameObject newPlayerObject = Instantiate(playerPrefab, startPoint.position, startPoint.rotation);
        CurrentPlayer = newPlayerObject.GetComponent<PlayerHealth>();

        if (mainCameraFollow != null)
        {
            mainCameraFollow.SetTarget(newPlayerObject.GetComponent<Rigidbody2D>());
        }

        AssignSceneReferences(newPlayerObject);
        OnPlayerSpawned?.Invoke();
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        CurrentPlayer = null;
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
            deathScreenPanel.GetComponent<Animator>().SetTrigger("Show");
        }
        yield return new WaitForSeconds(respawnDelay);

        SpawnPlayer();

        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
    }

    private void AssignSceneReferences(GameObject playerObject)
    {
        // Find the tilemaps ONCE.
        Tilemap wallTilemap = GameObject.Find("Wall").GetComponent<Tilemap>();
        Tilemap floorTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();

        // Find the tile assets.
        CrackedFloorManager floorManager = FindFirstObjectByType<CrackedFloorManager>();
        TileBase floorTileAsset = floorManager.normalFloorTile; // Assuming you add this public reference

        // Get the scripts from the newly created player.
        WallBreaker wallBreaker = playerObject.GetComponent<WallBreaker>();
        TorchPlacer torchPlacer = playerObject.GetComponent<TorchPlacer>();

        // --- THE FIX for references ---
        if (wallBreaker != null)
        {
            wallBreaker.InitializeSceneReferences(wallTilemap, floorTilemap, floorTileAsset);
        }
        if (torchPlacer != null)
        {
            torchPlacer.InitializeSceneReferences(floorTilemap);
        }
    }
}