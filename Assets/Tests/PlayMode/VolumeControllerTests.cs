using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class VolumeControllerTests
{
    private GameObject _musicObject;
    private GameObject _sliderObject;
    private Slider _slider;
    private VolumeController _volumeController;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        
        _musicObject = new GameObject("Music");
        _musicObject.AddComponent<AudioSource>();
        _musicObject.AddComponent<BackgroundMusic>();

        _sliderObject = new GameObject("Slider");
        _slider = _sliderObject.AddComponent<Slider>();
        _volumeController = _sliderObject.AddComponent<VolumeController>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_musicObject != null) Object.Destroy(_musicObject);
        if (_sliderObject != null) Object.Destroy(_sliderObject);
        
        PlayerPrefs.DeleteAll();
        
        typeof(BackgroundMusic).GetField("instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator Start_ShouldInitializeSliderValueWithMusicVolume()
    {
        BackgroundMusic.Instance.Volume = 0.8f;

        yield return null;

        Assert.AreEqual(0.8f, _slider.value);
    }

    [UnityTest]
    public IEnumerator OnSliderValueChanged_ShouldUpdateBackgroundMusicVolume()
    {
        yield return null;

        _slider.value = 0.2f;

        Assert.AreEqual(0.2f, BackgroundMusic.Instance.Volume);
    }
}