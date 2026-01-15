using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] RelicUIManager relicUIManager;
    [SerializeField] Deck deckBuilder;
    [SerializeField] MapUIManager mapUIManager;

    [SerializeField] TextMeshProUGUI manaText;

    void Start()
    {
        var data = PlayerSelectionManager.Instance.GetSelectedPlayer();

        if (data == null)
        {
            Debug.LogError("No player selected!");
            return;
        }

        GameObject player = Instantiate(
            data.playerPrefab,
            transform.position,
            Quaternion.identity
        );

        player.transform.parent = this.transform;
        RectTransform rect = player.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        CardHolder cardHolder = player.GetComponentInChildren<CardHolder>();
        deckBuilder.cardHolder = cardHolder;

        RelicManager relicManager = player.GetComponentInChildren<RelicManager>();
        relicUIManager.relicManager = relicManager;

        ManaSystem manaSystem = player.GetComponentInChildren<ManaSystem>();
        manaSystem.ManaText = manaText;

        mapUIManager.manaSystem = manaSystem;
        mapUIManager.player = player.GetComponent<Player>();

        // Init stats
        Player p = player.GetComponent<Player>();
        p.stats.maxHP = data.maxHP;
        p.stats.currentHP = data.maxHP;

        // Add start relic
        if (data.startRelic != null)
        {
            p.relicManager.AddRelic(data.startRelic);
        }
    }
}
