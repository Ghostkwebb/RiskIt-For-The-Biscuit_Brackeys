using System.Collections; // Required for Coroutines
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Player Respawn")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float respawnDelay = 2.0f; // Time to wait before respawning

    [Header("UI")]
    [Tooltip("The parent panel for the death screen UI.")]
    [SerializeField] private GameObject deathScreenPanel;

    private CameraFollow mainCameraFollow;

    public static GameObject activeCoinPouch;

    public static Transform CurrentPlayerTransform { get; private set; }
    public static event Action OnPlayerSpawned;

    void Awake()
    {
        PlayerMovement initialPlayer = FindFirstObjectByType<PlayerMovement>();
        if (initialPlayer != null)
        {
            CurrentPlayerTransform = initialPlayer.transform;
        }
    }

    void Start()
    {
        mainCameraFollow = FindFirstObjectByType<CameraFollow>();
    }

    public void RespawnPlayer()
    {
        // Use a coroutine to add a delay before the player respawns.
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log("Player died. Showing death screen...");

        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
            deathScreenPanel.GetComponent<Animator>().SetTrigger("Show");
        }

        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Respawning player now!");

        GameObject newPlayer = Instantiate(playerPrefab, startPoint.position, startPoint.rotation);

        CurrentPlayerTransform = newPlayer.transform;
        OnPlayerSpawned?.Invoke();
        var mainCameraFollow = FindFirstObjectByType<CameraFollow>();

        if (mainCameraFollow != null)
        {
            mainCameraFollow.SetTarget(newPlayer.GetComponent<Rigidbody2D>());
        }

        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
    }
}