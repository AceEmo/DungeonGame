using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class StaticBackgroundTests
{
    private GameObject backgroundObject;
    private StaticBackground staticBackground;

    [SetUp]
    public void Setup()
    {
        backgroundObject = new GameObject("StaticBackgroundTest");
        staticBackground = backgroundObject.AddComponent<StaticBackground>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(backgroundObject);
    }

    [Test]
    public void LateUpdateSetsLocalPositionToInitialValue()
    {
        backgroundObject.transform.localPosition = new Vector3(100f, 100f, 100f);

        MethodInfo startMethod = typeof(StaticBackground).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(staticBackground, null);

        MethodInfo lateUpdateMethod = typeof(StaticBackground).GetMethod("LateUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        lateUpdateMethod.Invoke(staticBackground, null);

        Assert.AreEqual(new Vector3(0f, 0f, 10f), backgroundObject.transform.localPosition);
    }
}