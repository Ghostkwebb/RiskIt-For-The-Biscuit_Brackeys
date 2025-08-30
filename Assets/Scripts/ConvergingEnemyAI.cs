using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))] // Or RichAI
public class ConvergingEnemyAI : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Drag the child object that contains the visuals (Sprite Renderer, Animator) here.")]
    public GameObject bodyObject;

    [Header("Combat")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    private Transform playerTransform;
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();

        if (bodyObject != null)
        {
            animator = bodyObject.GetComponent<Animator>();
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
        if (bodyObject == null) return;

        bool isMoving = aiPath.desiredVelocity.sqrMagnitude > 0.1f;
        if (animator != null) animator.SetBool("isMoving", isMoving);

        float moveX = aiPath.desiredVelocity.x;
        if (moveX < -0.1f)
        {
            bodyObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (moveX > 0.1f)
        {
            bodyObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        // If the enemy is NOT moving but the sound IS playing, stop it.
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
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
        AudioManager.Instance.PlaySFX("special_enemy_attack");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}