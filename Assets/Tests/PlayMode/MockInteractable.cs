using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class MockInteractable : MonoBehaviour, IInteractable
{
    public bool WasInteracted { get; private set; }

    public string GetHintText()
    {
        return "[E] Test Interact";
    }

    public void Interact()
    {
        WasInteracted = true;
    }
}

public class PlayerInteractionTests
{
    private GameObject playerObj;
    private PlayerInteraction playerInteraction;
    private GameObject interactableObj;
    private GameObject uiObj;
    private GameObject gmObj;

    [SetUp]
    public void Setup()
    {
        gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();

        uiObj = new GameObject("InteractionUI");
        InteractionUI ui = uiObj.AddComponent<InteractionUI>();

        FieldInfo panelField = typeof(InteractionUI).GetField("interactionPanel", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (panelField != null) panelField.SetValue(ui, new GameObject("Panel"));

        FieldInfo textField = typeof(InteractionUI).GetField("hintText", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (textField != null) textField.SetValue(ui, new GameObject("Text").AddComponent<TextMeshProUGUI>());

        playerObj = new GameObject("Player");
        playerObj.SetActive(false);
        playerInteraction = playerObj.AddComponent<PlayerInteraction>();

        FieldInfo layerField = typeof(PlayerInteraction).GetField("interactableLayer", BindingFlags.NonPublic | BindingFlags.Instance);
        LayerMask interactableMask = 1 << 4; 
        layerField.SetValue(playerInteraction, interactableMask);

        FieldInfo rangeField = typeof(PlayerInteraction).GetField("interactRange", BindingFlags.NonPublic | BindingFlags.Instance);
        rangeField.SetValue(playerInteraction, 2f);

        interactableObj = new GameObject("InteractableItem");
        interactableObj.layer = 4; 
        interactableObj.AddComponent<BoxCollider2D>();
        interactableObj.AddComponent<MockInteractable>();
        interactableObj.transform.position = new Vector3(1f, 0f, 0f); 

        playerObj.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(interactableObj);
        Object.DestroyImmediate(uiObj);
        Object.DestroyImmediate(gmObj);

        typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(InteractionUI).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator FindClosestInteractableSetsCurrentTargetWhenInRange()
    {
        yield return null;

        MethodInfo findMethod = typeof(PlayerInteraction).GetMethod("FindClosestInteractable", BindingFlags.NonPublic | BindingFlags.Instance);
        findMethod.Invoke(playerInteraction, null);

        FieldInfo targetField = typeof(PlayerInteraction).GetField("currentTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        IInteractable target = (IInteractable)targetField.GetValue(playerInteraction);

        Assert.IsNotNull(target);
        Assert.AreEqual("[E] Test Interact", target.GetHintText());
    }

    [UnityTest]
    public IEnumerator FindClosestInteractableClearsTargetWhenOutOfRange()
    {
        interactableObj.transform.position = new Vector3(10f, 0f, 0f); 
        yield return null;

        MethodInfo findMethod = typeof(PlayerInteraction).GetMethod("FindClosestInteractable", BindingFlags.NonPublic | BindingFlags.Instance);
        findMethod.Invoke(playerInteraction, null);

        FieldInfo targetField = typeof(PlayerInteraction).GetField("currentTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        IInteractable target = (IInteractable)targetField.GetValue(playerInteraction);

        Assert.IsNull(target);
    }
}