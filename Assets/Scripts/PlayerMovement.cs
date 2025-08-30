using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minMoveSpeed = 1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private AudioSource walkingAudioSource;


    public Vector2 lastCardinalDirection { get; private set; } = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walkingAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        // Get raw horizontal and vertical input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // --- THIS IS THE NEW 4-DIRECTIONAL LOGIC ---
        if (moveX != 0)
        {
            movement = new Vector2(moveX, 0);
        }
        else if (moveY != 0)
        {
            movement = new Vector2(0, moveY);
        }
        else
        {
            movement = Vector2.zero; // No movement if no keys are pressed
        }

        if (movement.sqrMagnitude > 0.1f)
        {
            lastCardinalDirection = movement.normalized;
        }
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (movement.sqrMagnitude > 0.1f && !walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
        else if (movement.sqrMagnitude < 0.1f && walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Stop();
        }
    }

    public void ReduceSpeed(float amount)
    {
        moveSpeed -= amount;
        if (moveSpeed < minMoveSpeed)
        {
            moveSpeed = minMoveSpeed;
        }
    }

    public void ResetSpeed()
    {
        moveSpeed = 8f; // Assuming 8 is your initial speed. You might want to store this properly.
    }
}