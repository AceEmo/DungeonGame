using UnityEngine;
using TMPro;

public class ScrapUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scrapText;

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        scrapText.text = GameManager.Instance.Scrap.ToString("00");
    }
}