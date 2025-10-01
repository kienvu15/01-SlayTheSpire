using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Reference")]
    public CoinManager coinManager;

    [Header("UI References")]
    public GameObject eventPanel;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Image eventImageUI;    
    public Image backgroundImageUI;

    public Transform choiceButtonHolder;
    public GameObject choiceButtonPrefab;

    [Header("Data")]
    public EventDatabase eventDatabase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowRandomEvent();
    }

    public void ShowRandomEvent()
    {
        var data = eventDatabase.events[Random.Range(0, eventDatabase.events.Length)];
        ShowEvent(data);
    }

    public void ShowEvent(RoomEventData data)
    {
        eventPanel.SetActive(true);

        titleText.text = data.eventTitle;
        descriptionText.text = data.description;

        if (eventImageUI != null)
            eventImageUI.sprite = data.eventImage;

        if (backgroundImageUI != null)
            backgroundImageUI.sprite = data.BackgroundImage;

        // Xóa button cũ
        foreach (Transform child in choiceButtonHolder)
            Destroy(child.gameObject);

        // Tạo button theo choice
        foreach (var choice in data.choices)
        {
            var btnObj = Instantiate(choiceButtonPrefab, choiceButtonHolder);

            // text
            var btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = choice.choiceText;

            // ảnh button
            var btnImg = btnObj.GetComponent<Image>();
            if (btnImg != null && choice.choiceImage != null)
                btnImg.sprite = choice.choiceImage;

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyChoice(choice);
            });
        }
    }


    private void ApplyChoice(EventChoice choice)
    {
        Debug.Log($"Player chọn: {choice.choiceText}");

        // Nếu có outcome → random 1 kết quả và áp dụng
        if (choice.outcomes.Count > 0)
        {
            var outcome = GetRandomOutcome(choice.outcomes);
            Debug.Log($"Outcome: {outcome.resultType} ({outcome.value})");

            switch (outcome.resultType)
            {
                case EventResultType.Leave:
                    gameObject.SetActive(false);
                    break;

                case EventResultType.Gold:
                    coinManager.AddCoins(outcome.value);
                    break;

                case EventResultType.Damage:
                    // Player.Instance.hp -= outcome.value;
                    break;

                case EventResultType.Heal:
                    // Player.Instance.hp += outcome.value;
                    break;

                case EventResultType.Combat:
                    MapUIManager.Instance.ShowBattleCanvas();
                    break;

                case EventResultType.Shop:
                    MapUIManager.Instance.OpenShop();
                    break;

                case EventResultType.Relic:
                    // Inventory.AddRelic(outcome.value);
                    break;
            }

            // hiện text outcome
            descriptionText.text = outcome.resultText;
            eventImageUI.sprite = null;
        }

        // Sau khi thực hiện outcome → nếu có event tiếp theo thì nhảy tiếp
        if (choice.nextEvent != null)
        {
            ShowEvent(choice.nextEvent);
            return;
        }

        // Nếu không có outcome và không có nextEvent thì đóng panel
        // eventPanel.SetActive(false);
    }


    private EventOutcome GetRandomOutcome(List<EventOutcome> list)
    {
        float totalWeight = 0f;
        foreach (var o in list) totalWeight += o.weight;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var o in list)
        {
            cumulative += o.weight;
            if (roll <= cumulative)
                return o;
        }
        return list[list.Count - 1]; // fallback
    }


}
