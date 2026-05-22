using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private const string VolumeKey = "MusicVolume";
    private const float DefaultVolume = 0.5f;

    private static BackgroundMusic instance;
    private AudioSource audioSource;

    public static BackgroundMusic Instance => instance;

    public float Volume
    {
        get => audioSource != null ? audioSource.volume : DefaultVolume;
        set
        {
            if (audioSource != null)
            {
                float clampedValue = Mathf.Clamp01(value);
                audioSource.volume = clampedValue;
                
                PlayerPrefs.SetFloat(VolumeKey, clampedValue);
            }
        }
    }

    private void Awake()
    {
        InitializeSingleton();
        audioSource = GetComponent<AudioSource>();
        LoadVolumeSettings();
    }

    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumeSettings()
    {
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}