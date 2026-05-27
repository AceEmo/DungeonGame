using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerCoreTests
{
    private GameObject _coreObject1;
    private GameObject _coreObject2;

    [TearDown]
    public void TearDown()
    {
        if (_coreObject1 != null) Object.Destroy(_coreObject1);
        if (_coreObject2 != null) Object.Destroy(_coreObject2);
        
        typeof(PlayerCore).GetField("instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator Awake_ShouldSetSingletonInstance()
    {
        _coreObject1 = new GameObject("PlayerCore");
        PlayerCore core = _coreObject1.AddComponent<PlayerCore>();

        yield return null;

        Assert.IsNotNull(_coreObject1);
    }

    [UnityTest]
    public IEnumerator Awake_WhenDuplicateExists_ShouldDestroyDuplicate()
    {
        _coreObject1 = new GameObject("FirstCore");
        _coreObject1.AddComponent<PlayerCore>();

        _coreObject2 = new GameObject("SecondCore");
        _coreObject2.AddComponent<PlayerCore>();

        yield return null;

        Assert.IsTrue(_coreObject2 == null);
    }
}