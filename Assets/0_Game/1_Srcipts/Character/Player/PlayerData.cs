using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Info")]
    public string playerName;
    public Sprite playerSplashArt;
    [TextArea(3, 6)]
    public string lore;

    [Header("Stats")]
    public int maxHP;

    [Header("Start Relic")]
    public Relic startRelic;

    [Header("Prefab")]
    public GameObject playerPrefab;
}
