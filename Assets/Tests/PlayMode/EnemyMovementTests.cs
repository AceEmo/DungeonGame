using NUnit.Framework;
using UnityEngine;

public class EnemyMovementTests
{
    private GameObject enemyObject;
    private EnemyData enemyData;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        enemyObject = new GameObject("TestEnemy");
        Enemy enemyComponent = enemyObject.AddComponent<Enemy>();
        rb = enemyObject.AddComponent<Rigidbody2D>();

        enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.speed = 4f;
        enemyComponent.data = enemyData;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void FlyingMovementAppliesVelocityDirectly()
    {
        FlyingMovement flyingMovement = enemyObject.AddComponent<FlyingMovement>();
        Vector2 moveDirection = new Vector2(1f, 0f);

        flyingMovement.Move(moveDirection);

        Assert.AreEqual(new Vector2(4f, 0f), rb.linearVelocity);
    }

    [Test]
    public void GroundMovementStopsWhenDirectionIsZero()
    {
        GroundMovement groundMovement = enemyObject.AddComponent<GroundMovement>();
        
        groundMovement.Move(Vector2.zero);

        Assert.AreEqual(Vector2.zero, rb.linearVelocity);
    }
}