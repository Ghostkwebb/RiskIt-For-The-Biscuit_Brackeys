using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunawayCoin : MonoBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.5f;

    // State
    private bool isFleeing = false;
    private Transform playerTransform;

    // Components
    private Rigidbody2D rb;
    private DebuffManager debuffManager;

    void Start()
    {
        InitializeComponents();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        debuffManager = FindFirstObjectByType<DebuffManager>();
    }

    private void HandleMovement()
    {
        if (isFleeing)
        {
            FleeFromPlayer();
        }
    }

    private void FleeFromPlayer()
    {
        // Find the best possible direction to escape
        Vector2 bestDirection = FindBestFleeDirection();

        // If bestDirection is (0,0), it means there's no escape path
        if (bestDirection == Vector2.zero)
        {
            Debug.Log("Coin is cornered!");
            isFleeing = false; // Give up
            return;
        }

        // Move in the chosen best direction
        rb.MovePosition(rb.position + bestDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // This is the new "brain" of the coin
    private Vector2 FindBestFleeDirection()
    {
        // 1. Define all possible directions
        Vector2[] allDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // 2. Find all directions that are not blocked by a wall
        List<Vector2> validDirections = new List<Vector2>();
        foreach (var dir in allDirections)
        {
            if (!IsPathBlocked(dir))
            {
                validDirections.Add(dir);
            }
        }

        // 3. If there are no valid directions, return Vector2.zero to signal we are trapped
        if (validDirections.Count == 0)
        {
            return Vector2.zero;
        }

        // 4. From the valid directions, find the one that points most away from the player
        Vector2 idealFleeDirection = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;

        // Use LINQ to find the best direction based on the dot product score
        // The highest score (closest to 1) is the direction most aligned with the ideal flee path
        return validDirections.OrderByDescending(dir => Vector2.Dot(dir, idealFleeDirection)).First();
    }

    private bool IsPathBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    // Called when the player enters the large trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Starting to flee.");
            isFleeing = true;
            playerTransform = other.transform;
        }
    }

    // Called when the player leaves the large trigger zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the area. Coin stopped.");
            isFleeing = false;
        }
    }

    // Called when the player physically collides with the coin
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