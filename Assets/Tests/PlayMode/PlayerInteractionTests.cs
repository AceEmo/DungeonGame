using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerInteractionTests
{
    private GameObject _playerObject;
    private GameObject _interactableObject;
    private PlayerInteraction _interaction;

    [SetUp]
    public void SetUp()
    {
        _playerObject = new GameObject("Player");
        _interaction = _playerObject.AddComponent<PlayerInteraction>();

        _interactableObject = new GameObject("Interactable");
        _interactableObject.AddComponent<BoxCollider2D>();
        _interactableObject.AddComponent<MockInteractable>();

        GameObject uiObject = new GameObject("InteractionUI");
        uiObject.AddComponent<InteractionUI>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_playerObject);
        Object.DestroyImmediate(_interactableObject);
        
        if (InteractionUI.Instance != null)
        {
            Object.DestroyImmediate(InteractionUI.Instance.gameObject);
        }
        
        typeof(InteractionUI).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator FindClosestInteractable_WhenTargetInRange_ShouldSetCurrentTarget()
    {
        _playerObject.transform.position = Vector3.zero;
        _interactableObject.transform.position = new Vector3(0.5f, 0f, 0f);
        _interaction.SendMessage("Start");
        yield return null;

        _interaction.SendMessage("FindClosestInteractable");

        var currentTarget = typeof(PlayerInteraction)
            .GetField("currentTarget", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_interaction);

        Assert.IsNotNull(currentTarget);
    }

    private class MockInteractable : MonoBehaviour, IInteractable
    {
        public string GetHintText() => "[E] Test";
        public void Interact() {}
    }
}