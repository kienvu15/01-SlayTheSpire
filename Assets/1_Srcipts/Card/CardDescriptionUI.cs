using UnityEngine;
using TMPro;

public class CardDescriptionUI : MonoBehaviour
{
    public static CardDescriptionUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string description)
    {
        descriptionText.text = description;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public bool IsVisible => panel.activeSelf;
}
