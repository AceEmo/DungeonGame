using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class HeartUITests
{
    private GameObject heartUIObject;
    private HeartUI heartUI;
    private GameObject playerObject;
    private PlayerHealth playerHealth;
    private GameObject heartContainer;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("PlayerTest");
        playerObject.SetActive(false);
        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<BoxCollider2D>();
        
        PlayerMovement movement = playerObject.AddComponent<PlayerMovement>();
        movement.enabled = false;

        playerHealth = playerObject.AddComponent<PlayerHealth>();
        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.startHealth = 6f;
        stats.maxHealth = 6f;
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerHealth, stats);
        
        playerObject.SetActive(true);

        heartUIObject = new GameObject("HeartUI");
        heartUI = heartUIObject.AddComponent<HeartUI>();

        heartContainer = new GameObject("Container");
        FieldInfo panelField = typeof(HeartUI).GetField("panel", BindingFlags.NonPublic | BindingFlags.Instance);
        panelField.SetValue(heartUI, heartContainer.transform);

        GameObject prefabObj = new GameObject("HeartPrefab");
        prefabObj.AddComponent<Image>();
        FieldInfo prefabField = typeof(HeartUI).GetField("heartPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        prefabField.SetValue(heartUI, prefabObj);

        FieldInfo fullField = typeof(HeartUI).GetField("fullHeart", BindingFlags.NonPublic | BindingFlags.Instance);
        fullField.SetValue(heartUI, Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.zero));
        
        FieldInfo halfField = typeof(HeartUI).GetField("halfHeart", BindingFlags.NonPublic | BindingFlags.Instance);
        halfField.SetValue(heartUI, Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.zero));
        
        FieldInfo emptyField = typeof(HeartUI).GetField("emptyHeart", BindingFlags.NonPublic | BindingFlags.Instance);
        emptyField.SetValue(heartUI, Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.zero));

        FieldInfo playerField = typeof(HeartUI).GetField("playerHealth", BindingFlags.NonPublic | BindingFlags.Instance);
        playerField.SetValue(heartUI, playerHealth);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(heartUIObject);
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(heartContainer);
    }

    [UnityTest]
    public IEnumerator UpdateHeartsCreatesCorrectNumberOfContainers()
    {
        MethodInfo startMethod = typeof(HeartUI).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(heartUI, null);

        yield return null;

        FieldInfo listField = typeof(HeartUI).GetField("hearts", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Image> hearts = (List<Image>)listField.GetValue(heartUI);

        Assert.AreEqual(3, hearts.Count);
    }

    [UnityTest]
    public IEnumerator UpdateHeartsChangesSpritesBasedOnHealth()
    {
        MethodInfo startMethod = typeof(HeartUI).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(heartUI, null);

        playerHealth.TakeDamage(1f, Vector2.zero);

        yield return null;

        FieldInfo listField = typeof(HeartUI).GetField("hearts", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Image> hearts = (List<Image>)listField.GetValue(heartUI);
        
        FieldInfo fullField = typeof(HeartUI).GetField("fullHeart", BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite fullSprite = (Sprite)fullField.GetValue(heartUI);
        
        FieldInfo halfField = typeof(HeartUI).GetField("halfHeart", BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite halfSprite = (Sprite)halfField.GetValue(heartUI);

        Assert.AreEqual(fullSprite, hearts[0].sprite);
        Assert.AreEqual(fullSprite, hearts[1].sprite);
        Assert.AreEqual(halfSprite, hearts[2].sprite);
    }
}