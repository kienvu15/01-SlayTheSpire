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

    private CharacterData currentData;

   
    public void Show(CharacterData data)
    {
        currentData = data;

        nameText.text = data.playerName;
        playerImage.sprite = data.playerSplashArt;
        hpText.text = $"{data.maxHP}/{data.maxHP}";
        goldText.text = $"{data.startCoin}";
        loreText.text = data.lore;

        if (data.startRelic != null)
        {
            relicIcon.sprite = data.startRelic.icon;
            relicName.text = data.startRelic.relicName;
            //relicDescription.text = data.startRelic.description;

        }
        else
        {
            relicIcon.sprite = null;
            relicName.text = "None";
        }
    }

    public CharacterData GetSelected() => currentData;
}
