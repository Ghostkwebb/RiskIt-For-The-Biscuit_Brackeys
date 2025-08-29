using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
public class ConvergingEnemyAI : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    // --- NEW REFERENCE ---
    [Header("Visuals")]
    [Tooltip("Drag the child 'Visuals' object here.")]
    [SerializeField] private Transform visualsTransform;

    private Transform playerTransform;
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    // We no longer need a direct reference to the SpriteRenderer

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateAnimationAndVisuals();
    }

    private void Initialize()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();

        // The animator is now on the child object
        if (visualsTransform != null)
        {
            animator = visualsTransform.GetComponent<Animator>();
        }

        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;

        if (playerTransform != null)
        {
            aiPath.destination = playerTransform.position;
        }

        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void UpdateAnimationAndVisuals()
    {
        bool isMoving = aiPath.desiredVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // --- THE FLIPPING LOGIC IS HERE ---
        float moveX = aiPath.desiredVelocity.x;
        if (moveX < -0.1f)
        {
            // Moving left: flip the visuals by setting X scale to -1
            visualsTransform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (moveX > 0.1f)
        {
            // Moving right: set scale back to normal
            visualsTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && playerTransform != null)
        {
            seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.errorLog);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
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

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}