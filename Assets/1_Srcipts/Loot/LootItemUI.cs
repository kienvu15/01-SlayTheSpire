using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text nameText;
    public Button clickButton; 

    public LootData myLootData;

    public void Init(LootData data)
    {
        myLootData = data;

        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.name;

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(OnLootClicked);
    }

    private void OnLootClicked()
    {
        switch (myLootData.lootType)
        {
            case LootType.Gold:
                CollectGold();
                CheckAndDestroy(); 
                break;

            case LootType.Card:
                CollectCard();
                break;

            case LootType.Relic:
                CollectRelic();
                CheckAndDestroy(); 
                break;
        }
    }

    private void CheckAndDestroy()
    {
        ImmediateCheckHolder();
        Destroy(gameObject);
    }

    private void ImmediateCheckHolder()
    {
        if (UIManager.Instance.lootHolder == null) return;
        int remainingLoot = UIManager.Instance.lootHolder.transform.childCount;
        if (remainingLoot <= 1)
        {
            Debug.Log("All loot collected. Deactivating loot panel.");
            UIManager.Instance.DeactiveLootPanel(); 
        }
    }

    private void CollectGold()
    {
        SoundManager.Instance.Play("Coin");
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(myLootData.amount);
        }
        UIManager.Instance.AnimationGold();
    }

    private void CollectCard()
    {
        SoundManager.Instance.Play("SelectButton");
        UIManager.Instance.ActivePickMeCardPanel();
        if (RandomCardSystem.Instance != null)
        {
            RandomCardSystem.Instance.GeneratePickMeCard(this);
        }
    }

    private void CollectRelic()
    {
        SoundManager.Instance.Play("Coin");

        if (RelicManager.Instance != null && myLootData.relic != null)
        {
            RelicManager.Instance.AddRelic(myLootData.relic);
        }
    }

    public void CheckHolder()
    {
        Invoke(nameof(InternalCheckHolder), 0.01f);
    }

    private void InternalCheckHolder()
    {
        if (UIManager.Instance.lootHolder == null) return;
        int remainingLoot = UIManager.Instance.lootHolder.transform.childCount;

        if (remainingLoot <= 0)
        {
            Debug.Log("All loot collected. Deactivating loot holder.");
            UIManager.Instance.DeactiveLootPanel(); 
        }
    }

    public void FinalizeLooting()
    {
        Destroy(gameObject);
        UIManager.Instance.CheckAndCloseLootPanel(UIManager.Instance.lootHolder.transform);
    }
}