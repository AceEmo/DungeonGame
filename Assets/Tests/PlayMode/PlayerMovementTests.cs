using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MockInputProvider : MonoBehaviour, IInputProvider
{
    public float horizontalMove;
    public float verticalMove;
    public float horizontalShoot;
    public float verticalShoot;

    public float GetAxisRaw(string axisName)
    {
        switch (axisName)
        {
            case "Horizontal": return horizontalMove;
            case "Vertical": return verticalMove;
            case "HorizontalArrows": return horizontalShoot;
            case "VerticalArrows": return verticalShoot;
            default: return 0f;
        }
    }
}

public class PlayerMovementTests
{
    private GameObject playerObj;
    private PlayerMovement playerMovement;
    private MockInputProvider mockInput;
    private Rigidbody2D rb;
    private Animator animator;

    [SetUp]
    public void Setup()
    {
        playerObj = new GameObject("Player");
        playerObj.SetActive(false);

        mockInput = playerObj.AddComponent<MockInputProvider>();
        rb = playerObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        animator = playerObj.AddComponent<Animator>();

        playerMovement = playerObj.AddComponent<PlayerMovement>();

        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.moveSpeed = 5f;

        FieldInfo statsField = typeof(PlayerMovement).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerMovement, stats);

        FieldInfo rbField = typeof(PlayerMovement).GetField("rb", BindingFlags.NonPublic | BindingFlags.Instance);
        rbField.SetValue(playerMovement, rb);

        FieldInfo animField = typeof(PlayerMovement).GetField("animator", BindingFlags.NonPublic | BindingFlags.Instance);
        animField.SetValue(playerMovement, animator);

        FieldInfo inputField = typeof(PlayerMovement).GetField("inputProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        inputField.SetValue(playerMovement, mockInput);

        playerObj.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(playerObj);
    }

    [UnityTest]
    public IEnumerator UpdateSetsMovementAndAnimatorSpeed()
    {
        mockInput.horizontalMove = 1f;
        mockInput.verticalMove = 0f;

        yield return new WaitForSeconds(0.1f);

        FieldInfo moveField = typeof(PlayerMovement).GetField("movement", BindingFlags.NonPublic | BindingFlags.Instance);
        Vector2 currentMovement = (Vector2)moveField.GetValue(playerMovement);

        Assert.AreEqual(new Vector2(1f, 0f), currentMovement);
    }

    [UnityTest]
    public IEnumerator UpdatePrioritizesShootingInputForLookDirection()
    {
        mockInput.horizontalMove = 1f; 
        mockInput.horizontalShoot = 0f;
        mockInput.verticalShoot = 1f; 

        yield return new WaitForSeconds(0.1f);

        FieldInfo lookField = typeof(PlayerMovement).GetField("lookDirection", BindingFlags.NonPublic | BindingFlags.Instance);
        Vector2 lookDir = (Vector2)lookField.GetValue(playerMovement);

        Assert.AreEqual(new Vector2(0f, 1f), lookDir);
    }

    [UnityTest]
    public IEnumerator FixedUpdateMovesRigidbodyBasedOnStats()
    {
        mockInput.horizontalMove = 1f;
        Vector2 initialPosition = rb.position;

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        Assert.Greater(rb.position.x, initialPosition.x);
    }
}