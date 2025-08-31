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

    [Header("Audio")]
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource chaseAudioSource;

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

    void OnEnable()
    {
        // When this enemy is enabled, subscribe to the death event
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDisable()
    {
        // When this enemy is disabled, unsubscribe to prevent errors
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
    }

    // This method is called automatically when the OnPlayerDied event is broadcasted
    private void HandlePlayerDeath()
    {
        Debug.Log("Basic Enemy: Player has died. Returning to start.");
        playerTransform = null;
        ChangeState(State.RETURNING); // Go back to the returning state
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
        bool isMoving = aiPath.canMove && !aiPath.reachedEndOfPath && aiPath.desiredVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            lastDirection = aiPath.desiredVelocity.normalized;
        }

        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);

        if (isMoving && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
            chaseAudioSource.Play();
        }
        else if (!isMoving && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
            chaseAudioSource.Stop();
        }
    }

    // This method decides WHEN to change state.
    private void HandleStateChecks()
    {
        switch (currentState)
        {
            case State.IDLE:
                aiPath.canMove = false;
                spriteRenderer.color = Color.white;
                break;

            case State.CHASING:
                aiPath.canMove = true;
                aiPath.destination = playerTransform.position;
                spriteRenderer.color = aggroColor;

                if (Vector2.Distance(transform.position, startPosition) > leashDistance)
                {
                    ChangeState(State.RETURNING);
                }
                break;

            case State.RETURNING:
                aiPath.canMove = true;
                aiPath.destination = startPosition;
                spriteRenderer.color = Color.white;

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

        AudioManager.Instance.PlaySFX("basic_enemy_attack");

        // Damage the player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}