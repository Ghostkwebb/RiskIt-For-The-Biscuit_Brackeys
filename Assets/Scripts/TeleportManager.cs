using UnityEngine;
using Pathfinding;
using Pathfinding.Util;

public class TeleportManager : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("The minimum distance from the player to teleport.")]
    [SerializeField] private float minTeleportRadius = 10f;
    [Tooltip("The maximum distance from the player to teleport.")]
    [SerializeField] private float maxTeleportRadius = 15f;
    [Tooltip("How many times to try finding a valid spot before giving up.")]
    [SerializeField] private int maxAttempts = 50;

    private Transform playerTransform;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
    }

    public void TeleportPlayerRandomly()
    {
        Vector3 teleportPosition = Vector3.zero;
        bool positionFound = false;

        // 1. Get the player's current node just once before the loop.
        GraphNode startNode = AstarPath.active.GetNearest(playerTransform.position).node;

        // If the player is somehow not on a valid node, we can't teleport.
        if (startNode == null)
        {
            Debug.LogError("Player is not on a valid pathfinding node. Cannot teleport.");
            return;
        }

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minTeleportRadius, maxTeleportRadius);
            Vector3 potentialPosition = playerTransform.position + ((Vector3)randomDirection * randomDistance);

            // 2. Get the potential destination node.
            GraphNode endNode = AstarPath.active.GetNearest(potentialPosition).node;

            // 3. Check if the destination node is walkable AND if a path is possible between the start and end nodes.
            if (endNode != null && endNode.Walkable && PathUtilities.IsPathPossible(startNode, endNode))
            {
                // We use the node's position to ensure we land exactly on the grid.
                teleportPosition = (Vector3)endNode.position;
                positionFound = true;
                break;
            }
        }

        if (positionFound)
        {
            Debug.Log($"Teleporting player to {teleportPosition}");
            playerTransform.position = teleportPosition;
        }
        else
        {
            Debug.LogWarning("Could not find a valid and reachable teleport location after multiple attempts.");
        }
    }
}