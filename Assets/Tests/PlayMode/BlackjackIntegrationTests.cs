using System.Collections;
using System.Reflection;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class BlackjackIntegrationTests
{
    private GameObject environmentObject;
    private GameObject interactObject;
    private BlackjackGame game;
    private BlackjackUI ui;
    private BlackjackRewardSystem rewardSystem;
    private BlackjackInteract interact;

    private GameObject playerObject;
    private PlayerHealth playerHealth;

    private Transform itemSpawnPoint;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        environmentObject = new GameObject("BlackjackEnv");
        environmentObject.SetActive(false);

        ui = environmentObject.AddComponent<BlackjackUI>();
        rewardSystem = environmentObject.AddComponent<BlackjackRewardSystem>();
        game = environmentObject.AddComponent<BlackjackGame>();

        interactObject = new GameObject("InteractObj");
        interact = interactObject.AddComponent<BlackjackInteract>();
        GameManager.Instance.RegisterBlackjackCanvas(environmentObject);


        SetupUIMocks();
        SetupRewardMocks();
        SetupPlayerMock();

        interact.itemSpawnPoint = itemSpawnPoint;

        environmentObject.SetActive(true);
    }

    private void SetupUIMocks()
    {
        ui.playerCardArea = new GameObject("PlayerArea").transform;
        ui.playerCardArea.SetParent(environmentObject.transform);
        
        ui.dealerCardArea = new GameObject("DealerArea").transform;
        ui.dealerCardArea.SetParent(environmentObject.transform);

        GameObject cardPrefab = new GameObject("CardPrefab");
        cardPrefab.AddComponent<Image>();
        ui.cardPrefab = cardPrefab;

        ui.resultText = new GameObject("ResultText").AddComponent<TextMeshProUGUI>();
        ui.playerScoreText = new GameObject("PlayerScore").AddComponent<TextMeshProUGUI>();
        ui.dealerScoreText = new GameObject("DealerScore").AddComponent<TextMeshProUGUI>();

        ui.hitButton = new GameObject("HitBtn").AddComponent<Button>();
        ui.standButton = new GameObject("StandBtn").AddComponent<Button>();
        ui.exitButton = new GameObject("ExitBtn").AddComponent<Button>();

        Texture2D tex = new Texture2D(2, 2);
        ui.backCardSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), Vector2.zero);

        game.cardSprites = new Sprite[] { ui.backCardSprite };
    }

    private void SetupRewardMocks()
    {
        itemSpawnPoint = new GameObject("ItemSpawnPoint").transform;
        itemSpawnPoint.SetParent(environmentObject.transform);

        rewardSystem.rewardPrefab = new GameObject("RewardPrefab");
        rewardSystem.blackjackRewardPrefab = new GameObject("BJRewardPrefab");
    }

    private void SetupPlayerMock()
    {
        playerObject = new GameObject("PlayerTest");
        playerObject.SetActive(false);
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Animator>();
        
        PlayerMovement pm = playerObject.AddComponent<PlayerMovement>();
        pm.enabled = false;

        playerHealth = playerObject.AddComponent<PlayerHealth>();
        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.startHealth = 10f;
        stats.maxHealth = 10f;
        
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerHealth, stats);

        playerObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Time.timeScale = 1f;
        
        Object.DestroyImmediate(environmentObject);
        Object.DestroyImmediate(interactObject);
        Object.DestroyImmediate(playerObject);

        if (ui != null && ui.backCardSprite != null)
        {
            Object.DestroyImmediate(ui.backCardSprite.texture);
            Object.DestroyImmediate(ui.backCardSprite);
        }

        if (GameManager.Instance != null)
        {
            Object.DestroyImmediate(GameManager.Instance.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator InteractActivatesCanvasAndSetsTimeScaleToZero()
    {
        environmentObject.SetActive(false);

        interact.Interact();

        Assert.IsTrue(environmentObject.activeSelf);
        Assert.AreEqual(0f, Time.timeScale);

        yield return null;
    }

    [UnityTest]
    public IEnumerator ExitGameRestoresTimeScaleAndDeactivatesCanvas()
    {
        Time.timeScale = 0f;

        FieldInfo gameOverField = typeof(BlackjackGame).GetField("gameOver", BindingFlags.NonPublic | BindingFlags.Instance);
        gameOverField.SetValue(game, true);

        game.ExitGame();

        Assert.AreEqual(1f, Time.timeScale);
        Assert.IsFalse(environmentObject.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator RewardSystemWinRoutineSpawnsCorrectPrefab()
    {
        int before = Object
            .FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Count(go => go.name == "BJRewardPrefab");

        yield return rewardSystem.WinRoutine(ui, true, itemSpawnPoint);

        int after = Object
            .FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Count(go => go.name == "BJRewardPrefab");

        Assert.AreEqual(before + 1, after);
        Assert.IsTrue(ui.exitButton.interactable);
    }

    [UnityTest]
    public IEnumerator RewardSystemLoseRoutineDamagesPlayer()
    {
        float initialHealth = playerHealth.CurrentHealth;

        yield return rewardSystem.LoseRoutine(ui, environmentObject.transform);

        Assert.Less(playerHealth.CurrentHealth, initialHealth);
        Assert.IsTrue(ui.exitButton.interactable);
    }

    [UnityTest]
    public IEnumerator GameFlowClearsTableAndResetsScoresOnStart()
    {
        ui.playerScoreText.text = "99";
        ui.resultText.text = "OLD";

        game.StartBlackjack();

        yield return new WaitForSecondsRealtime(0.1f);

        Assert.AreEqual("0", ui.playerScoreText.text);
        Assert.AreEqual(" ", ui.resultText.text);
    }
}