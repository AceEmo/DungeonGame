using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    private AudioSource audioSource;

    public static BackgroundMusic Instance => instance;

    public float Volume
    {
        get => audioSource != null ? audioSource.volume : 0f;
        set
        {
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Clamp01(value);
            }
        }
    }

    private void Awake()
    {
        InitializeSingleton();
        audioSource = GetComponent<AudioSource>();
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
}