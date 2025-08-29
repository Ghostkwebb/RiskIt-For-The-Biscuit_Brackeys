using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    // You can call this from an Animation Event at the end of the explosion clip
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}