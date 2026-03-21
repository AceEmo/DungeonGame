using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BackgroundMusicTests
{
    private GameObject firstMusicObj;

    [SetUp]
    public void Setup()
    {
        firstMusicObj = new GameObject("BackgroundMusic");
        firstMusicObj.AddComponent<BackgroundMusic>();
    }

    [TearDown]
    public void Teardown()
    {
        if (firstMusicObj != null)
        {
            Object.DestroyImmediate(firstMusicObj);
        }

        FieldInfo instanceField = typeof(BackgroundMusic).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [UnityTest]
    public IEnumerator AwakeDestroysDuplicateInstance()
    {
        yield return null;

        GameObject secondMusicObj = new GameObject("DuplicateMusic");
        secondMusicObj.AddComponent<BackgroundMusic>();

        yield return null;

        Assert.IsTrue(secondMusicObj == null);
        Assert.IsFalse(firstMusicObj == null);
    }
}