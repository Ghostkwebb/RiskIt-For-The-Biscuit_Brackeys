using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minMoveSpeed = 1f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public Vector2 lastMovementDirection { get; private set; } = Vector2.down; // Start facing down

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    public void ReduceSpeed(float amount)
    {
        moveSpeed -= amount;
        // Clamp the speed so it doesn't go below our defined minimum
        if (moveSpeed < minMoveSpeed)
        {
            moveSpeed = minMoveSpeed;
        }
    }

    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // If the player is moving, update the last direction
        if (movement.sqrMagnitude > 0.1f)
        {
            lastMovementDirection = movement.normalized;
        }
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}