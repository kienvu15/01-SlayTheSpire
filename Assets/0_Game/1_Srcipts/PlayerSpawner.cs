using TMPro;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] CardCollectionPanel cardCollectionPanel;
    [SerializeField] MapUIManager mapUIManager;
    [SerializeField] GameStage_Manager gameFlowManager;

    [Header("Player_Spawn_Pile")]
    [SerializeField] CardHolder cardHolder;
    [SerializeField] RelicManager relicManager;
    [SerializeField] ManaSystem manaSystem;
    [SerializeField] CoinManager coinManager;
    [SerializeField] Character_Info_UI character_Info_UI;
    [SerializeField] MidMatch_Manager midMatch_Manager;

    [Header("ToolTest")]
    [SerializeField] CharacterData defaultPlayerData;

    void Start()
    {
        CharacterData data = null;

        #region Get_data
        if (PlayerSelectionManager.Instance != null)
        {
            data = PlayerSelectionManager.Instance.GetSelectedPlayer();
        }
        if (data == null)
        {
            data = defaultPlayerData;
        }
        if (data == null)
        {
            Debug.LogError("No player data available!");
            return;
        }
        #endregion
        #region spawn_Prefab + reference_Player
        GameObject player = Instantiate(data.base_Prefab, transform.position, Quaternion.identity);

        Player playerRef = player.GetComponent<Player>();
        Character character = playerRef.GetComponent<Character>();

        playerRef.avatarImage.sprite = data.player_Avatar;

        playerRef.stats.maxHP = data.maxHP;
        playerRef.stats.currentHP = data.maxHP;
        playerRef.stats.shield = data.base_DEF;
        playerRef.stats.spd = data.base_SPD;
        //coinManager.currentCoin = data.startCoin;

        player.transform.parent = this.transform;
        RectTransform rect = player.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = data.local_Scale;
        #endregion
        #region Initialize_Card
        cardHolder.Initialize(data);
        #endregion
        #region Reference_RelicOwner + Initialize_Relic
        relicManager.owner = character;
        if (data.startRelic != null)
        {
            relicManager.AddRelic(data.startRelic);
        }
        #endregion
        #region Initialize_ManaSystem
        manaSystem.InitMana(data);
        #endregion
        #region Initialize_CoinManager
        coinManager.InitCoin(data);
        coinManager.UpdateUI();
        #endregion
        #region Init Stats Again
        playerRef.stats.handSize = data.handSize;
        character.Init_HP_UI();
        character_Info_UI.player = playerRef;
        character_Info_UI.Init_InfoUI(data);
        #endregion
        #region ref Player to midMatch_Manager
        midMatch_Manager.player = playerRef;
        midMatch_Manager.handSize = playerRef.stats.handSize;
        #endregion

        mapUIManager.manaSystem = manaSystem;
        mapUIManager.player = player.GetComponent<Player>();



        gameFlowManager.player = player.GetComponent<Player>();

    }
}
