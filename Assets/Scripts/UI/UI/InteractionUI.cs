using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance { get; private set; }

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    
    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); 
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowHint(string text, Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null) return;

        hintText.text = text;
        hintPanel.SetActive(true);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        hintPanel.transform.position = screenPos;
    }

    public void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}