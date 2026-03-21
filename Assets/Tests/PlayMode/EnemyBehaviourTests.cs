using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyBehaviourTests
{
    private GameObject enemyObject;
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        enemyObject = new GameObject("TestEnemy");
        
        playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        playerObject.transform.position = new Vector3(5f, 0f, 0f);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(playerObject);
    }

    [UnityTest]
    public IEnumerator ChaseBehaviourReturnsDirectionTowardsPlayer()
    {
        ChaseBehaviour chaseBehaviour = enemyObject.AddComponent<ChaseBehaviour>();

        yield return null;

        Vector2 direction = chaseBehaviour.GetDirection();

        Assert.AreEqual(new Vector2(1f, 0f), direction);
    }
}