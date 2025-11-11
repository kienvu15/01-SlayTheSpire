using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject descriptionPanel;
    public TextMeshProUGUI[] relicNameText;
    public TextMeshProUGUI relicDescText;
    public GameObject nameText;

    public Button iconButton;
    public GameObject buyButton;
    public TextMeshProUGUI priceTXT;

    public ShopSystem shopSystem;
    private Relic relicData;
    private bool isShopMode = false;

    private void Start()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        nameText.SetActive(false);

        // Gắn sự kiện click icon
        if (iconButton != null)
            iconButton.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        // Nếu panel đang mở mà người chơi click chuột trái ra ngoài → tắt
        if (descriptionPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Kiểm tra nếu không click lên chính panel hoặc icon
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    descriptionPanel.GetComponent<RectTransform>(),
                    Input.mousePosition, null) &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    iconButton.GetComponent<RectTransform>(),
                    Input.mousePosition, null))
            {
                descriptionPanel.SetActive(false);
            }
        }
    }

    public void Setup(Relic relic, bool shopMode)
    {
        relicData = relic;
        isShopMode = shopMode;

        if (iconImage != null)
            iconImage.sprite = relic.icon;

        foreach (var txt in relicNameText)
            txt.text = relic.relicName;

        relicDescText.text = relic.description;

        priceTXT.text = Random.Range(50, 180).ToString();

        shopSystem = GetComponentInParent<ShopSystem>();
        if (shopSystem == null)
            buyButton.SetActive(false);

        descriptionPanel.SetActive(false);
    }

    public void Init(Relic data, ShopSystem shop)
    {
        relicData = data;
        shopSystem = shop;
        Setup(relicData, false);

        if (buyButton != null)
        {
            if (shopSystem != null)
            {
                buyButton.SetActive(true);
                var btn = buyButton.GetComponentInChildren<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    shopSystem.BuyRelic(relicData);
                    Button buyButtonBB = buyButton.GetComponent<Button>();
                    buyButtonBB.onClick.RemoveAllListeners();
                    buyButtonBB.interactable = false;
                    priceTXT.text = "Sold Out";
                });
            }
            else
            {
                buyButton.SetActive(false);
            }
        }
    }

    public void OnClick()
    {
        bool isActive = descriptionPanel.activeSelf;
        descriptionPanel.SetActive(!isActive);
    }
}
