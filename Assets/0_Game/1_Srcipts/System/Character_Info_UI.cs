using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character_Info_UI : MonoBehaviour
{
    public static Character_Info_UI Instance;

    [Header("References")]
    public Image characterAvatarIcon_Image;
    public TextMeshProUGUI character_Name;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI spdText;
    
    public Player player; 

    public void Init_InfoUI(CharacterData playerData)
    {
        if (player == null) return;

        characterAvatarIcon_Image.sprite = playerData.player_AvatarIcon;
        character_Name.text = playerData.playerName;
        hpText.text = $"{player.stats.currentHP}/{player.stats.maxHP}";
        defText.text = $"{player.stats.shield}";
        spdText.text = $"{player.stats.spd}";
    }

    public void Update_InfoUI()
    {
        if (player == null) return;

        hpText.text = $"{player.stats.currentHP}/{player.stats.maxHP}";
        defText.text = $"{player.stats.shield}";
        spdText.text = $"{player.stats.spd}";
    }

}
