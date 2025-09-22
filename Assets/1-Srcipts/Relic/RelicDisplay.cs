using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject descriptionPanel;
    public TextMeshProUGUI relicNameText;
    public TextMeshProUGUI relicDescText;
    public GameObject nameText;

    public Button iconButton;
    public GameObject buyButton;

    public ShopSystem shopSystem;
    private Relic relicData;
    private bool isShopMode = false;

    void Start()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        nameText.SetActive(false);
    }

    public void Setup(Relic relic, bool shopMode)
    {
        relicData = relic;
        isShopMode = shopMode;

        if (iconImage != null)
            iconImage.sprite = relic.icon;

        relicNameText.text = relic.relicName;
        relicDescText.text = relic.description;

        iconButton.onClick.AddListener(OnClick);

        shopSystem = GetComponentInParent<ShopSystem>();
        if (shopSystem == null)
            buyButton.SetActive(false);

        //buyButton.gameObject.SetActive(shopMode);
        //buyButton.onClick.RemoveAllListeners();
        //if (shopMode)
        //{
        //    buyButton.onClick.AddListener(() =>
        //    {
        //        Debug.Log($"[Shop] Bought relic: {relic.relicName}");
        //        descriptionPanel.SetActive(false);
        //        // TODO: Gọi RelicManager.EquipRelic(relic)
        //    });
        //}

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
                    buyButton.SetActive(false); // ẩn nút sau khi mua
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
        descriptionPanel.SetActive(!descriptionPanel.activeSelf);
    }
}
