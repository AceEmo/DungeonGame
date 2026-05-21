using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject roomMinimapPrefab;

    [Header("Minimap Settings")]
    [SerializeField] private float uiRoomSpacing = 35f;
    [SerializeField] private Color currentRoomColor = Color.white;
    [SerializeField] private Color visitedRoomColor = Color.gray;

    private readonly Dictionary<Vector2Int, Image> minimapIcons = new Dictionary<Vector2Int, Image>();
    private Vector2Int lastPlayerGridPos = new Vector2Int(-999, -999);
    private Transform playerTransform;

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
        FindPlayer();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive()) return;

        TrackPlayerMovement();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void InitializeMinimap(Dictionary<Vector2Int, Rooms> generatedRooms)
    {
        ClearMinimap();
        FindPlayer();

        foreach (var pair in generatedRooms)
        {
            Vector2Int gridPos = pair.Key;
            
            GameObject iconObj = Instantiate(roomMinimapPrefab, gridContainer);
            Image iconImage = iconObj.GetComponent<Image>();

            RectTransform rectTrans = iconObj.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(gridPos.x * uiRoomSpacing, gridPos.y * uiRoomSpacing);

            iconObj.SetActive(false);

            minimapIcons.Add(gridPos, iconImage);
        }

        lastPlayerGridPos = new Vector2Int(-999, -999);
    }

    private void TrackPlayerMovement()
    {
        if (playerTransform == null) return;

        float roomSize = 30f; 
        
        int playerGridX = Mathf.RoundToInt(playerTransform.position.x / roomSize);
        int playerGridY = Mathf.RoundToInt(playerTransform.position.y / roomSize);
        Vector2Int currentPlayerGridPos = new Vector2Int(playerGridX, playerGridY);

        if (currentPlayerGridPos != lastPlayerGridPos)
        {
            UpdateRoomVisibilities(currentPlayerGridPos);
            lastPlayerGridPos = currentPlayerGridPos;
        }
    }

    private void UpdateRoomVisibilities(Vector2Int currentPlayerPos)
    {
        if (minimapIcons.ContainsKey(lastPlayerGridPos))
        {
            minimapIcons[lastPlayerGridPos].color = visitedRoomColor;
        }

        if (minimapIcons.ContainsKey(currentPlayerPos))
        {
            minimapIcons[currentPlayerPos].gameObject.SetActive(true);
            minimapIcons[currentPlayerPos].color = currentRoomColor;
        }

        Vector2Int[] neighbors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in neighbors)
        {
            Vector2Int neighborPos = currentPlayerPos + dir;
            if (minimapIcons.ContainsKey(neighborPos))
            {
                minimapIcons[neighborPos].gameObject.SetActive(true);
            }
        }

        CenterMinimapOn(currentPlayerPos);
    }

    private void CenterMinimapOn(Vector2Int gridPos)
    {
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        gridRect.anchoredPosition = new Vector2(-gridPos.x * uiRoomSpacing, -gridPos.y * uiRoomSpacing);
    }

    private void ClearMinimap()
    {
        foreach (var icon in minimapIcons.Values)
        {
            if (icon != null) Destroy(icon.gameObject);
        }
        minimapIcons.Clear();
    }
}