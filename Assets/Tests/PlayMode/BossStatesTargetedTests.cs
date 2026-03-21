using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BossStatesTargetedTests
{
    private BossContext context;
    private GameObject bossObj;
    private GameObject playerObj;

    [SetUp]
    public void Setup()
    {
        bossObj = new GameObject("Boss");
        playerObj = new GameObject("Player");
        playerObj.SetActive(false);
        playerObj.tag = "Player";
        playerObj.transform.position = new Vector3(2f, 0f, 0f);

        BossData data = ScriptableObject.CreateInstance<BossData>();
        data.attackDamage = 5;
        data.attackRadius = 1f;
        data.dashDuration = 0.5f;
        data.dashSpeed = 10f;

        context = new BossContext
        {
            BossTransform = bossObj.transform,
            Player = playerObj.transform,
            Data = data,
            Animator = bossObj.AddComponent<Animator>(),
            Movement = new BossMovement(bossObj.AddComponent<Rigidbody2D>()),
            Brain = new BossBrain(null),
            Health = new BossHealth(100),
            Combat = new BossCombat(
                new GameObject("Up").transform,
                new GameObject("Down").transform,
                new GameObject("Left").transform,
                new GameObject("Right").transform
            )
        };
        context.Brain = new BossBrain(context);
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(bossObj);
        UnityEngine.Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void AttackStateDealsDamageAndChangesState()
    {
        AttackState attackState = new AttackState();
        
        playerObj.AddComponent<BoxCollider2D>();
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<Animator>();
        
        PlayerHealth health = playerObj.AddComponent<PlayerHealth>();
        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.startHealth = 10f;
        stats.maxHealth = 10f;
        
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(health, stats);
        
        playerObj.SetActive(true);

        health.TakeDamage(0f, Vector2.zero); 
        
        context.LastMoveDirection = Vector2.right;
        context.Combat = new BossCombat(bossObj.transform, bossObj.transform, bossObj.transform, bossObj.transform);

        attackState.EnterState(context);
        attackState.UpdateState(context);

        Assert.AreEqual(Vector2.zero, bossObj.GetComponent<Rigidbody2D>().linearVelocity);
    }

    [Test]
    public void DashStateAppliesVelocityAndRespectsDuration()
    {
        playerObj.SetActive(true);
        DashState dashState = new DashState();
        
        dashState.EnterState(context);
        dashState.UpdateState(context);

        Rigidbody2D rb = bossObj.GetComponent<Rigidbody2D>();
        Assert.AreEqual(new Vector2(10f, 0f), rb.linearVelocity);
    }
}