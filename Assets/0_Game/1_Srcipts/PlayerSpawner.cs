using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
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
