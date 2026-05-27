using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UIManagerTests
{
    private GameObject _uiManagerObject1;
    private GameObject _uiManagerObject2;

    [TearDown]
    public void TearDown()
    {
        if (_uiManagerObject1 != null) Object.Destroy(_uiManagerObject1);
        if (_uiManagerObject2 != null) Object.Destroy(_uiManagerObject2);
        
        typeof(UIManager).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator Awake_ShouldSetInstanceCorrectly()
    {
        _uiManagerObject1 = new GameObject("UIManager");
        UIManager manager = _uiManagerObject1.AddComponent<UIManager>();

        yield return null;

        Assert.AreEqual(manager, UIManager.Instance);
    }

    [UnityTest]
    public IEnumerator Awake_WhenDuplicateInstanceExists_ShouldDestroyDuplicate()
    {
        _uiManagerObject1 = new GameObject("FirstUIManager");
        _uiManagerObject1.AddComponent<UIManager>();

        _uiManagerObject2 = new GameObject("SecondUIManager");
        _uiManagerObject2.AddComponent<UIManager>();

        yield return null;

        Assert.IsTrue(_uiManagerObject2 == null);
    }
}