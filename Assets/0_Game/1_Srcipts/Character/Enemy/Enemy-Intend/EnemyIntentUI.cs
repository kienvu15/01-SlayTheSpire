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

        if (value <= 0)
        {
            valueText.gameObject.SetActive(false);
        }
        else if (value > 0)
        {
            valueText.gameObject.SetActive(true);
            if (valueText != null) valueText.text = value.ToString();
        }
    }
}
