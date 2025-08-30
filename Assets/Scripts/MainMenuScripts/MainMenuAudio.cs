using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    void Start()
    {
        // On the main menu, we want to play the main music track.
        if (AudioManager.Instance != null)
        {
            // Make sure you have a sound named "DungeonMusic1" in your AudioManager library.
            AudioManager.Instance.Play("dungeon_music_1");
        }
    }
}