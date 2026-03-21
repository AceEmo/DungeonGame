using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MockBehaviour : MonoBehaviour, IEnemyBehaviour 
{ 
    public Vector2 directionToReturn;
    public Vector2 GetDirection() => directionToReturn; 
}

public class MockMovement : MonoBehaviour, IEnemyMovement 
{ 
    public Vector2 lastMoveDirection;
    public void Move(Vector2 direction) => lastMoveDirection = direction; 
}

public class EnemyTests
{
    private GameObject enemyObj;
    private Enemy enemy;
    private MockBehaviour mockBehaviour;
    private MockMovement mockMovement;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        enemyObj = new GameObject("Enemy");
        enemyObj.SetActive(false);

        rb = enemyObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        
        mockBehaviour = enemyObj.AddComponent<MockBehaviour>();
        mockMovement = enemyObj.AddComponent<MockMovement>();
        
        enemyObj.AddComponent<EnemyHealth>();
        enemy = enemyObj.AddComponent<Enemy>();
        
        EnemyData testData = ScriptableObject.CreateInstance<EnemyData>();
        testData.speed = 5f;
        enemy.data = testData;

        enemyObj.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        if (enemyObj != null) Object.DestroyImmediate(enemyObj);
        
        ResetStaticInstance<GameManager>();
        ResetStaticInstance<InteractionUI>();
    }

    private void ResetStaticInstance<T>()
    {
        FieldInfo instanceField = typeof(T).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) 
                               ?? typeof(T).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (instanceField != null) instanceField.SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator FixedUpdate_CallsMovementWithBehaviourDirection()
    {
        Vector2 expectedDir = new Vector2(1f, 0.5f);
        mockBehaviour.directionToReturn = expectedDir;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(expectedDir, mockMovement.lastMoveDirection);
    }

    [Test]
    public void Enemy_Data_PropertyReturnsCorrectData()
    {
        Assert.AreEqual(enemy.data, enemy.Data);
    }
}