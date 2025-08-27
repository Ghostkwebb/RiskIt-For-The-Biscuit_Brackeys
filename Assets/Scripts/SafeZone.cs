using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private DebuffManager debuffManager;

    void Start()
    {
        // Find the DebuffManager at the start of the game
        debuffManager = FindFirstObjectByType<DebuffManager>();
        if (debuffManager == null)
        {
            Debug.LogError("SafeZone could not find the DebuffManager!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the player enters, call the central reset function.
        if (other.CompareTag("Player"))
        {
            debuffManager.RemoveAllDebuffs();
        }
    }
}