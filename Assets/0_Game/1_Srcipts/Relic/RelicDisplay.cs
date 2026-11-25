using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject selected_Sprite;

    [Header("Button")]
    public Button relic_Button;


    [Header("Shop UI")]
    public GameObject goldContainer;
    public TextMeshProUGUI priceTXT;
    public GameObject soldOut;

    private Relic relicData;
    private RelicInstance relicInstance;
    private static bool firstTimeClick = false;
    private static RelicDisplay currentSelected;
    [HideInInspector] public NewShopSystem shopSystem;
    public Relic RelicData => relicData;

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
    }

    public void OnClickAddLisentShop()
    {
        relic_Button.onClick.AddListener(relic_ClickedShop);
    }
    public void relic_ClickedShop()
    {
        shopSystem.SelectRelic(this);

        if (shopSystem.IsRelicSelected(this)) // hoặc kiểm tra selected_Sprite.activeSelf
        {
            UIManager.Instance.RelicInfo_Help(true);
            UIManager.Instance.Init_RelicInforShop(relicInstance);
        }
    }
    public void SetSelected(bool value)
    {
        selected_Sprite.SetActive(value);
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

    public void SetSoldOut()
    {
        soldOut.SetActive(true);
        goldContainer.SetActive(false);
        Color color = iconImage.color;
        color.a = 0.5f;
        iconImage.color = color;
        relic_Button.onClick.RemoveAllListeners();
    }



}
