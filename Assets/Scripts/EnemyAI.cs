using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float leashDistance = 10f; // How far it will chase from its start point

    [Header("Combat")]
    [SerializeField] private int damage = 1;

    // A simple state machine to define the enemy's behavior
    private enum State
    {
        IDLE,
        CHASING,
        RETURNING
    }

    private State currentState;
    private Transform playerTransform;
    private Vector3 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        HandleState();
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        currentState = State.IDLE;
    }

    // This is our main state machine, called every physics update
    private void HandleState()
    {
        switch (currentState)
        {
            case State.IDLE:
                // Do nothing while idle, just wait for the player
                break;
            case State.CHASING:
                HandleChasing();
                break;
            case State.RETURNING:
                HandleReturning();
                break;
        }
    }

    private void HandleChasing()
    {
        // If the player gets too far from the enemy's starting point, give up and return.
        if (Vector2.Distance(transform.position, startPosition) > leashDistance)
        {
            currentState = State.RETURNING;
            return;
        }

        // If the player is somehow gone (e.g., died), return.
        if (playerTransform == null)
        {
            currentState = State.RETURNING;
            return;
        }

        // Move towards the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleReturning()
    {
        // Move towards the starting position
        Vector2 direction = (startPosition - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // If we have arrived back at the start, go back to idle.
        if (Vector2.Distance(transform.position, startPosition) < 0.1f)
        {
            currentState = State.IDLE;
        }
    }

    // --- DETECTION AND COMBAT ---

    // This is called when the player enters the large trigger (detection) circle
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy has spotted the player!");
            playerTransform = other.transform;
            currentState = State.CHASING;
        }
    }

    // This is called when the player leaves the detection circle
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy has lost the player.");
            playerTransform = null;
            currentState = State.RETURNING; // Start returning home
        }
    }

    // This is called when the enemy's physical collider hits another collider
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                // Optional: Add a small knockback or a pause after attacking here
            }
        }
    }
}