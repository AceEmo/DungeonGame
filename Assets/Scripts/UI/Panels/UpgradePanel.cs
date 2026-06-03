using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UpgradePanel : MonoBehaviour
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

    [SerializeField] private TextMeshProUGUI statsText;

    private UpgradeOption[] upgradeOptions;

    private struct UpgradeOption
    {
        public Button Button;
        public TextMeshProUGUI CostText;
        public int Cost;
        public Action Apply;
    }

    private void EnsureUpgradeOptionsBuilt()
    {
        if (upgradeOptions != null)
        {
            return;
        }

        upgradeOptions = new[]
        {
            new UpgradeOption
            {
                Button = healthButton,
                CostText = healthCostText,
                Cost = 50,
                Apply = ApplyHealthUpgrade
            },
            new UpgradeOption
            {
                Button = speedButton,
                CostText = speedCostText,
                Cost = 30,
                Apply = ApplySpeedUpgrade
            },
            new UpgradeOption
            {
                Button = fireRateButton,
                CostText = fireRateCostText,
                Cost = 30,
                Apply = ApplyFireRateUpgrade
            },
            new UpgradeOption
            {
                Button = bulletSpeedButton,
                CostText = bulletSpeedCostText,
                Cost = 20,
                Apply = ApplyBulletSpeedUpgrade
            },
            new UpgradeOption
            {
                Button = damageButton,
                CostText = damageCostText,
                Cost = 40,
                Apply = ApplyDamageUpgrade
            }
        };
    }

    private void OnEnable()
    {
        EnsureUpgradeOptionsBuilt();
        Refresh();
        playerStats.OnScrapChanged += Refresh;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
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

    private void UpdateButtons()
    {
        EnsureUpgradeOptionsBuilt();

        foreach (UpgradeOption option in upgradeOptions)
        {
            if (option.Button != null)
            {
                option.Button.interactable = playerStats.scrap >= option.Cost;
            }

            if (option.CostText != null)
            {
                option.CostText.text = option.Cost.ToString();
            }
        }
    }

    public void UpgradeHealth() => TryUpgrade(0);
    public void UpgradeSpeed() => TryUpgrade(1);
    public void UpgradeFireRate() => TryUpgrade(2);
    public void UpgradeBulletSpeed() => TryUpgrade(3);
    public void UpgradeDamage() => TryUpgrade(4);

    public void Close()
    {
        GameManager.Instance.CloseUpgrade();
    }

    private void TryUpgrade(int index)
    {
        EnsureUpgradeOptionsBuilt();

        UpgradeOption option = upgradeOptions[index];
        if (playerStats.scrap < option.Cost)
        {
            return;
        }

        playerStats.AddScrap(-option.Cost);
        option.Apply();
        Refresh();
    }

    private void ApplyHealthUpgrade()
    {
        const float healthIncrease = 2f;
        playerStats.maxHealth += healthIncrease;
        playerStats.startHealth += healthIncrease;
        playerHealth.Heal(healthIncrease);
        playerHealth.ApplyStats();
    }

    private void ApplySpeedUpgrade()
    {
        playerStats.moveSpeed += 0.5f;
    }

    private void ApplyFireRateUpgrade()
    {
        playerStats.fireRate += -0.05f;
    }

    private void ApplyBulletSpeedUpgrade()
    {
        playerStats.bulletSpeed += 2f;
    }

    private void ApplyDamageUpgrade()
    {
        playerStats.damage += 1;
    }
}
