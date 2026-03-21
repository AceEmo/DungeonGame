using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class GroundMovementTests
{
    private GameObject enemyObj;
    private GroundMovement movement;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        enemyObj = new GameObject("MovingEnemy");
        enemyObj.SetActive(false);
        
        rb = enemyObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        enemyObj.AddComponent<BoxCollider2D>();
        
        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.data = ScriptableObject.CreateInstance<EnemyData>();
        enemy.data.speed = 5f;
        enemy.enabled = false; 

        movement = enemyObj.AddComponent<GroundMovement>();

        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        FieldInfo wallLayerField = typeof(GroundMovement).GetField("wallLayer", flags);
        wallLayerField.SetValue(movement, (LayerMask)(1 << 4));

        FieldInfo enemyLayerField = typeof(GroundMovement).GetField("enemyLayer", flags);
        enemyLayerField.SetValue(movement, (LayerMask)(1 << 2));

        enemyObj.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        if (enemyObj != null)
        {
            Object.DestroyImmediate(enemyObj);
        }
    }

    [Test]
    public void SteeringIsSmoothedOverTime()
    {
        movement.Move(Vector2.right);
        Vector2 firstVel = rb.linearVelocity;

        movement.Move(Vector2.right);
        Vector2 secondVel = rb.linearVelocity;

        Assert.Greater(secondVel.magnitude, firstVel.magnitude);
    }
}