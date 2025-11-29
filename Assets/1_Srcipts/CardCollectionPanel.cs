using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCollectionPanel : MonoBehaviour
{
    [Header("References")]
    public CardHolder cardHolder;          
    public GameObject cardDisplayPrefab;   
    public Transform contentParent;       

    private List<GameObject> spawnedCards = new List<GameObject>();

    [Header("UI")]
    public Button removeButton; 

    private CardDisplay selectedCard;
    private bool isRemoveMode = false;

    public Deck deck;

    private void OnEnable()
    {
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        foreach (var obj in spawnedCards)
            Destroy(obj);
        spawnedCards.Clear();

        if (cardHolder == null || cardHolder.allCardData.Count == 0)
            return;

        foreach (CardData data in cardHolder.allCardData)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, contentParent);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            display.useScaleAnimation = true;
            if (display != null)
            {
                display.LoadCard(data);

                // Gán callback chọn card
                Button btn = newCard.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        selectedCard = display;
                    });
                }
            }

            spawnedCards.Add(newCard);
        }
    }

    // Mở panel theo mode
    public void Open(bool removeMode = false)
    {
        isRemoveMode = removeMode;
        gameObject.SetActive(true);
        RefreshPanel();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Gọi khi bấm nút Remove
    public void OnClickRemove()
    {
        if (selectedCard != null && cardHolder != null)
        {
            ConfirmPickReward();
            
        }
    }


    private void ConfirmPickReward()
    {
        if (selectedCard != null)
        {

            if (RandomCardSystem.Instance != null)
            {
            }

            RectTransform target = UIManager.Instance.deckIcon;

                UIManager.Instance.AnimateCardFly(
                    selectedCard.GetComponent<RectTransform>(),
                    target,
                    () => {
                        StartCoroutine(OnCardSelected(selectedCard));

                }
            );
            for (int i = 0; i < selectedCard.transform.childCount; i++)
            {
                selectedCard.transform.GetChild(i).gameObject.SetActive(false);
            }

            selectedCard.HideDescription();
        }
    }
    public IEnumerator OnCardSelected(CardDisplay pickedCard)
    {
        selectedCard.goldContainer.SetActive(false);
        UIManager.Instance.AnimateCardFly(
           pickedCard.GetComponent<RectTransform>(),
           UIManager.Instance.deckIcon,
           () =>
           {
               selectedCard = null;

           });
        yield return new WaitForSeconds(0.3f);

        cardHolder.RemoveCard(selectedCard.cardData);
        deck.RemoveFromDeck(selectedCard.cardData);
        CardDisplay di = selectedCard.GetComponent<CardDisplay>();
        if (di.descriptionPanel != null)
        {
            if (di.descriptionPanel.activeSelf)
            {
                Destroy(di.descriptionPanel);
            }
        }
        RefreshPanel();
    }

}
