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
    [SerializeField] private float attackCooldown = 1.5f; // Time between attacks
    private float lastAttackTime = -999f; // Time when the last attack occurred

    [Header("Visuals")]
    [SerializeField] private Color aggroColor = Color.red;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private enum State { IDLE, CHASING, RETURNING }
    private State currentState;
    private Transform playerTransform;
    private Vector3 startPosition;

    // --- A* Component References ---
    private Seeker seeker;
    private AIPath aiPath;

    private Vector2 lastDirection = new Vector2(0, -1);

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        // We use Update for state checks that don't involve physics.
        HandleStateChecks();
        UpdateAnimation();
    }

    private void Initialize()
    {
        startPosition = transform.position;
        currentState = State.IDLE;

        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        aiPath.canMove = false;

        // --- Set initial facing direction in the animator ---
        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);
    }

    private void UpdateAnimation()
    {
        // 1. Determine if the AI is actively moving.
        bool isMoving = aiPath.canMove && !aiPath.reachedEndOfPath && aiPath.desiredVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // 2. If the AI is moving, update its last known direction.
        if (isMoving)
        {
            lastDirection = aiPath.desiredVelocity.normalized;
        }

        // 3. Set the Animator parameters based on the last known direction, REGARDLESS of whether it is moving or not.
        // This ensures the enemy keeps facing the correct way when it stops.
        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);
    }

    // This method decides WHEN to change state.
    private void HandleStateChecks()
    {
        switch (currentState)
        {
            case State.IDLE:
                // If we become idle, stop moving.
                aiPath.canMove = false;
                spriteRenderer.color = Color.white;
                break;

            case State.CHASING:
                // If we are chasing, make sure we are moving and have a target.
                aiPath.canMove = true;
                aiPath.destination = playerTransform.position;
                spriteRenderer.color = aggroColor;

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
                spriteRenderer.color = Color.white;

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
        if (other.collider.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            Attack(other.collider.GetComponent<PlayerHealth>());
        }
    }

    private void Attack(PlayerHealth playerHealth)
    {
        Debug.Log("Enemy is attacking the player!");

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack");

        // Damage the player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}