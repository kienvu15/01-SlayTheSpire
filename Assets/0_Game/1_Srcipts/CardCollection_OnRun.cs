using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCollection_OnRun : MonoBehaviour
{
    [Header("References")]
    public Deck deck;
    public GameObject cardDisplayPrefab;
    public GameObject conditionPrefab;
    public Transform contentParent;
    public Transform conditionParent;
    public ConditionIconDatabase conditionDb;

    [Header("UI")]
    public Button removeButton; 

    private CardDisplay selectedCard;
    private bool isRemoveMode = false;

    public static bool firstTimeOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>();
    private List<GameObject> spawnedConditions = new List<GameObject>();
    
    private void OnEnable()
    {
        RefreshPanel();
    }
    private void OnDisable() => ClearConditions();

    public void RefreshPanel()
    {
        foreach (var obj in spawnedCards)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedCards.Clear();

        if (deck == null) return;

        List<CardInstance> allCards = deck.GetAllCards();

        foreach (CardInstance instance in allCards)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, contentParent);
            CardDisplay display = newCard.GetComponent<CardDisplay>();
            if (display == null) continue;

            Canvas canvas = newCard.GetComponent<Canvas>();
            if (canvas != null)
                canvas.overrideSorting = false;

            display.LoadCard(instance);   // truyền instance để giữ level

            Button btn = newCard.GetComponentInChildren<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnCard_Clicked(display));

            spawnedCards.Add(newCard);
        }
    }

    public void OnCard_Clicked(CardDisplay pickedCard)
    {
        UIManager.Instance.CardInfo_Help(true);
        firstTimeOpen = true;

        ShowConditions(pickedCard.cardInstance);
        UIManager.Instance.Init_CardInfor(pickedCard);
    }

    private void ShowConditions(CardInstance card)
    {
        ClearConditions();

        if (card == null || conditionPrefab == null || conditionParent == null || conditionDb == null)
            return;

        foreach (ConditionType type in card.data.GetKeywords(card.level))
        {
            if (!conditionDb.TryGet(type, out var entry)) continue;

            GameObject go = Instantiate(conditionPrefab, conditionParent);
            go.GetComponent<ConditionInfoItem>().Setup(
                conditionDb.GetDisplayName(type),
                entry.description,
                DescriptionFormatter.GetColor(type),
                entry.icon);

            spawnedConditions.Add(go);
        }
    }

    public void ClearConditions()
    {
        foreach (var go in spawnedConditions)
            if (go != null) Destroy(go);
        spawnedConditions.Clear();
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
        //if (selectedCard != null && cardHolder != null)
        //{
        //    ConfirmPickReward();
        //}
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

           // selectedCard.HideDescription();
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

        //cardHolder.RemoveCard(selectedCard.cardData);
        deck.RemoveFromDeck(selectedCard.cardData);
        CardDisplay di = selectedCard.GetComponent<CardDisplay>();
        //if (di.descriptionPanel != null)
        //{
        //    if (di.descriptionPanel.activeSelf)
        //    {
        //        Destroy(di.descriptionPanel);
        //    }
        //}
        RefreshPanel();
    }

}
