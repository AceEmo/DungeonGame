using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance { get; private set; }

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;
    
    [SerializeField] private Vector3 hintOffset = new Vector3(0f, 30f, 0f);

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainCamera = Camera.main;
        HideHint();
    }

    public void ShowHint(string text, Vector3 worldPosition)
    {
        if (hintPanel == null || hintText == null) return;

        if (GameManager.Instance != null && !GameManager.Instance.IsGameplayActive())
        {
            HideHint();
            return;
        }

        hintPanel.SetActive(true);
        hintText.text = text;
        
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        hintPanel.transform.position = screenPosition + hintOffset;
    }

    public void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}