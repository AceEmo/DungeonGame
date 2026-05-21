using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MinimapView))]
public class MinimapController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float worldRoomSize = 30f;

    private readonly MinimapData mapData = new MinimapData();
    private MinimapView mapView;
    
    private readonly Vector2Int invalidGridPosition = new Vector2Int(-999, -999);
    private Vector2Int lastPlayerGridPos = new Vector2Int(-999, -999);

    private Transform playerTransform;
    private IInputProvider inputProvider;
    private bool isLargeMapOpen = false;

    private void OnEnable()
    {
        LevelGenerator.OnLevelGenerated += InitializeMinimap;
    }

    private void OnDisable()
    {
        LevelGenerator.OnLevelGenerated -= InitializeMinimap;
    }

    private void Start()
    {
        mapView = GetComponent<MinimapView>();
        InitializeInput();
        ResetMinimapState();
        FindPlayer();
    }

    private void Update()
    {
        if (!IsGameplayActive()) return;

        UpdatePlayerMovement();
        HandleMapToggleInput();
    }

    private bool IsGameplayActive()
    {
        return GameManager.Instance != null && GameManager.Instance.IsGameplayActive();
    }

    private void InitializeInput()
    {
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<StandardInputProvider>();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void ResetMinimapState()
    {
        mapData.Clear();
        mapView.ClearIcons();
    }

    private void InitializeMinimap(Dictionary<Vector2Int, Rooms> generatedRooms)
    {
        ResetMinimapState();
        FindPlayer();

        foreach (var pair in generatedRooms)
        {
            Vector2Int gridPos = pair.Key;
            Rooms room = pair.Value;

            mapData.AddRoom(gridPos, room.Type);
            mapView.CreateIcon(gridPos);
        }

        lastPlayerGridPos = invalidGridPosition;
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (playerTransform == null) return;

        Vector2Int currentPlayerGridPos = GetPlayerGridPosition();

        if (currentPlayerGridPos != lastPlayerGridPos)
        {
            lastPlayerGridPos = currentPlayerGridPos;
            mapData.MarkAsExplored(currentPlayerGridPos);
            
            RefreshMapLayout(currentPlayerGridPos);
        }
    }

    private Vector2Int GetPlayerGridPosition()
    {
        if (playerTransform == null) return invalidGridPosition;

        int playerGridX = Mathf.RoundToInt(playerTransform.position.x / worldRoomSize);
        int playerGridY = Mathf.RoundToInt(playerTransform.position.y / worldRoomSize);
        return new Vector2Int(playerGridX, playerGridY);
    }

    private void RefreshMapLayout(Vector2Int currentPlayerPos)
    {
        mapView.UpdateIconsState(mapData, currentPlayerPos);

        if (isLargeMapOpen)
        {
            mapView.AutoZoomToExplored(mapData.GetKnownRooms(), lastPlayerGridPos);
        }
        else
        {
            mapView.CenterOn(currentPlayerPos, isLargeMapOpen);
        }
    }

    private void HandleMapToggleInput()
    {
        if (inputProvider.GetButtonDown("Map"))
        {
            isLargeMapOpen = !isLargeMapOpen;

            if (isLargeMapOpen)
                mapView.DisplayLargeMap();
            else
                mapView.DisplayMinimap();

            StartCoroutine(DelayedRefresh());
        }
    }

    private IEnumerator DelayedRefresh()
    {
        yield return null;
        RefreshMapLayout(lastPlayerGridPos);
    }
}