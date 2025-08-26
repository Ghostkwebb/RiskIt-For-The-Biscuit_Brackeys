using UnityEngine;

public class Coin : MonoBehaviour
{
    private DebuffManager debuffManager;

    void Start()
    {
        // Use the new, recommended method
        debuffManager = FindFirstObjectByType<DebuffManager>();
        if (debuffManager == null)
        {
            Debug.LogError("DebuffManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            debuffManager.TryApplyRandomDebuff();
            Destroy(gameObject);
        }
    }
}