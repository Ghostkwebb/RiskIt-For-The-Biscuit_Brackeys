using System;
using UnityEngine;

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
    [SerializeField] private AudioSource musicSource; // We only need ONE for music now
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Library")]
    [SerializeField] private Sound[] sounds;

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

    // We no longer start music automatically. Other scripts will tell us when.

    public void PlaySFX(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip, s.volume);
        }
    }

    // This is now the main way to play music or ambience.
    public void Play(string soundName)
    {
        Sound s = FindSound(soundName);
        if (s == null) return;

        // Decide which source to use based on the 'loop' property
        AudioSource source = s.loop ? musicSource : sfxSource;

        // If it's ambience, use the dedicated ambience source
        if (soundName.Contains("Ambience"))
        {
            source = ambienceSource;
        }

        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch;
        source.loop = s.loop;
        source.Play();

        Debug.Log($"Playing sound: {soundName} on {source.name}");
    }

    private Sound FindSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: '" + soundName + "' not found!");
            return null;
        }
        return s;
    }
}