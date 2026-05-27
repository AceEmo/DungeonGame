using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MinimapViewTests
{
    private GameObject _viewObject;
    private MinimapView _view;
    private RectTransform _container;
    private RectTransform _indicator;
    private RectTransform _panel;

    [SetUp]
    public void SetUp()
    {
        _viewObject = new GameObject();
        _view = _viewObject.AddComponent<MinimapView>();

        _panel = new GameObject("Panel").AddComponent<RectTransform>();
        _container = new GameObject("Container").AddComponent<RectTransform>();
        _indicator = new GameObject("Indicator").AddComponent<RectTransform>();

        var prefab = new GameObject("Prefab");
        prefab.AddComponent<RectTransform>();
        prefab.AddComponent<Image>();

        typeof(MinimapView).GetField("minimapPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_view, _panel);
        typeof(MinimapView).GetField("gridContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_view, _container);
        typeof(MinimapView).GetField("playerIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_view, _indicator);
        typeof(MinimapView).GetField("roomMinimapPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_view, prefab);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_viewObject);
        Object.Destroy(_panel.gameObject);
        Object.Destroy(_container.gameObject);
        Object.Destroy(_indicator.gameObject);
    }

    [UnityTest]
    public IEnumerator CreateIcon_ShouldAddIconToDictionaryAndContainer()
    {
        _view.SendMessage("Awake");
        yield return null;

        _view.CreateIcon(new Vector2Int(1, 2));

        Assert.AreEqual(1, _container.childCount);
    }

    [UnityTest]
    public IEnumerator ClearIcons_ShouldDestroyAllSpawnedIcons()
    {
        _view.SendMessage("Awake");
        yield return null;
        _view.CreateIcon(new Vector2Int(0, 0));

        _view.ClearIcons();
        yield return null;

        var icons = (Dictionary<Vector2Int, Image>)typeof(MinimapView)
            .GetField("minimapIcons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_view);

        Assert.AreEqual(0, icons.Count);
    }

    [UnityTest]
    public IEnumerator DisplayLargeMap_ShouldModifyPanelAnchors()
    {
        _view.SendMessage("Awake");
        yield return null;

        _view.DisplayLargeMap();

        Assert.AreEqual(new Vector2(0.05f, 0.05f), _panel.anchorMin);
        Assert.AreEqual(new Vector2(0.95f, 0.95f), _panel.anchorMax);
    }

    [UnityTest]
    public IEnumerator DisplayMinimap_ShouldRestoreOriginalScale()
    {
        _view.SendMessage("Awake");
        yield return null;
        _container.localScale = Vector3.one * 5f;

        _view.DisplayMinimap();

        Assert.AreEqual(Vector3.one, _container.localScale);
    }
}