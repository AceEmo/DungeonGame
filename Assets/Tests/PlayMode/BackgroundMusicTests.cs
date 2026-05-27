using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BackgroundMusicTests
{
    private GameObject _musicObject1;
    private GameObject _musicObject2;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        if (_musicObject1 != null) Object.Destroy(_musicObject1);
        if (_musicObject2 != null) Object.Destroy(_musicObject2);
        
        PlayerPrefs.DeleteAll();
        
        typeof(BackgroundMusic).GetField("instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator Awake_ShouldSetInstanceAndLoadSavedVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", 0.75f);
        _musicObject1 = new GameObject("Music");
        _musicObject1.AddComponent<AudioSource>();
        BackgroundMusic music = _musicObject1.AddComponent<BackgroundMusic>();

        yield return null;

        Assert.AreEqual(music, BackgroundMusic.Instance);
        Assert.AreEqual(0.75f, music.Volume);
    }

    [UnityTest]
    public IEnumerator Awake_WhenDuplicateInstanceExists_ShouldDestroyDuplicate()
    {
        _musicObject1 = new GameObject("FirstMusic");
        _musicObject1.AddComponent<AudioSource>();
        _musicObject1.AddComponent<BackgroundMusic>();

        _musicObject2 = new GameObject("SecondMusic");
        _musicObject2.AddComponent<AudioSource>();
        _musicObject2.AddComponent<BackgroundMusic>();

        yield return null;

        Assert.IsTrue(_musicObject2 == null);
    }

    [UnityTest]
    public IEnumerator VolumeProperty_ShouldClampAndSaveToPlayerPrefs()
    {
        _musicObject1 = new GameObject("Music");
        _musicObject1.AddComponent<AudioSource>();
        BackgroundMusic music = _musicObject1.AddComponent<BackgroundMusic>();
        yield return null;

        music.Volume = 1.5f;

        Assert.AreEqual(1.0f, music.Volume);
        Assert.AreEqual(1.0f, PlayerPrefs.GetFloat("MusicVolume"));
    }
}