using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIntentUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI valueText;

    // Update icon và value
    public void SetIntent(Sprite intentIcon, int value)
    {
        if (icon != null) icon.sprite = intentIcon;
        if (valueText != null) valueText.text = value.ToString();
    }
}
