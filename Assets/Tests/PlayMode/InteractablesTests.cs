using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InteractablesTests
{
    private GameObject testEnvironment;

    [SetUp]
    public void Setup()
    {
        testEnvironment = new GameObject("TestEnvironment");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testEnvironment);
    }

    [Test]
    public void ChestInteractGetHintTextReturnsCorrectValue()
    {
        GameObject chestObject = new GameObject();
        ChestInteract chestInteract = chestObject.AddComponent<ChestInteract>();

        Assert.AreEqual("[E] Open", chestInteract.GetHintText());

        Object.DestroyImmediate(chestObject);
    }

    [UnityTest]
    public IEnumerator ChestInteractInstantiatesPrefabAndDestroysItself()
    {
        GameObject chestObject = new GameObject();
        ChestInteract chestInteract = chestObject.AddComponent<ChestInteract>();
        
        GameObject mockPrefab = new GameObject("MockPrefab");
        chestInteract.OpenChestPrefab = mockPrefab;
        chestInteract.HealthPrefab = mockPrefab;
        chestInteract.Gear1Prefab = mockPrefab;
        chestInteract.Gear2Prefab = mockPrefab;

        chestInteract.Interact();

        yield return null;

        Assert.IsTrue(chestObject == null);
        Object.DestroyImmediate(mockPrefab);
    }

    [Test]
    public void GearInteractGetHintTextReturnsCorrectValue()
    {
        GameObject gearObject = new GameObject();
        GearInteract gearInteract = gearObject.AddComponent<GearInteract>();

        Assert.AreEqual("[E] Collect", gearInteract.GetHintText());

        Object.DestroyImmediate(gearObject);
    }

    [UnityTest]
    public IEnumerator GearInteractDestroysItselfOnInteract()
    {
        GameObject gearObject = new GameObject();
        GearInteract gearInteract = gearObject.AddComponent<GearInteract>();
        
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManagerObject.AddComponent<GameManager>(); 

        gearInteract.Interact();

        yield return null;

        Assert.IsTrue(gearObject == null);
        Object.DestroyImmediate(gameManagerObject);
    }

    [Test]
    public void HealthBoxInteractGetHintTextReturnsCorrectValue()
    {
        GameObject healthBoxObject = new GameObject();
        HealthBoxInteract healthBoxInteract = healthBoxObject.AddComponent<HealthBoxInteract>();

        Assert.AreEqual("[E] Heal", healthBoxInteract.GetHintText());

        Object.DestroyImmediate(healthBoxObject);
    }

    [UnityTest]
    public IEnumerator HealthBoxInteractDoesNotDestroyWhenPlayerHealthIsMax()
    {
        GameObject healthBoxObject = new GameObject();
        HealthBoxInteract healthBoxInteract = healthBoxObject.AddComponent<HealthBoxInteract>();

        GameObject playerObject = new GameObject();
        playerObject.SetActive(false);

        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<BoxCollider2D>();
        
        PlayerMovement playerMovement = playerObject.AddComponent<PlayerMovement>();
        playerMovement.enabled = false;

        PlayerHealth playerHealth = playerObject.AddComponent<PlayerHealth>();
        
        PlayerStats mockStats = ScriptableObject.CreateInstance<PlayerStats>();
        mockStats.startHealth = 10f;
        mockStats.maxHealth = 10f;
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerHealth, mockStats);
        
        playerObject.SetActive(true);
        
        healthBoxInteract.Interact();

        yield return null;

        Assert.IsFalse(healthBoxObject == null);

        Object.DestroyImmediate(healthBoxObject);
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(mockStats);
    }

    [UnityTest]
    public IEnumerator HealthBoxInteractHealsAndDestroysItselfWhenPlayerNeedsHealth()
    {
        GameObject healthBoxObject = new GameObject();
        HealthBoxInteract healthBoxInteract = healthBoxObject.AddComponent<HealthBoxInteract>();

        GameObject playerObject = new GameObject();
        playerObject.SetActive(false);

        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<BoxCollider2D>();
        
        PlayerMovement playerMovement = playerObject.AddComponent<PlayerMovement>();
        playerMovement.enabled = false;

        PlayerHealth playerHealth = playerObject.AddComponent<PlayerHealth>();
        
        PlayerStats mockStats = ScriptableObject.CreateInstance<PlayerStats>();
        mockStats.startHealth = 5f;
        mockStats.maxHealth = 10f;
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerHealth, mockStats);
        
        playerObject.SetActive(true);

        healthBoxInteract.Interact();

        yield return null;

        Assert.IsTrue(healthBoxObject == null);
        Assert.IsTrue(playerHealth.CurrentHealth > 5f);

        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(mockStats);
    }
}