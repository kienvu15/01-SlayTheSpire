using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoHeader : MonoBehaviour
{
    [Header("References")]
    public Slider healBar;
    public TextMeshProUGUI hpText;
    public Player player; // tham chiếu tới Player

    void Start()
    {
        if (player != null && healBar != null)
        {
            healBar.maxValue = player.stats.maxHP;
            healBar.value = player.stats.currentHP;
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player == null) return;

        if (healBar != null)
        {
            healBar.maxValue = player.stats.maxHP;
            healBar.value = player.stats.currentHP;
        }

        if (hpText != null)
        {
            hpText.text = $"{player.stats.currentHP}/{player.stats.maxHP}";
        }
    }

}
