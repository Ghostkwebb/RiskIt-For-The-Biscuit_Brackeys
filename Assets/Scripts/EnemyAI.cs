using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float leashDistance = 10f;

    [Header("Combat")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    [Header("Visuals")]
    [SerializeField] private Color aggroColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource chaseAudioSource;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private enum State { IDLE, CHASING, RETURNING }
    private State currentState;
    private Vector3 startPosition;

    private Seeker seeker;
    private AIPath aiPath;
    private Vector2 lastDirection = new Vector2(0, -1);

    // No longer need to subscribe to events

    void Start()
    {
        Initialize();
    }

    void Update()
    {
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
        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);
    }

    private void HandleStateChecks()
    {
        switch (currentState)
        {
            case State.IDLE:
                aiPath.canMove = false;
                spriteRenderer.color = Color.white;
                StopAllSounds();
                break;

            case State.CHASING:
                // --- THE FIX: Constantly check for the player ---
                if (GameManager.CurrentPlayer == null)
                {
                    ChangeState(State.RETURNING);
                    return;
                }
                aiPath.canMove = true;
                aiPath.destination = GameManager.CurrentPlayer.transform.position;
                spriteRenderer.color = aggroColor;
                PlayChaseSound();

                if (Vector2.Distance(transform.position, startPosition) > leashDistance)
                {
                    ChangeState(State.RETURNING);
                }
                break;

            case State.RETURNING:
                aiPath.canMove = true;
                aiPath.destination = startPosition;
                spriteRenderer.color = Color.white;
                PlayWalkSound();

                if (aiPath.reachedEndOfPath)
                {
                    ChangeState(State.IDLE);
                }
                break;
        }
    }

    // --- AUDIO HELPER METHODS (from previous fix) ---
    private void PlayWalkSound()
    {
        if (chaseAudioSource.isPlaying) chaseAudioSource.Stop();
        if (!walkAudioSource.isPlaying) walkAudioSource.Play();
    }

    private void PlayChaseSound()
    {
        if (walkAudioSource.isPlaying) walkAudioSource.Stop();
        if (!chaseAudioSource.isPlaying) chaseAudioSource.Play();
    }

    private void StopAllSounds()
    {
        if (walkAudioSource.isPlaying) walkAudioSource.Stop();
        if (chaseAudioSource.isPlaying) chaseAudioSource.Stop();
    }

    private void UpdateAnimation()
    {
        // This logic is mostly fine, we just remove the redundant audio calls
        bool isMoving = aiPath.canMove && !aiPath.reachedEndOfPath && aiPath.desiredVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            lastDirection = aiPath.desiredVelocity.normalized;
        }
        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(State.CHASING);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        AudioManager.Instance.PlaySFX("basic_enemy_attack");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}