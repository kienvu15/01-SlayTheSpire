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

        List<CardData> allCards = deck.GetAllCards();

        foreach (CardData data in allCards)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, contentParent);
            CardDisplay display = newCard.GetComponent<CardDisplay>();

            Canvas canvas = newCard.GetComponent<Canvas>();
            canvas.overrideSorting = false;

            if (display == null) continue;
            //display.useScaleAnimation = true;
            display.LoadCard(data);
            Button btn = newCard.GetComponentInChildren<Button>();
            if (btn != null)
            {
                //CardDisplay capturedDisplay = display;
                //btn.onClick.AddListener(() =>
                //{
                //    selectedCard = capturedDisplay;
                //});
                btn.onClick.AddListener(() => OnCard_Clicked(display));
            }
            spawnedCards.Add(newCard);
        }
    }

    public void OnCard_Clicked(CardDisplay pickedCard)
    {
        string Name = pickedCard.cardData.cardName;
        string Description = pickedCard.cardData.description;
        string manaCost = pickedCard.cardData.manaCost.ToString();

        //if (firstTimeOpen == false)
        //{
            UIManager.Instance.CardInfo_Help(true);
            firstTimeOpen = true;
        //}
        ShowConditions(pickedCard.cardData);
        UIManager.Instance.Init_CardInfor(pickedCard);
    }

    private void ShowConditions(CardData data)
    {
        ClearConditions();

        if (conditionPrefab == null || conditionParent == null || conditionDb == null) return;

        foreach (ConditionType type in data.GetKeywords())
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
