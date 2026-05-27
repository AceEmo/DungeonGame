using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MinimapCoordinateCalculatorTests
{
    private MinimapCoordinateCalculator _calculator;
    private readonly Vector2Int _invalidPos = new Vector2Int(-999, -999);

    [SetUp]
    public void SetUp()
    {
        _calculator = new MinimapCoordinateCalculator(30f, _invalidPos);
    }

    [Test]
    public void GetPlayerGridPosition_WhenTransformIsNull_ShouldReturnInvalidPosition()
    {
        Vector2Int result = _calculator.GetPlayerGridPosition(null);

        Assert.AreEqual(_invalidPos, result);
    }

    [Test]
    public void GetPlayerGridPosition_WhenTransformIsValid_ShouldReturnRoundedGridCoordinates()
    {
        GameObject playerObj = new GameObject();
        playerObj.transform.position = new Vector3(31f, -58f, 0f);

        Vector2Int result = _calculator.GetPlayerGridPosition(playerObj.transform);

        Assert.AreEqual(new Vector2Int(1, -2), result);
        Object.DestroyImmediate(playerObj);
    }
}