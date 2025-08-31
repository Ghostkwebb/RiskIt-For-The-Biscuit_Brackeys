using UnityEngine;
using TMPro;

public class EscapeDoor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2.0f;
    [SerializeField] private LayerMask playerLayer;

    [Header("UI")]
    [Tooltip("The child Canvas object that holds the requirement text.")]
    [SerializeField] private GameObject requirementTextCanvas;

    private TextMeshProUGUI requirementText;
    private GameManager gameManager;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        HandleInteraction();
    }

    private void Initialize()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        // Find the TextMeshPro component on the child canvas
        if (requirementTextCanvas != null)
        {
            requirementText = requirementTextCanvas.GetComponentInChildren<TextMeshProUGUI>();
            // Update the text with the correct value from the GameManager
            if (gameManager != null && requirementText != null)
            {
                requirementText.text = $"Requires: {gameManager.RequiredCoinsToWin} Coins";
            }

            // Start with the text hidden
            requirementTextCanvas.SetActive(false);
        }
    }

    private void HandleInteraction()
    {
        // We now need to check if the player is nearby every frame for the UI
        if (IsPlayerNearby())
        {
            // If the player is near, show the text.
            if (requirementTextCanvas != null && !requirementTextCanvas.activeSelf)
            {
                requirementTextCanvas.SetActive(true);
            }

            // And check for the 'E' key press to try and win.
            if (Input.GetKeyDown(KeyCode.E) && gameManager != null)
            {
                gameManager.AttemptToWin();
            }
        }
        else
        {
            // If the player is not near, hide the text.
            if (requirementTextCanvas != null && requirementTextCanvas.activeSelf)
            {
                requirementTextCanvas.SetActive(false);
            }
        }
    }

    private bool IsPlayerNearby()
    {
        return Physics2D.OverlapCircle(transform.position, interactionDistance, playerLayer) != null;
    }
}