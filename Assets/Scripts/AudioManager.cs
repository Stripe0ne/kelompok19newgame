using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manages all audio in the game (BGM and SFX)
/// Singleton pattern - only one instance exists
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        // Singleton pattern biar cuma ada 1 AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Biar gak ilang pas ganti scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup audio sources kalo belom ada
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.parent = transform;
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.parent = transform;
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        LoadVolumeSettings();
    }

    private void Start()
    {
        ApplyVolume();
    }

    private void LoadVolumeSettings()
    {
        // Load saved volume dari PlayerPrefs
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void SaveVolumeSettings()
    {
        // Save volume ke PlayerPrefs
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        
        if (musicSource.clip == clip && musicSource.isPlaying) return; // Jangan restart kalo lagunya sama
        
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        SaveVolumeSettings();
    }

    private void ApplyVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}

