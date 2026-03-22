using UnityEngine;
using TMPro;

public class ScrapUI : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private TextMeshProUGUI scrapText;


    private void Start()
    {
        UpdateScrapDisplay();
    }
    
    private void OnEnable()
    {
        if (stats != null)
        {
            stats.OnScrapChanged += UpdateScrapDisplay;
        }
        
        UpdateScrapDisplay();
    }

    private void OnDisable()
    {
        if (stats != null)
        {
            stats.OnScrapChanged -= UpdateScrapDisplay;
        }
    }

    private void UpdateScrapDisplay()
    {
        if (scrapText != null && stats != null)
        {
            scrapText.text = stats.scrap.ToString("00");
        }
    }
}