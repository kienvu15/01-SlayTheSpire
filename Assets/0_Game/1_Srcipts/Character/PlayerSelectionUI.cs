using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI nameText;
    public Image playerImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI loreText;
    public Image relicIcon;
    public TextMeshProUGUI relicName;

    private PlayerData currentData;

    public void Show(PlayerData data)
    {
        currentData = data;

        nameText.text = data.playerName;
        playerImage.sprite = data.playerSplashArt;
        hpText.text = $"HP: {data.maxHP}";
        loreText.text = data.lore;

        if (data.startRelic != null)
        {
            relicIcon.sprite = data.startRelic.icon;
            relicName.text = data.startRelic.relicName;
        }
        else
        {
            relicIcon.sprite = null;
            relicName.text = "None";
        }
    }

    public PlayerData GetSelected() => currentData;
}
