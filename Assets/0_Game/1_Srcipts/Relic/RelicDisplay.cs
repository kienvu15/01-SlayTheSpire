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

}
