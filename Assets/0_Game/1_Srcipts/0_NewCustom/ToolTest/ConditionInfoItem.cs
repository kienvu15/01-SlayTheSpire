using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionInfoItem : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;

    public void Setup(string displayName, string description, string colorHex, Sprite icon = null)
    {
        nameText.text = $"<color={colorHex}>{displayName}</color>";
        descText.text = description;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }
    }
}