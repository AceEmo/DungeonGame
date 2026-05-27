using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MinimapControllerTests
{
    private GameObject _controllerObject;
    private GameObject _playerObject;
    private MinimapController _controller;
    private MinimapView _mockView;

    [SetUp]
    public void SetUp()
    {
        _controllerObject = new GameObject();
        _mockView = _controllerObject.AddComponent<MinimapView>();
        
        var panel = new GameObject().AddComponent<RectTransform>();
        var container = new GameObject().AddComponent<RectTransform>();
        var indicator = new GameObject().AddComponent<RectTransform>();
        var prefab = new GameObject();
        prefab.AddComponent<RectTransform>();
        prefab.AddComponent<Image>();

        typeof(MinimapView).GetField("minimapPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mockView, panel);
        typeof(MinimapView).GetField("gridContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mockView, container);
        typeof(MinimapView).GetField("playerIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mockView, indicator);
        typeof(MinimapView).GetField("roomMinimapPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mockView, prefab);

        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";

        _controller = _controllerObject.AddComponent<MinimapController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controllerObject);
        Object.Destroy(_playerObject);
    }

    [UnityTest]
    public IEnumerator EvaluateCurrentSceneMap_WhenHubScene_ShouldSetupHubMap()
    {
        _controller.SendMessage("Awake");
        yield return null;

        _controller.SendMessage("EvaluateCurrentSceneMap", "HubRoom");

        var mapData = (MinimapData)typeof(MinimapController)
            .GetField("mapData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_controller);

        Assert.AreEqual(5, mapData.RoomTypes.Count);
    }

    [UnityTest]
    public IEnumerator UpdatePlayerMovement_WhenPlayerMovesRoom_ShouldMarkAsExplored()
    {
        _controller.SendMessage("Awake");
        yield return null;
        _controller.SendMessage("EvaluateCurrentSceneMap", "HubRoom");

        _playerObject.transform.position = new Vector3(30f, 0f, 0f);
        _controller.SendMessage("UpdatePlayerMovement");

        var mapData = (MinimapData)typeof(MinimapController)
            .GetField("mapData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_controller);

        Assert.IsTrue(mapData.IsExplored(new Vector2Int(1, 0)));
    }
}