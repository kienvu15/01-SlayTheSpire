using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject selected_Sprite;
    //public TextMeshProUGUI relicNameText;

    [Header("Button")]
    public Button relic_Button;

    [Header("Shop System")]
    public NewShopSystem shopSystem;
    private bool isShopMode = false;

    [Header("Shop UI")]
    public GameObject goldContainer;
    public TextMeshProUGUI priceTXT;
    public Button buyButton;
    public GameObject soldOut;

    public CanvasGroup blockerGroup;

    private Relic relicData;
    private RelicInstance relicInstance;
    private static bool firstTimeClick = false;
    private static RelicDisplay currentSelected;

    public void Init_Data(RelicInstance data)
    {
        relicInstance = data;
        relicData = data.relicData;

        iconImage.sprite = relicData.icon;

        UpdateRelicUI();
    }

    public void UpdateRelicUI()
    {
        if (relicInstance == null) return;
        //descriptionText.text = relicInstance.GetDescription();
        //levelText.text = $"Lv.{relicInstance.level}";
        //upgradeButton.gameObject.SetActive(relicInstance.CanUpgrade());
    }

    public void OnClickAddLisent()
    {
        relic_Button.onClick.AddListener(relic_Clicked);
    }
    public void relic_Clicked()
    {
        if (currentSelected != null && currentSelected != this)
        {
            currentSelected.Deselect();
        }
        currentSelected = this;
        selected_Sprite.SetActive(true);
        //if(firstTimeClick == false)
        //{
            firstTimeClick = true;
            UIManager.Instance.RelicInfo_Help(true);
        //}
        UIManager.Instance.Init_RelicInfor(relicInstance);
    }

    public void Deselect()
    {
        selected_Sprite.SetActive(false);
    }

    public static RelicDisplay GetSelectedRelic()
    {
        return currentSelected;
    }






    public void Setup(Relic relic, bool shopMode)
    {
        relicData = relic;
        isShopMode = shopMode;
        if (iconImage != null)
            iconImage.sprite = relic.icon;
        //relicNameText.text = relic.relicName;

        //for (int i = 0; i < relicNameText.Length; i++)
        //{
        //    relicNameText[i].text = relic.relicName;
        //}
        //relicDescText.text = relic.description;
        //shopSystem = GetComponentInParent<NewShopSystem>();
        //descriptionPanel.SetActive(false);
    }

    public void Init(Relic data, NewShopSystem shop)
    {
        relicData = data;
        shopSystem = shop;
        goldContainer.SetActive(true);
        priceTXT.text = relicData.gold_Buy.ToString();
        buyButton.gameObject.SetActive(true);

        Setup(relicData, true);
    }

    public void BuyRelic()
    {
        //descriptionPanel.gameObject.SetActive(false);
        int cost = relicData.gold_Buy;
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
        //if (descriptionPanel != null)
        //    descriptionPanel.SetActive(false);

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
        else
        {
            ToggleDescription();
        }
    }

    public void ToggleDescription()
    {
        //if (descriptionPanel == null) return;

        //// 4. Xóa logic đóng card khác ở đây, ShopSystem sẽ lo việc đó
        //// if (currentlyOpen != null && currentlyOpen != this)
        ////    currentlyOpen.HideDescription();

        //bool isOpening = !descriptionPanel.activeSelf;
        //descriptionPanel.SetActive(isOpening);

        //if (isOpening)
        //{
        //    // currentlyOpen = this; // Xóa
        //    ShowBlocker();

        //    // Thêm hình ảnh minh họa cho dễ hình dung luồng dữ liệu (nếu cần)
        //    //  -> Không cần thiết ở đây, code khá rõ.
        //}
        //else
        //{
        //    // currentlyOpen = null; // Xóa
        //    HideBlocker();
        //}
    }
}
