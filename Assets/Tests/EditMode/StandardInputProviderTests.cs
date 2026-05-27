using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class StandardInputProviderTests
{
    private GameObject _inputObject;
    private StandardInputProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _inputObject = new GameObject();
        _provider = _inputObject.AddComponent<StandardInputProvider>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_inputObject);
    }

    [Test]
    public void GetAxisRaw_WithUnsupportedAxis_ShouldReturnZero()
    {
        float result = _provider.GetAxisRaw("InvalidAxis");

        Assert.AreEqual(0f, result);
    }

    [Test]
    public void GetButtonDown_WithUnsupportedButton_ShouldReturnFalse()
    {
        bool result = _provider.GetButtonDown("InvalidButton");

        Assert.IsFalse(result);
    }
}