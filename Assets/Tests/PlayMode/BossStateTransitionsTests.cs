using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossStateTransitionsTests
{
    private GameObject _bossObject;
    private GameObject _playerObject;
    private BossContext _context;
    private BossBrain _brain;

    [SetUp]
    public void SetUp()
    {
        _bossObject = new GameObject("Boss");
        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";

        _context = new BossContext
        {
            BossTransform = _bossObject.transform,
            Player = _playerObject.transform,
            Data = ScriptableObject.CreateInstance<BossData>(),
            Health = new BossHealth(100),
            Movement = new BossMovement(_bossObject.AddComponent<Rigidbody2D>()),
            Rage = new BossRage(),
            Animator = _bossObject.AddComponent<Animator>()
        };

        _context.Data.attackRange = 2f;
        _context.Data.dashTriggerDistance = 5f;
        _context.Data.dashChance = 0f;
        _context.Data.dashCooldown = 2f;
        _context.Data.steeringSmooth = 0.15f;

        _brain = new BossBrain(_context);
        _context.Brain = _brain;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_bossObject);
        Object.DestroyImmediate(_playerObject);
    }

    [UnityTest]
    public IEnumerator IdleState_ShouldTransitionToChaseStateOnUpdate()
    {
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(".*Animator.*"));

        _brain.Start();
        _brain.Update();
        yield return null;

        _playerObject.transform.position = new Vector3(10f, 10f, 0f);
        _brain.Update();
        
        Assert.Greater(_context.LastMoveDirection.x, 0f);
        Assert.Greater(_context.LastMoveDirection.y, 0f);
    }
}