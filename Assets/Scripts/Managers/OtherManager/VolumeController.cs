using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour
{
    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        InitializeSliderValue();
        volumeSlider.onValueChanged.AddListener(HandleVolumeChange);
    }

    private void InitializeSliderValue()
    {
        if (BackgroundMusic.Instance != null)
        {
            volumeSlider.value = BackgroundMusic.Instance.Volume;
        }
    }

    private void HandleVolumeChange(float value)
    {
        if (BackgroundMusic.Instance != null)
        {
            BackgroundMusic.Instance.Volume = value;
        }
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(HandleVolumeChange);
    }
}
