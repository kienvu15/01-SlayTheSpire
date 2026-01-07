using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public List<EnemyView> enemies = new List<EnemyView>();
    public EnemySpawner spawner;

    private void Start()
    {
        RefreshEnemies();
    }

    public void RefreshEnemies()
    {
        enemies.Clear();
        foreach (var slot in SlotManager.Instance.GetAllSlots())
        {
            if (slot.isOccupied && slot.occupant != null)
            {
                enemies.Add(slot.occupant);
            }
        }
    }

    public IEnumerator OnPlayerEndTurn(Player player)
    {
        yield return StartCoroutine(EnemyTurn(player));
    }


    public IEnumerator EnemyTurn(Player player)
    {
        Debug.Log("[EnemySystem] Enemy turn start");

        Spot[] slots = SlotManager.Instance.GetAllSlots();

        for (int i = 0; i < slots.Length; i++)
        {
            Spot slot = slots[i];
            if (slot.isOccupied && slot.occupant != null)
            {
                EnemyView enemy = slot.occupant;
                enemy.PerformAction(player);
                yield return new WaitForSeconds(0.5f);
            }
        }

        Debug.Log("[EnemySystem] Enemy turn end");
    }


    public void OnEnemyDied(EnemyView enemy)
    {
        if (enemy.currentSlot != null)
        {
            enemy.currentSlot.isOccupied = false;
            enemy.currentSlot.occupant = null;
        }

        enemies.Remove(enemy);
        StartCoroutine(CheckWaveClear());
    }


    private IEnumerator CheckWaveClear()
    {
        yield return new WaitForSeconds(0.3f);

        RefreshEnemies();
        if (enemies.Count == 0)
        {
            Debug.Log("[EnemySystem] Wave cleared!");
            spawner.NextWave();
        }
    }
}
