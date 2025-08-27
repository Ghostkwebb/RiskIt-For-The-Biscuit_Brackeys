using UnityEngine;

public class RunawayCoin : MonoBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.5f;

    // State
    private bool isFleeing = false;
    private Vector2 lockedFleeDirection; // The direction the coin is locked into

    // Components
    private Rigidbody2D rb;
    private DebuffManager debuffManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        debuffManager = FindFirstObjectByType<DebuffManager>();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isFleeing)
        {
            FleeInLockedDirection();
        }
    }

    private void FleeInLockedDirection()
    {
        // Check if the path is blocked by a wall
        if (IsPathBlocked(lockedFleeDirection))
        {
            // If we hit a wall, we are cornered. Stop fleeing permanently.
            isFleeing = false;
            Debug.Log("Coin is cornered!");
        }
        else
        {
            // If the path is clear, continue moving in the locked direction.
            rb.MovePosition(rb.position + lockedFleeDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private bool IsPathBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    // This is called when the player enters the large trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the coin is already fleeing, do nothing.
        if (isFleeing || !other.CompareTag("Player"))
        {
            return;
        }

        DetermineFleeDirection(other.transform.position);

        isFleeing = true;
        Debug.Log($"Player detected! Fleeing in direction: {lockedFleeDirection}");
    }

    // Decides whether to flee horizontally or vertically based on the player's approach.
    private void DetermineFleeDirection(Vector3 playerPosition)
    {
        // Get the vector pointing from the coin to the player
        Vector2 directionToPlayer = playerPosition - transform.position;

        // Check if the player is approaching more from the sides or from top/bottom
        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
        {
            // Player is more horizontal to the coin, so flee horizontally.
            // If player is on the right (x > 0), flee left. Otherwise, flee right.
            lockedFleeDirection = (directionToPlayer.x > 0) ? Vector2.left : Vector2.right;
        }
        else
        {
            // Player is more vertical to the coin, so flee vertically.
            // If player is above (y > 0), flee down. Otherwise, flee up.
            lockedFleeDirection = (directionToPlayer.y > 0) ? Vector2.down : Vector2.up;
        }
    }

    // This is called when the player physically collides with the coin
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            CollectCoin(other.gameObject);
        }
    }

    private void CollectCoin(GameObject player)
    {
        Debug.Log("Runaway coin collected!");

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.AddCoins(1);
        }

        debuffManager.TryApplyRandomDebuff();
        Destroy(gameObject);
    }
}