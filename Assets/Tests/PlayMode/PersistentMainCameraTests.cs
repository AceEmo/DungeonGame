using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PersistentMainCameraTests
{
    private GameObject _cameraObject1;
    private GameObject _cameraObject2;

    [TearDown]
    public void TearDown()
    {
        if (_cameraObject1 != null) Object.Destroy(_cameraObject1);
        if (_cameraObject2 != null) Object.Destroy(_cameraObject2);
    }

    [UnityTest]
    public IEnumerator Awake_WhenNoOtherMainCameraExists_ShouldPersist()
    {
        _cameraObject1 = new GameObject("MainCamera");
        _cameraObject1.tag = "MainCamera";
        _cameraObject1.AddComponent<Camera>();
        
        _cameraObject1.AddComponent<PersistentMainCamera>();

        yield return null;

        Assert.IsNotNull(_cameraObject1);
    }

    [UnityTest]
    public IEnumerator Awake_WhenAnotherMainCameraExists_ShouldDestroyItself()
    {
        _cameraObject1 = new GameObject("ExistingMainCamera");
        _cameraObject1.tag = "MainCamera";
        _cameraObject1.AddComponent<Camera>();

        _cameraObject2 = new GameObject("DuplicateMainCamera");
        _cameraObject2.tag = "MainCamera";
        _cameraObject2.AddComponent<Camera>();
        _cameraObject2.AddComponent<PersistentMainCamera>();

        yield return null;

        Assert.IsTrue(_cameraObject2 == null);
    }
}