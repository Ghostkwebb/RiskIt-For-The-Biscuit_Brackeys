using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float leashDistance = 10f;

    [Header("Combat")]
    [SerializeField] private int damage = 1;


    private enum State { IDLE, CHASING, RETURNING }
    private State currentState;
    private Transform playerTransform;
    private Vector3 startPosition;

    // --- A* Component References ---
    private Seeker seeker;
    private AIPath aiPath;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        // We use Update for state checks that don't involve physics.
        HandleStateChecks();
    }

    private void Initialize()
    {
        startPosition = transform.position;
        currentState = State.IDLE;

        // Get the A* components
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();

        // Disable the AIPath component initially. We only want it active when chasing or returning.
        aiPath.canMove = false;
    }

    // This method decides WHEN to change state.
    private void HandleStateChecks()
    {
        switch (currentState)
        {
            case State.IDLE:
                // If we become idle, stop moving.
                aiPath.canMove = false;
                break;

            case State.CHASING:
                // If we are chasing, make sure we are moving and have a target.
                aiPath.canMove = true;
                aiPath.destination = playerTransform.position;

                // Check if we should stop chasing (leash distance).
                if (Vector2.Distance(transform.position, startPosition) > leashDistance)
                {
                    ChangeState(State.RETURNING);
                }
                break;

            case State.RETURNING:
                // If we are returning, make sure we are moving towards our start point.
                aiPath.canMove = true;
                aiPath.destination = startPosition;

                // Check if we have arrived home.
                // aiPath.reachedEndOfPath is a handy property for this.
                if (aiPath.reachedEndOfPath)
                {
                    ChangeState(State.IDLE);
                }
                break;
        }
    }

    // A helper method to cleanly change states and handle logic.
    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"Enemy changed state to: {currentState}");
    }

    // --- DETECTION AND COMBAT ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            ChangeState(State.CHASING);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            ChangeState(State.RETURNING);
        }
    }

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