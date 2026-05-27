using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class TerminalUIControllerTests
{
    private GameObject _controllerObject;
    private TerminalUIController _controller;
    private TextMeshProUGUI _textMesh;

    [SetUp]
    public void SetUp()
    {
        _controllerObject = new GameObject();
        _controller = _controllerObject.AddComponent<TerminalUIController>();
        _textMesh = new GameObject().AddComponent<TextMeshProUGUI>();

        typeof(TerminalUIController).GetField("contentText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_controller, _textMesh);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controllerObject);
        Object.Destroy(_textMesh.gameObject);
    }

    [UnityTest]
    public IEnumerator ShowMission_ShouldChangeContentText()
    {
        _controller.ShowMission();
        yield return null;

        Assert.IsTrue(_textMesh.text.Contains("MISSION PARAMETERS"));
    }

    [UnityTest]
    public IEnumerator ShowManuals_ShouldChangeContentText()
    {
        _controller.ShowManuals();
        yield return null;

        Assert.IsTrue(_textMesh.text.Contains("ENGINEERING MANUAL"));
    }

    [UnityTest]
    public IEnumerator ShowHome_ShouldRevertToHomeLogText()
    {
        _controller.ShowMission();
        _controller.ShowHome();
        yield return null;

        Assert.IsTrue(_textMesh.text.Contains("SYSTEM LOG"));
    }
}