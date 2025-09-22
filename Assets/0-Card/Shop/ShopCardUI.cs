using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Image icon;
    public Button buyButton;

    private CardData card;
    private ShopSystem shop;

    public void Setup(CardData newCard, ShopSystem shopSystem)
    {
        card = newCard;
        shop = shopSystem;

        nameText.text = card.cardName;
        
        buyButton.onClick.AddListener(() => shop.BuyCard(card));
    }
}
