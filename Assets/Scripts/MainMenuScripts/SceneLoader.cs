using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneLoader : MonoBehaviour
{
    // A static variable to hold the index of the scene we want to load.
    // This is the key to communication between scenes.
    public static int sceneToLoad = -1;

    private VideoPlayer videoPlayer;

    void Start()
    {
        Initialize();
        StartLoadingSequence();
    }

    private void Initialize()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void StartLoadingSequence()
    {
        // If no scene was specified, just go back to the main menu.
        if (sceneToLoad < 0)
        {
            SceneManager.LoadScene(0); // MainMenu is scene 0
            return;
        }

        // Start the coroutine that handles the whole process
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // 1. Start playing the video
        videoPlayer.Play();

        // 2. Give the video a moment to start before we begin loading
        yield return new WaitForSeconds(0.5f);

        // 3. Start loading the target scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Prevent the scene from activating as soon as it's finished loading
        asyncLoad.allowSceneActivation = false;

        // 4. Wait until the video has finished playing
        // We check isPlaying and also the frame to be safe
        while (videoPlayer.isPlaying)
        {
            yield return null; // Wait for the next frame
        }

        // 5. The video is done. Now, allow the new scene to activate.
        Debug.Log("Video finished. Activating new scene.");
        asyncLoad.allowSceneActivation = true;
    }
}