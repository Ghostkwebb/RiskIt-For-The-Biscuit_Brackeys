using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
public class ConvergingEnemyAI : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int damage = 1;

    // References
    private Transform playerTransform;
    private Seeker seeker;
    private AIPath aiPath;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Get the A* components
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();

        // Find the player's transform to set as the target
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;

        // Tell the AIPath component what to follow
        if (playerTransform != null)
        {
            aiPath.destination = playerTransform.position;
        }

        // Start calculating paths repeatedly
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    // This method is called repeatedly to find the best path to the player
    private void UpdatePath()
    {
        // If the seeker is ready and we have a target, calculate a new path
        if (seeker.IsDone() && playerTransform != null)
        {
            seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
        }
    }

    // A callback function that runs when the path has been calculated
    private void OnPathComplete(Path p)
    {
        // If there was an error in path calculation, you can handle it here
        if (p.error)
        {
            Debug.LogError(p.errorLog);
        }
    }

    // We no longer need our own movement logic in FixedUpdate.
    // The AIPath component handles it automatically.

    // --- COMBAT ---
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}