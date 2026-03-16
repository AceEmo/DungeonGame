using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        hintPanel.SetActive(false);
    }

    public void ShowHint(string text, Vector3 worldPosition)
    {
        if (!hintPanel || !hintText) return;
        hintPanel.SetActive(true);
        hintText.text = text;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        hintPanel.transform.position = screenPos + new Vector3(0, 30, 0);
    }

    public void HideHint()
    {
        if (!hintPanel) return;
        hintPanel.SetActive(false);
    }
}