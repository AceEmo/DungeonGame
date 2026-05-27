using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MinimapInputHandlerTests
{
    private MockInputProvider _mockInput;
    private MinimapInputHandler _inputHandler;

    [SetUp]
    public void SetUp()
    {
        _mockInput = new MockInputProvider();
        _inputHandler = new MinimapInputHandler(_mockInput);
    }

    [Test]
    public void ShouldToggleMap_WhenButtonNotPressed_ShouldReturnFalse()
    {
        _mockInput.IsPressed = false;

        bool result = _inputHandler.ShouldToggleMap();

        Assert.IsFalse(result);
        Assert.IsFalse(_inputHandler.IsLargeMapOpen);
    }

    [Test]
    public void ShouldToggleMap_WhenButtonPressed_ShouldToggleStateAndReturnTrue()
    {
        _mockInput.IsPressed = true;

        bool result = _inputHandler.ShouldToggleMap();

        Assert.IsTrue(result);
        Assert.IsTrue(_inputHandler.IsLargeMapOpen);
    }

    private class MockInputProvider : IInputProvider
    {
        public bool IsPressed;

        public float GetAxisRaw(string axisName) => 0f;

        public bool GetButtonDown(string buttonName) => IsPressed;

        public bool GetButton(string buttonName) => IsPressed;
    }
}