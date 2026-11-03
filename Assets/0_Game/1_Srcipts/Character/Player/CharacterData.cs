using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Info")]
    public string playerName;
    public Sprite playerSplashArt;
    [TextArea(3, 6)]
    public string lore;

    [Header("Stats")]
    public int maxHP;
    public int base_DEF;
    public int base_SPD;

    [Header("Coin")]
    public int startCoin;

    [Header("Mana")]
    public int maxMana;
    public int startMana;
    public int manaPerTurn;

    [Header("Condition Status")]
    public int maxConditions = 2;

    [Header("Start Deck")]
    public int handSize;
    public Character_Deck_Setup startDeck;

    [Header("Start Relic")]
    public Relic startRelic;

    [Header("Player_Prefab")]
    public GameObject base_Prefab;
    public Sprite player_Avatar;
    public Sprite player_AvatarIcon;
    public Vector3 local_Scale;

}
