using NUnit.Framework;
using UnityEngine;

public class StandardInputProviderTests
{
    private GameObject inputObj;
    private StandardInputProvider provider;

    [SetUp]
    public void Setup()
    {
        inputObj = new GameObject("InputProvider");
        provider = inputObj.AddComponent<StandardInputProvider>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(inputObj);
    }

    [Test]
    public void GetAxisRaw_ReturnsZero_WhenNoInputIsPresent()
    {
        float horizontal = provider.GetAxisRaw("Horizontal");
        float vertical = provider.GetAxisRaw("Vertical");

        Assert.AreEqual(0f, horizontal, "Horizontal input should be 0 in test environment.");
        Assert.AreEqual(0f, vertical, "Vertical input should be 0 in test environment.");
    }

    [Test]
    public void Provider_ImplementsIInputProvider()
    {
        Assert.IsTrue(provider is IInputProvider, "StandardInputProvider must implement IInputProvider.");
    }
}