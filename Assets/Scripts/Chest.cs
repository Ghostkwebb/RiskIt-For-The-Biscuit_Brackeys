using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Rewards")]
    [SerializeField] private int minCoins = 0;
    [SerializeField] private int maxCoins = 50;

    [Header("Interaction")]
    [Tooltip("How close the player needs to be to open the chest.")]
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private LayerMask playerLayer; // Set this to the Player's layer

    // State
    private bool hasBeenOpened = false;

    // Components
    private Animator animator;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleInteraction();
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
    }

    private void HandleInteraction()
    {
        // Don't do anything if the chest has already been opened
        if (hasBeenOpened)
        {
            return;
        }

        // Check for the "E" key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Find all colliders on the "Player" layer within a small circle around the chest
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, interactionDistance, playerLayer);

            // If a player was found nearby
            if (playerCollider != null)
            {
                // We found the player, so open the chest
                OpenChest(playerCollider.GetComponent<PlayerStats>());
            }
        }
    }

    private void OpenChest(PlayerStats playerStats)
    {
        hasBeenOpened = true;
        Debug.Log("Opening chest...");

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        int coinsToGive = Random.Range(minCoins, maxCoins + 1);

        if (playerStats != null)
        {
            playerStats.AddCoins(coinsToGive);
            Debug.Log($"Player found {coinsToGive} coins!");
        }
    }
}