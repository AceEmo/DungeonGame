using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InteractablesTests
{
    private GameObject _chestObject;
    private GameObject _terminalObject;
    private GameObject _gearObject;
    private GameObject _healthBoxObject;
    private GameObject _upgradeMachineObject;

    [SetUp]
    public void SetUp()
    {
        _chestObject = new GameObject("Chest");
        _terminalObject = new GameObject("Terminal");
        _gearObject = new GameObject("Gear");
        _healthBoxObject = new GameObject("HealthBox");
        _upgradeMachineObject = new GameObject("UpgradeMachine");
    }

    [TearDown]
    public void TearDown()
    {
        if (_chestObject != null) Object.Destroy(_chestObject);
        if (_terminalObject != null) Object.Destroy(_terminalObject);
        if (_gearObject != null) Object.Destroy(_gearObject);
        if (_healthBoxObject != null) Object.Destroy(_healthBoxObject);
        if (_upgradeMachineObject != null) Object.Destroy(_upgradeMachineObject);
    }

    [UnityTest]
    public IEnumerator ChestInteract_GetHintText_ShouldReturnCorrectString()
    {
        var chest = _chestObject.AddComponent<ChestInteract>();

        string hint = chest.GetHintText();

        Assert.AreEqual("[E] Open", hint);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ChestInteract_Interact_ShouldSpawnPrefabsAndDestroyItself()
    {
        var chest = _chestObject.AddComponent<ChestInteract>();
        chest.OpenChestPrefab = new GameObject("OpenChestPrefab");
        chest.HealthPrefab = new GameObject("HealthPrefab");
        chest.Gear1Prefab = new GameObject("Gear1Prefab");
        chest.Gear2Prefab = new GameObject("Gear2Prefab");

        chest.Interact();
        yield return null;

        Assert.IsTrue(_chestObject == null);
        Object.Destroy(chest.OpenChestPrefab);
        Object.Destroy(chest.HealthPrefab);
        Object.Destroy(chest.Gear1Prefab);
        Object.Destroy(chest.Gear2Prefab);
    }

    [UnityTest]
    public IEnumerator GameInfoTerminal_GetHintText_ShouldReturnCorrectString()
    {
        var terminal = _terminalObject.AddComponent<GameInfoTerminal>();

        string hint = terminal.GetHintText();

        Assert.AreEqual("[E] Examine Terminal", hint);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GearInteract_GetHintText_ShouldReturnCorrectString()
    {
        var gear = _gearObject.AddComponent<GearInteract>();

        string hint = gear.GetHintText();

        Assert.AreEqual("[E] Collect", hint);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GearInteract_Interact_ShouldAddScrapAndDestroyItself()
    {
        var gear = _gearObject.AddComponent<GearInteract>();
        var statsObject = new GameObject("PlayerStats");
        var playerStats = statsObject.AddComponent<PlayerStats>();
        gear.scrapAmount = 5;
        gear.GetType().GetField("playerStats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gear, playerStats);

        gear.Interact();
        yield return null;

        Assert.AreEqual(5, playerStats.ScrapAmount);
        Assert.IsTrue(_gearObject == null);
        Object.Destroy(statsObject);
    }

    [UnityTest]
    public IEnumerator HealthBoxInteract_GetHintText_ShouldReturnCorrectString()
    {
        var healthBox = _healthBoxObject.AddComponent<HealthBoxInteract>();

        string hint = healthBox.GetHintText();

        Assert.AreEqual("[E] Heal", hint);
        yield return null;
    }

    [UnityTest]
    public IEnumerator HealthBoxInteract_Interact_WhenPlayerNeedsHealing_ShouldHealAndDestroyItself()
    {
        var healthBox = _healthBoxObject.AddComponent<HealthBoxInteract>();
        var playerObject = new GameObject("Player");
        var mockHealth = playerObject.AddComponent<MockPlayerHealth>();
        mockHealth.CurrentHealth = 5f;
        mockHealth.MaxHealth = 10f;

        healthBox.Interact();
        yield return null;

        Assert.IsTrue(mockHealth.HealCalled);
        Assert.IsTrue(_healthBoxObject == null);
        Object.Destroy(playerObject);
    }

    [UnityTest]
    public IEnumerator HealthBoxInteract_Interact_WhenPlayerAtMaxHealth_ShouldNotHealOrDestroyItself()
    {
        var healthBox = _healthBoxObject.AddComponent<HealthBoxInteract>();
        var playerObject = new GameObject("Player");
        var mockHealth = playerObject.AddComponent<MockPlayerHealth>();
        mockHealth.CurrentHealth = 10f;
        mockHealth.MaxHealth = 10f;

        healthBox.Interact();
        yield return null;

        Assert.IsFalse(mockHealth.HealCalled);
        Assert.IsNotNull(_healthBoxObject);
        Object.Destroy(playerObject);
    }

    [UnityTest]
    public IEnumerator UpgradeMachineInteract_GetHintText_ShouldReturnCorrectString()
    {
        var upgradeMachine = _upgradeMachineObject.AddComponent<UpgradeMachineInteract>();

        string hint = upgradeMachine.GetHintText();

        Assert.AreEqual("[E] Upgrade Station", hint);
        yield return null;
    }

    private class MockPlayerHealth : MonoBehaviour
    {
        public float CurrentHealth;
        public float MaxHealth;
        public bool HealCalled { get; private set; }

        public void Heal(float amount)
        {
            HealCalled = true;
        }
    }

    private class PlayerStats : MonoBehaviour
    {
        public int ScrapAmount { get; private set; }

        public void AddScrap(int amount)
        {
            ScrapAmount += amount;
        }
    }
}