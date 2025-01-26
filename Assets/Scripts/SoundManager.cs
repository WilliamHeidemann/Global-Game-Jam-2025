using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Clips")]
    public AudioClip[] soundEffects;
    public AudioClip backgroundMusic;

    [Header("Audio Sources")]
    public AudioSource soundEffectSource;
    public AudioSource clawMovementSource;
    public AudioSource musicSource;

    private void Awake()
    {
        // Ensures only one SoundManager instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the SoundManager between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    // Function to play sound effects
    public void PlaySoundEffect(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < soundEffects.Length)
        {
            soundEffectSource.pitch = Random.Range(0.6f, 0.8f);
            soundEffectSource.PlayOneShot(soundEffects[soundIndex]);
        }
    }
    
    public void PlayClawMovementSound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < soundEffects.Length)
        {
            clawMovementSource.pitch = Random.Range(0.6f, 0.8f);
            clawMovementSource.PlayOneShot(soundEffects[soundIndex]);
        }
    }

    // Function to play background music
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            Debug.Log("Background music");
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (backgroundMusic == null)
        {
            Debug.LogWarning("Background music not assigned.");
        }
        else
        {
            Debug.Log("Background music is already playing.");
        }
    }

    // Function to stop background music
    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    // Function to adjust volume
    public void SetVolume(float volume)
    {
        soundEffectSource.volume = volume;
        musicSource.volume = volume;
    }
}
