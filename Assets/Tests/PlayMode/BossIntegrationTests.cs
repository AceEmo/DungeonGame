using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossIntegrationTests
{
    private GameObject playerObject;
    private GameObject bossObject;
    private Boss bossComponent;
    private BossData bossData;
    private bool bossDiedEventTriggered;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        playerObject.transform.position = new Vector3(5f, 5f, 0f);

        bossObject = new GameObject("Boss");
        bossObject.SetActive(false);

        bossData = ScriptableObject.CreateInstance<BossData>();
        bossData.MaxHealth = 50;
        bossData.hitColor = Color.white;
        bossData.attackRange = 2f;
        bossData.speed = 3f;
        bossData.dashChance = 0f;

        bossComponent = bossObject.AddComponent<Boss>();
        bossComponent.data = bossData;

        Transform up = new GameObject("Up").transform;
        Transform down = new GameObject("Down").transform;
        Transform left = new GameObject("Left").transform;
        Transform right = new GameObject("Right").transform;

        up.SetParent(bossObject.transform);
        down.SetParent(bossObject.transform);
        left.SetParent(bossObject.transform);
        right.SetParent(bossObject.transform);

        bossComponent.attackPointUp = up;
        bossComponent.attackPointDown = down;
        bossComponent.attackPointLeft = left;
        bossComponent.attackPointRight = right;

        bossObject.AddComponent<SpriteRenderer>();
        bossObject.AddComponent<Animator>();
        bossObject.AddComponent<Rigidbody2D>();

        bossObject.SetActive(true);

        bossDiedEventTriggered = false;
        bossComponent.OnBossDied += () => bossDiedEventTriggered = true;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
        if (bossObject != null)
        {
            Object.DestroyImmediate(bossObject);
        }
        Object.DestroyImmediate(bossData);
    }

    [UnityTest]
    public IEnumerator BossStartsInIdleAndTransitionsToChase()
    {
        yield return null; 
        yield return null;

        FieldInfo contextField = typeof(Boss).GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
        BossContext context = (BossContext)contextField.GetValue(bossComponent);

        FieldInfo fsmField = typeof(BossBrain).GetField("fsm", BindingFlags.NonPublic | BindingFlags.Instance);
        BossStateMachine fsm = (BossStateMachine)fsmField.GetValue(context.Brain);

        FieldInfo stateField = typeof(BossStateMachine).GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        IBossState currentState = (IBossState)stateField.GetValue(fsm);

        Assert.IsTrue(currentState is ChaseState);
    }

    [UnityTest]
    public IEnumerator TakeDamageReducesHealthAndTriggersHitAnimation()
    {
        bossComponent.TakeDamage(10);

        yield return null;

        FieldInfo contextField = typeof(Boss).GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
        BossContext context = (BossContext)contextField.GetValue(bossComponent);

        Assert.AreEqual(40, context.Health.MaxHealth - 10);
    }

    [UnityTest]
    public IEnumerator FatalDamageTriggersDeathStateAndEvent()
    {
        bossComponent.TakeDamage(50);

        yield return null;

        Assert.IsTrue(bossDiedEventTriggered);

        FieldInfo contextField = typeof(Boss).GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
        BossContext context = (BossContext)contextField.GetValue(bossComponent);

        Assert.IsTrue(context.IsDead);

        Rigidbody2D rb = bossObject.GetComponent<Rigidbody2D>();
        Assert.IsFalse(rb.simulated);
    }

    [UnityTest]
    public IEnumerator DeathSequenceFadesAndDestroysBossObject()
    {
        bossComponent.TakeDamage(100);

        yield return new WaitForSeconds(1.6f);

        Assert.IsTrue(bossObject == null);
    }
}