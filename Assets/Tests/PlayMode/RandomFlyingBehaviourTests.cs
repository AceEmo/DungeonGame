using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RandomFlyingBehaviourTests
{
    private GameObject enemyObj;
    private Enemy enemy;
    private RandomFlyingBehaviour flyingBehaviour;
    private GameObject wallObj;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        enemyObj = new GameObject("FlyingEnemy");
        enemyObj.SetActive(false);
        enemy = enemyObj.AddComponent<Enemy>();
        enemy.data = ScriptableObject.CreateInstance<EnemyData>();
        enemy.data.changeDirectionInterval = 2f;
        enemy.enabled = false; 
        
        Rigidbody2D rb = enemyObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        enemyObj.AddComponent<BoxCollider2D>();

        flyingBehaviour = enemyObj.AddComponent<RandomFlyingBehaviour>();
        enemyObj.SetActive(true);

        wallObj = new GameObject("Wall");
        wallObj.tag = "Wall";
        wallObj.transform.position = new Vector3(0.5f, 0f, 0f); 
        BoxCollider2D wallCol = wallObj.AddComponent<BoxCollider2D>();
        wallCol.size = new Vector2(1f, 10f);
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(enemyObj);
        UnityEngine.Object.DestroyImmediate(wallObj);
    }

    [UnityTest]
    public IEnumerator StartPicksInitialDirection()
    {
        yield return null;

        FieldInfo dirField = typeof(RandomFlyingBehaviour).GetField("currentDirection", BindingFlags.NonPublic | BindingFlags.Instance);
        Vector2 dir = (Vector2)dirField.GetValue(flyingBehaviour);

        Assert.AreNotEqual(Vector2.zero, dir);
        Assert.IsTrue(dir.magnitude > 0.9f && dir.magnitude < 1.1f); 
    }

    [UnityTest]
    public IEnumerator GetDirectionChangesDirectionAfterTimerExceedsInterval()
    {
        yield return null;

        FieldInfo dirField = typeof(RandomFlyingBehaviour).GetField("currentDirection", BindingFlags.NonPublic | BindingFlags.Instance);
        Vector2 initialDir = (Vector2)dirField.GetValue(flyingBehaviour);

        FieldInfo timerField = typeof(RandomFlyingBehaviour).GetField("timer", BindingFlags.NonPublic | BindingFlags.Instance);
        timerField.SetValue(flyingBehaviour, 5f); 

        Vector2 newDir = flyingBehaviour.GetDirection();

        Assert.AreNotEqual(initialDir, newDir);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnCollisionEnter2DWithWallReflectsDirection()
    {
        FieldInfo dirField = typeof(RandomFlyingBehaviour).GetField("currentDirection", BindingFlags.NonPublic | BindingFlags.Instance);
        dirField.SetValue(flyingBehaviour, Vector2.right);

        enemyObj.transform.position = Vector3.zero;
        wallObj.transform.position = new Vector3(3f, 0f, 0f); 

        enemyObj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * 15f;

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        Vector2 directionAfterCollision = (Vector2)dirField.GetValue(flyingBehaviour);
        
        Assert.Less(directionAfterCollision.x, 0f);
    }
}