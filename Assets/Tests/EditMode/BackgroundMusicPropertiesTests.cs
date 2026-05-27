using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BackgroundMusicPropertiesTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void Volume_WhenAudioSourceIsNull_ShouldReturnDefaultVolume()
    {
        GameObject holder = new GameObject();
        BackgroundMusic music = holder.AddComponent<BackgroundMusic>();
        music.GetType().GetField("audioSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(music, null);

        float currentVolume = music.Volume;

        Assert.AreEqual(0.5f, currentVolume);
        Object.DestroyImmediate(holder);
    }
}