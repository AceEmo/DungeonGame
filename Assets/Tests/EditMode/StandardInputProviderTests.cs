using NUnit.Framework;
using UnityEngine;

public class StandardInputProviderTests
{
    [Test]
    public void GetAxisRawReturnsZeroWhenNoInput()
    {
        GameObject providerObj = new GameObject("InputProvider");
        StandardInputProvider provider = providerObj.AddComponent<StandardInputProvider>();
        
        float result = provider.GetAxisRaw("Horizontal");
        Assert.AreEqual(0f, result);
        
        Object.DestroyImmediate(providerObj);
    }
}