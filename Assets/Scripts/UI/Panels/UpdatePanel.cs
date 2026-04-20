using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdatePanel : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerHealth playerHealth;

    public Button healthButton;
    public Button speedButton;
    public Button fireRateButton;
    public Button bulletSpeedButton;
    public Button damageButton;

    public TextMeshProUGUI healthCostText;
    public TextMeshProUGUI speedCostText;
    public TextMeshProUGUI fireRateCostText;
    public TextMeshProUGUI bulletSpeedCostText;
    public TextMeshProUGUI damageCostText;

    private int healthCost = 50;
    private int speedCost = 30;
    private int fireRateCost = 30;
    private int bulletSpeedCost = 20;
    private int damageCost = 40;

    private float healthIncrease = 2f;
    private float speedIncrease = 0.5f;
    private float fireRateIncrease = -0.05f;
    private float bulletSpeedIncrease = 2f;
    private int damageIncrease = 1;

    [SerializeField] private TextMeshProUGUI statsText;

    private void OnEnable()
    {
        Refresh();
        playerStats.OnScrapChanged += Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.CloseUpgrade();
        }
    }

    private void OnDisable()
    {
        playerStats.OnScrapChanged -= Refresh;
    }

    private void Refresh()
    {
        statsText.text =
        "<color=#50C8FF><b>>>> PLAYER STATS <<<</b></color>\n" +
        "<color=#404040>-----------------------------</color>\n\n" +

        "<color=#808080>HP:\t\t\t</color><color=#00FF90>" + playerStats.maxHealth + "</color>\n" +
        "<color=#808080>SPEED:\t\t</color><color=#00FF90>" + playerStats.moveSpeed.ToString("F1") + "</color>\n" +
        "<color=#808080>FIRE RATE:\t\t</color><color=#00FF90>" + playerStats.fireRate.ToString("F2") + "</color>\n" +
        "<color=#808080>BULLET SPD:\t</color><color=#00FF90>" + playerStats.bulletSpeed.ToString("F1") + "</color>\n" +
        "<color=#808080>DAMAGE:\t\t</color><color=#FF5555>" + playerStats.damage + "</color>\n\n" +

        "<color=#404040>-----------------------------</color>";

        UpdateButtons();
    }
    void UpdateButtons()
    {
        healthButton.interactable = playerStats.scrap >= healthCost;
        speedButton.interactable = playerStats.scrap >= speedCost;
        fireRateButton.interactable = playerStats.scrap >= fireRateCost;
        bulletSpeedButton.interactable = playerStats.scrap >= bulletSpeedCost;
        damageButton.interactable = playerStats.scrap >= damageCost;
    }    
    
    public void UpgradeHealth()
    {
        if (playerStats.scrap < healthCost) return;

        playerStats.AddScrap(-healthCost);
        playerStats.maxHealth += healthIncrease;
        playerStats.startHealth += healthIncrease;

        playerHealth.Heal(healthIncrease);

        playerHealth.ApplyStats();
        Refresh();
    }

    public void UpgradeSpeed()
    {
        if (playerStats.scrap < speedCost) return;

        playerStats.AddScrap(-speedCost);
        playerStats.moveSpeed += speedIncrease;

        Refresh();
    }

    public void UpgradeFireRate()
    {
        if (playerStats.scrap < fireRateCost) return;

        playerStats.AddScrap(-fireRateCost);
        playerStats.fireRate += fireRateIncrease;

        Refresh();
    }

    public void UpgradeBulletSpeed()
    {
        if (playerStats.scrap < bulletSpeedCost) return;

        playerStats.AddScrap(-bulletSpeedCost);
        playerStats.bulletSpeed += bulletSpeedIncrease;

        Refresh();
    }

    public void UpgradeDamage()
    {
        if (playerStats.scrap < damageCost) return;

        playerStats.AddScrap(-damageCost);
        playerStats.damage += damageIncrease;

        Refresh();
    }

    public void Close()
    {
        GameManager.Instance.CloseUpgrade();
    }
}