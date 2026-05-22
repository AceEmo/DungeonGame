using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapView : MonoBehaviour
{
    [System.Serializable]
    public struct RoomSprites
    {
        public Sprite normalRoom;
        public Sprite blackjackRoom;
        public Sprite upgradeRoom;
        public Sprite bossRoom;
        public Sprite unexploredRoom;
    }

    [System.Serializable]
    public struct OriginalLayoutData
    {
        public Vector2 sizeDelta;
        public Vector3 anchoredPosition;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
    }

    [Header("UI Containers")]
    [SerializeField] private RectTransform minimapPanel;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject roomMinimapPrefab;
    [SerializeField] private RectTransform playerIndicator;

    [Header("Sprites Configuration")]
    [SerializeField] private RoomSprites sprites;
    [SerializeField] private float uiRoomSpacing = 35f;

    private readonly Dictionary<Vector2Int, Image> minimapIcons = new Dictionary<Vector2Int, Image>();
    private OriginalLayoutData originalLayout;

    private void Awake()
    {
        SaveOriginalLayout();
        if (playerIndicator != null)
        {
            playerIndicator.gameObject.SetActive(false);
        }
    }

    public void ClearIcons()
    {
        foreach (Image icon in minimapIcons.Values)
        {
            if (icon != null)
            {
                Destroy(icon.gameObject);
            }
        }
        minimapIcons.Clear();
    }

    public void CreateIcon(Vector2Int gridPos)
    {
        GameObject iconObj = Instantiate(roomMinimapPrefab, gridContainer);
        Image iconImage = iconObj.GetComponent<Image>();

        RectTransform rectTrans = iconObj.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(gridPos.x * uiRoomSpacing, gridPos.y * uiRoomSpacing);

        iconObj.SetActive(false);
        minimapIcons.Add(gridPos, iconImage);
    }

    public void UpdateIconsState(MinimapData data, Vector2Int currentPlayerPos)
    {
        if (minimapIcons.Count == 0) return;

        foreach (var pair in minimapIcons)
        {
            SetIconVisibilityAndSprite(pair.Key, pair.Value, data);
        }
        
        UpdatePlayerIndicator(currentPlayerPos);
    }

    public void CenterOn(Vector2Int gridPos, bool isLargeMapOpen)
    {
        if (gridContainer == null) return;

        float spacing = isLargeMapOpen ? uiRoomSpacing * 2f : uiRoomSpacing;
        gridContainer.anchoredPosition = new Vector2(-gridPos.x * spacing, -gridPos.y * spacing);
    }

    public void AutoZoomToExplored(List<Vector2Int> knownRooms, Vector2Int lastPlayerGridPos)
    {
        if (minimapPanel == null || knownRooms.Count == 0) return;

        CalculateBounds(knownRooms, out int minX, out int maxX, out int minY, out int maxY);

        float width = (maxX - minX + 1) * uiRoomSpacing;
        float height = (maxY - minY + 1) * uiRoomSpacing;

        gridContainer.anchoredPosition = new Vector2(-lastPlayerGridPos.x * uiRoomSpacing, -lastPlayerGridPos.y * uiRoomSpacing);

        float scaleX = minimapPanel.rect.width / width;
        float scaleY = minimapPanel.rect.height / height;

        float finalScale = Mathf.Clamp(Mathf.Min(scaleX, scaleY), 1f, 3f);
        gridContainer.localScale = new Vector3(finalScale, finalScale, 1f);
    }

    public void DisplayLargeMap()
    {
        if (minimapPanel == null) return;

        minimapPanel.anchorMin = new Vector2(0.05f, 0.05f);
        minimapPanel.anchorMax = new Vector2(0.95f, 0.95f);
        minimapPanel.pivot = new Vector2(0.5f, 0.5f);
        minimapPanel.anchoredPosition = Vector2.zero;
        minimapPanel.offsetMin = Vector2.zero;
        minimapPanel.offsetMax = Vector2.zero;
    }

    public void DisplayMinimap()
    {
        if (minimapPanel == null) return;

        minimapPanel.anchorMin = originalLayout.anchorMin;
        minimapPanel.anchorMax = originalLayout.anchorMax;
        minimapPanel.pivot = originalLayout.pivot;
        minimapPanel.anchoredPosition = originalLayout.anchoredPosition;
        minimapPanel.sizeDelta = originalLayout.sizeDelta;

        gridContainer.localScale = Vector3.one;
    }

    private void SetIconVisibilityAndSprite(Vector2Int pos, Image iconImage, MinimapData data)
    {
        if (data.IsExplored(pos))
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = GetSpriteForRoom(data.RoomTypes[pos]);
        }
        else if (data.IsDiscovered(pos))
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = sprites.unexploredRoom;
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerIndicator(Vector2Int currentPlayerPos)
    {
        if (playerIndicator == null) return;

        if (minimapIcons.ContainsKey(currentPlayerPos))
        {
            playerIndicator.gameObject.SetActive(true);
            playerIndicator.anchoredPosition = new Vector2(currentPlayerPos.x * uiRoomSpacing, currentPlayerPos.y * uiRoomSpacing);
            playerIndicator.SetAsLastSibling();
        }
        else
        {
            playerIndicator.gameObject.SetActive(false);
        }
    }

    private void SaveOriginalLayout()
    {
        if (minimapPanel == null) return;

        originalLayout = new OriginalLayoutData
        {
            sizeDelta = minimapPanel.sizeDelta,
            anchoredPosition = minimapPanel.anchoredPosition,
            anchorMin = minimapPanel.anchorMin,
            anchorMax = minimapPanel.anchorMax,
            pivot = minimapPanel.pivot
        };
    }

    private Sprite GetSpriteForRoom(RoomType type)
    {
        return type switch
        {
            RoomType.Normal => sprites.normalRoom,
            RoomType.Starter => sprites.normalRoom,
            RoomType.Blackjack => sprites.blackjackRoom,
            RoomType.Upgrade => sprites.upgradeRoom,
            RoomType.Boss => sprites.bossRoom,
            _ => sprites.normalRoom
        };
    }

    private void CalculateBounds(List<Vector2Int> rooms, out int minX, out int maxX, out int minY, out int maxY)
    {
        minX = int.MaxValue;
        maxX = int.MinValue;
        minY = int.MaxValue;
        maxY = int.MinValue;

        foreach (Vector2Int pos in rooms)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }
    }
}