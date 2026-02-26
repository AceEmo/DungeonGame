using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform panel;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHearts;
    }

    private void Start()
    {
        if (playerHealth != null)
            UpdateHearts(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void UpdateHearts(float current, float max)
    {
        int totalContainers = Mathf.CeilToInt(max / 2f);

        while (hearts.Count < totalContainers)
        {
            GameObject heart = Instantiate(heartPrefab, panel);
            hearts.Add(heart.GetComponent<Image>());
        }

        float health = current;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (health >= 2f)
            {
                hearts[i].sprite = fullHeart;
                health -= 2f;
            }
            else if (health >= 1f)
            {
                hearts[i].sprite = halfHeart;
                health -= 1f;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].gameObject.SetActive(i < totalContainers);
        }
    }
}