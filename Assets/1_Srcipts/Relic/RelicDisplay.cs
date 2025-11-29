using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicDisplay : MonoBehaviour, IHasDescriptionPanel
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject descriptionPanel;
    public TextMeshProUGUI[] relicNameText;
    public TextMeshProUGUI relicDescText;
    public GameObject nameText;

    [Header("Shop System")]
    public NewShopSystem shopSystem;
    private Relic relicData;
    private bool isShopMode = false;

    [Header("Shop UI")]
    public GameObject goldContainer;
    public TextMeshProUGUI priceTXT;
    public Button buyButton;
    public GameObject soldOut;

    public CanvasGroup blockerGroup;

    void OnEnable() => DescriptionPanelManager.Instance.Register(this);
    void OnDisable() => DescriptionPanelManager.Instance.Unregister(this);

    public GameObject GetDescriptionPanel() => descriptionPanel;
    public GameObject GetRootObject() => gameObject;
    public void vHideDescription() => HideDescription();

    private void Start()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
        blockerGroup = DescriptionPanelManager.Instance.blockerGroup;
        // Setup(relicData, false);
    }

    private void Update()
    {
        //if (descriptionPanel.activeSelf && Input.GetMouseButtonDown(0))
        //{
        //    if (!RectTransformUtility.RectangleContainsScreenPoint(
        //            descriptionPanel.GetComponent<RectTransform>(),
        //            Input.mousePosition, null) &&
        //        !RectTransformUtility.RectangleContainsScreenPoint(
        //            iconButton.GetComponent<RectTransform>(),
        //            Input.mousePosition, null))
        //    {
        //        descriptionPanel.SetActive(false);
        //    }
        //}
    }

    public void Setup(Relic relic, bool shopMode)
    {
        relicData = relic;
        isShopMode = shopMode;
        if (iconImage != null)
            iconImage.sprite = relic.icon;
        for (int i = 0; i < relicNameText.Length; i++)
        {
            relicNameText[i].text = relic.relicName;
        }
        relicDescText.text = relic.description;
        shopSystem = GetComponentInParent<NewShopSystem>();
        descriptionPanel.SetActive(false);
    }

    public void Init(Relic data, NewShopSystem shop)
    {
        relicData = data;
        shopSystem = shop;
        goldContainer.SetActive(true);
        priceTXT.text = relicData.goldCost.ToString();
        buyButton.gameObject.SetActive(true);

        Setup(relicData, true);
    }

    public void BuyRelic()
    {
        descriptionPanel.gameObject.SetActive(false);
        int cost = relicData.goldCost;
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(cost))
        {
            SoundManager.Instance.Play("Coin");
            RelicManager.Instance.AddRelic(relicData);
            buyButton.gameObject.SetActive(false);
            goldContainer.SetActive(false);
            gameObject.GetComponent<Button>().interactable = false;
            soldOut.SetActive(true);
        }
    }

    public void HideDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        // 2. Xóa đoạn check static này đi để tránh lỗi logic
        // if (currentlyOpen == this)
        //    currentlyOpen = null;

        HideBlocker();
    }

    private void ShowBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.alpha = 0;
            blockerGroup.blocksRaycasts = true;
            blockerGroup.interactable = true;
        }
    }

    private void HideBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.blocksRaycasts = false; // cho click xuyên qua
            blockerGroup.interactable = false;
        }
    }

    public void OnClick()
    {
        // 3. CHỈ GỌI DÒNG NÀY. Để ShopSystem tự quyết định việc Bật/Tắt
        if (shopSystem != null)
        {
            shopSystem.SelectRelic(this);
        }
    }

    public void ToggleDescription()
    {
        if (descriptionPanel == null) return;

        // 4. Xóa logic đóng card khác ở đây, ShopSystem sẽ lo việc đó
        // if (currentlyOpen != null && currentlyOpen != this)
        //    currentlyOpen.HideDescription();

        bool isOpening = !descriptionPanel.activeSelf;
        descriptionPanel.SetActive(isOpening);

        if (isOpening)
        {
            // currentlyOpen = this; // Xóa
            ShowBlocker();

            // Thêm hình ảnh minh họa cho dễ hình dung luồng dữ liệu (nếu cần)
            //  -> Không cần thiết ở đây, code khá rõ.
        }
        else
        {
            // currentlyOpen = null; // Xóa
            HideBlocker();
        }
    }
}
