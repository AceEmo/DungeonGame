using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MinimapView))]
public class MinimapController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float worldRoomSize = 30f;
    [SerializeField] private string hubSceneName = SceneNames.HubRoom;

    private readonly MinimapData mapData = new MinimapData();
    private readonly Vector2Int invalidGridPosition = new Vector2Int(-999, -999);
    
    private MinimapView mapView;
    private MinimapCoordinateCalculator coordinateCalculator;
    private MinimapInputHandler inputHandler;
    
    private Vector2Int lastPlayerGridPos = new Vector2Int(-999, -999);
    private Transform playerTransform;

    private void Awake()
    {
        mapView = GetComponent<MinimapView>();
        coordinateCalculator = new MinimapCoordinateCalculator(worldRoomSize, invalidGridPosition);
        
        InitializeInput();
    }

    private void OnEnable()
    {
        LevelGenerator.OnLevelGenerated += InitializeMinimap;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        LevelGenerator.OnLevelGenerated -= InitializeMinimap;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayer();
        EvaluateCurrentSceneMap(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (!IsGameplayActive()) return;

        UpdatePlayerMovement();
        HandleMapToggle();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        EvaluateCurrentSceneMap(scene.name);
    }

    private void InitializeInput()
    {
        var provider = GetComponent<IInputProvider>();
        if (provider == null)
        {
            provider = gameObject.AddComponent<StandardInputProvider>();
        }
        inputHandler = new MinimapInputHandler(provider);
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private bool IsGameplayActive()
    {
        return GameManager.Instance != null && GameManager.Instance.IsGameplayActive();
    }

    private void EvaluateCurrentSceneMap(string sceneName)
    {
        if (sceneName == hubSceneName)
        {
            SetupDefaultHubMap();
        }
    }

    private void ResetMinimapState()
    {
        mapData.Clear();
        
        if (mapView != null)
        {
            mapView.ClearIcons();
        }
    }

    private void SetupDefaultHubMap()
    {
        ResetMinimapState();
        mapData.InitializeDefaultHubState();

        if (mapView != null)
        {
            foreach (Vector2Int position in mapData.RoomTypes.Keys)
            {
                mapView.CreateIcon(position);
            }
        }

        lastPlayerGridPos = invalidGridPosition;
        UpdatePlayerMovement();
    }

    private void InitializeMinimap(Dictionary<Vector2Int, Rooms> generatedRooms)
    {
        ResetMinimapState();
        FindPlayer();

        foreach (var pair in generatedRooms)
        {
            mapData.AddRoom(pair.Key, pair.Value.Type);
            
            if (mapView != null)
            {
                mapView.CreateIcon(pair.Key);
            }
        }

        lastPlayerGridPos = invalidGridPosition;
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (playerTransform == null || coordinateCalculator == null) return;

        Vector2Int currentPlayerGridPos = coordinateCalculator.GetPlayerGridPosition(playerTransform);

        if (currentPlayerGridPos != lastPlayerGridPos)
        {
            lastPlayerGridPos = currentPlayerGridPos;
            mapData.MarkAsExplored(currentPlayerGridPos);
            RefreshMapLayout(currentPlayerGridPos);
        }
    }

    private void HandleMapToggle()
    {
        if (inputHandler == null || mapView == null) return;
        if (!inputHandler.ShouldToggleMap()) return;

        if (inputHandler.IsLargeMapOpen)
        {
            mapView.DisplayLargeMap();
        }
        else
        {
            mapView.DisplayMinimap();
        }

        StartCoroutine(DelayedRefreshRoutine());
    }

    private void RefreshMapLayout(Vector2Int currentPlayerPos)
    {
        if (mapView == null || inputHandler == null) return;

        mapView.UpdateIconsState(mapData, currentPlayerPos);

        if (inputHandler.IsLargeMapOpen)
        {
            mapView.AutoZoomToExplored(mapData.GetKnownRooms(), lastPlayerGridPos);
        }
        else
        {
            mapView.CenterOn(currentPlayerPos, inputHandler.IsLargeMapOpen);
        }
    }

    private IEnumerator DelayedRefreshRoutine()
    {
        yield return null;
        RefreshMapLayout(lastPlayerGridPos);
    }
}