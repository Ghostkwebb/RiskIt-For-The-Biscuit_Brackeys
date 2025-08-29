using UnityEngine;
using TMPro; // We don't need this here, but good to have for UI

public class Chest : MonoBehaviour
{
    [Header("Rewards")]
    [SerializeField] private int minCoins = 0;
    [SerializeField] private int maxCoins = 50;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Feedback")]
    [Tooltip("The FloatingTextMesh prefab to show messages.")]
    [SerializeField] private GameObject floatingTextPrefab;
    [Tooltip("The vertical offset for where the text appears.")]
    [SerializeField] private float textSpawnOffset = 1f;

    private bool hasBeenOpened = false;
    private Animator animator;

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
        animator = GetComponent<Animator>();
    }

    private void HandleInteraction()
    {
        if (hasBeenOpened) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, interactionDistance, playerLayer);
            if (playerCollider != null)
            {
                OpenChest(playerCollider.GetComponent<PlayerStats>(), playerCollider.GetComponent<PlayerHealth>());
            }
        }
    }

    private void OpenChest(PlayerStats playerStats, PlayerHealth playerHealth)
    {
        hasBeenOpened = true;
        animator.SetTrigger("Open");

        int outcome = Random.Range(0, 99);

        if (outcome >= 0 && outcome <= 69) // Get Coins
        {
            int coinsToGive = Random.Range(minCoins, maxCoins + 1);
            playerStats.AddCoins(coinsToGive);
            ShowFloatingText($"Found {coinsToGive} coins!");
        }
        else if (outcome >= 70 && outcome <= 89) // Mimic Attack
        {
            playerHealth.TakeDamage(1);
            ShowFloatingText("It's a Mimic!");
        }
        else
        {
            playerHealth.IncreaseMaxHealth(1);
            playerHealth.Heal(1);

            ShowFloatingText("Max Health Increased!");
        }
    }

    private void ShowFloatingText(string message)
    {
        if (floatingTextPrefab == null) return;

        Vector3 spawnPosition = transform.position + new Vector3(0, textSpawnOffset, 0);

        GameObject textObject = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);

        textObject.GetComponent<FloatingTextMesh>().SetText(message);
    }
}