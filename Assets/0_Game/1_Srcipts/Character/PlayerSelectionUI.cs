using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI nameText;
    public Image playerImage;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI goldText;

    public TextMeshProUGUI loreText;

    public Image relicIcon;
    public TextMeshProUGUI relicName;
    public TextMeshProUGUI relicDescription;

    private PlayerData currentData;

    public void Show(PlayerData data)
    {
        currentData = data;

        nameText.text = data.playerName;
        playerImage.sprite = data.playerSplashArt;
        hpText.text = $"{data.maxHP}/{data.maxHP}";
        goldText.text = $"{data.startGold}";
        loreText.text = data.lore;

        if (data.startRelic != null)
        {
            relicIcon.sprite = data.startRelic.icon;
            relicName.text = data.startRelic.relicName;
            relicDescription.text = data.startRelic.description;

        }
        else
        {
            relicIcon.sprite = null;
            relicName.text = "None";
        }
    }

    public PlayerData GetSelected() => currentData;
}
