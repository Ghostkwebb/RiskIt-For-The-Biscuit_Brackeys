using System;
using System.Collections;
using UnityEngine;

// This is a helper class to make our sound list look nice in the Inspector.
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Settings")]
    [Tooltip("Time in seconds for music tracks to fade in and out.")]
    [SerializeField] private float musicFadeTime = 2.0f;
    [Tooltip("The music tracks to alternate between.")]
    [SerializeField] private Sound[] musicPlaylist;

    [Header("Sound Library")]
    [SerializeField] private Sound[] sounds;

    private AudioSource activeMusicSource;
    private int currentTrackIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start the continuous ambient sound loop
        PlayAmbience("DungeonAmbience");

        // Set the initial active music source and start the music loop
        activeMusicSource = musicSourceA;
        StartCoroutine(MusicLoopCoroutine());
    }

    // --- SFX METHOD (remains the same) ---
    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null) return;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    // --- AMBIENCE METHOD ---
    public void PlayAmbience(string ambienceName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == ambienceName);
        if (s == null) return;
        ambienceSource.clip = s.clip;
        ambienceSource.volume = s.volume;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    // --- MUSIC LOOP LOGIC ---
    private IEnumerator MusicLoopCoroutine()
    {
        while (true) // This will loop forever
        {
            // 1. Pick the next track
            currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Length;
            Sound nextTrack = musicPlaylist[currentTrackIndex];

            // 2. Set up the inactive source with the new track
            AudioSource inactiveMusicSource = (activeMusicSource == musicSourceA) ? musicSourceB : musicSourceA;
            inactiveMusicSource.clip = nextTrack.clip;
            inactiveMusicSource.volume = 0;
            inactiveMusicSource.pitch = nextTrack.pitch;
            inactiveMusicSource.loop = true;
            inactiveMusicSource.Play();

            // 3. Fade out the old track and fade in the new one
            yield return StartCoroutine(FadeTracks(activeMusicSource, inactiveMusicSource));

            // 4. Switch the active source
            activeMusicSource = inactiveMusicSource;
        }
    }

    private IEnumerator FadeTracks(AudioSource oldSource, AudioSource newSource)
    {
        float timer = 0f;
        float startVolumeOld = oldSource.volume;

        while (timer < musicFadeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / musicFadeTime;

            oldSource.volume = Mathf.Lerp(startVolumeOld, 0, progress);
            newSource.volume = Mathf.Lerp(0, newSource.GetComponent<AudioSource>().volume, progress); // Assumes target volume is 1

            // Wait for the next frame
            yield return null;
        }

        oldSource.Stop();
        oldSource.volume = startVolumeOld; // Reset volume for next use
    }
}