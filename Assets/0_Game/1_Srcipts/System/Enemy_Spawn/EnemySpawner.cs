
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public MapEncounterDatabase encounterDatabase;
    public EnemySystem enemySystem;

    [Header("Base Prefab")]
    public GameObject baseEnemyPrefab;

    private EncounterData currentEncounter;
    private int currentWaveIndex;

    private void Start()
    {
        if (enemySystem != null)
            enemySystem.spawner = this;

        int battleCount = MapUIManager.Instance != null
            ? MapUIManager.Instance.battleRoomCount
            : 1;

        currentEncounter = encounterDatabase.GetEncounter(battleCount);
        currentWaveIndex = 0;

        if (currentEncounter != null)
            SpawnWave(currentWaveIndex);
        else
            Debug.LogError("[EnemySpawner] No valid Encounter!");
    }

    public void SpawnWave(int waveIndex)
    {
        if (currentEncounter == null ||
            currentEncounter.waves == null ||
            waveIndex < 0 ||
            waveIndex >= currentEncounter.waves.Length)
            return;

        WaveData wave = currentEncounter.waves[waveIndex];

        if (wave == null || wave.enemies == null)
            return;

        Spot[] slots = SlotManager.Instance.GetAllSlots();

        foreach (EnemySpawnData spawnData in wave.enemies)
        {
            if (spawnData == null || spawnData.enemyData == null)
                continue;

            int index = spawnData.slotIndex;

            if (index < 0 || index >= slots.Length)
                continue;

            Spot slot = slots[index];

            if (slot == null || slot.isOccupied)
                continue;

            GameObject enemyGO = Instantiate(
                baseEnemyPrefab,
                slot.transform
            );

            EnemyView enemy = enemyGO.GetComponent<EnemyView>();

            if (enemy == null)
            {
                Debug.LogError("[EnemySpawner] Missing EnemyView!");
                Destroy(enemyGO);
                continue;
            }

            // Truyền dữ liệu enemy
            enemy.Initialize(spawnData.enemyData);

            RectTransform enemyRect =
                enemyGO.GetComponent<RectTransform>();

            if (enemyRect != null)
            {
                enemyRect.localPosition = Vector3.zero;
                enemyRect.anchoredPosition = Vector2.zero;
            }

            EnemyDropZone dropZone =
                enemyGO.GetComponentInChildren<EnemyDropZone>();

            if (dropZone != null)
            {
                dropZone.deck = UIManager.Instance.deck;
                dropZone.discard = UIManager.Instance.discardScript;
            }

            slot.occupant = enemy;
            slot.isOccupied = true;
            enemy.currentSlot = slot;
        }

        enemySystem.RefreshEnemies();
    }

    public void NextWave()
    {
        if (currentEncounter == null)
            return;

        currentWaveIndex++;

        if (currentWaveIndex >= currentEncounter.waves.Length)
        {
            EndBattle();
            return;
        }

        SpawnWave(currentWaveIndex);
    }

    private void EndBattle()
    {
        UIManager.Instance.StartCoroutine(
            UIManager.Instance.AnimationButtonMoveAfterBattle()
        );

        MapUIManager.Instance?.HideBattleCanvas();
    }
}
