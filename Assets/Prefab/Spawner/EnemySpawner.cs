using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public EncounterDatabase encounterDatabase;   // Chỉ tham chiếu 1 ScriptableObject
    public EnemySystem enemySystem;
    public EnemyDatabase enemyDatabase;

    private int currentEncounterIndex = 0;
    private int currentWaveIndex = 0;

    private void Start()
    {
        if (enemySystem != null)
        {
            enemySystem.spawner = this;
        }

        SpawnWave(currentEncounterIndex, currentWaveIndex);
    }

    /// <summary>
    /// Lấy 1 wave ngẫu nhiên từ database
    /// </summary>
    public WaveData GetRandomEncounter()
    {
        if (encounterDatabase == null ||
            encounterDatabase.encounters == null ||
            encounterDatabase.encounters.Length == 0)
        {
            Debug.LogError("[EnemySpawner] EncounterDatabase rỗng!");
            return null;
        }

        // chọn Encounter ngẫu nhiên
        EncounterData chosenEncounter = encounterDatabase.encounters[
            Random.Range(0, encounterDatabase.encounters.Length)
        ];

        if (chosenEncounter == null || chosenEncounter.waves == null || chosenEncounter.waves.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Encounter rỗng!");
            return null;
        }

        // chọn Wave ngẫu nhiên
        int randomWaveIndex = Random.Range(0, chosenEncounter.waves.Length);
        return chosenEncounter.waves[randomWaveIndex];
    }

    /// <summary>
    /// Spawn wave theo EncounterIndex + WaveIndex
    /// </summary>
    public void SpawnWave(int encounterIndex, int waveIndex)
    {
        if (encounterDatabase == null ||
            encounterDatabase.encounters == null ||
            encounterIndex >= encounterDatabase.encounters.Length)
        {
            Debug.Log("[EnemySpawner] Tất cả Encounter đã xong!");
            return;
        }

        EncounterData encounter = encounterDatabase.encounters[encounterIndex];
        if (encounter == null || encounter.waves == null || waveIndex >= encounter.waves.Length)
        {
            Debug.Log("[EnemySpawner] Encounter hoặc wave rỗng!");
            return;
        }

        WaveData wave = encounter.waves[waveIndex];

        // === GIỮ NGUYÊN LOGIC SLOT & MOVE TO SLOT ===
        GameObject systemCanvas = GameObject.Find("EnemyFM");
        if (systemCanvas == null)
        {
            Debug.LogError("Không tìm thấy Canvas tên 'EnemyFM'!");
            return;
        }

        Spot[] slots = SlotManager.Instance.GetAllSlots();

        // Lấy rule-based slot assignment
        Dictionary<EnemyType, Spot> slotMap = SlotAssigner.AssignSlots(wave.enemyTypes, slots);

        foreach (EnemyType type in wave.enemyTypes)
        {
            Spot slot = slotMap[type];
            if (slot == null) continue;

            GameObject prefab = enemyDatabase.GetRandomPrefab(type);
            if (prefab == null) continue;

            GameObject enemyGO = Instantiate(prefab, systemCanvas.transform);
            RectTransform enemyRect = enemyGO.GetComponent<RectTransform>();

            // reset về 0 để tránh bị lệch layer
            Vector3 localPos = enemyRect.localPosition;
            localPos.z = 0;
            enemyRect.localPosition = localPos;

            // ban đầu để giữa, Move2Slot sẽ kéo về đúng slot
            enemyRect.anchoredPosition = Vector2.zero;

            EnemyView enemy = enemyGO.GetComponent<EnemyView>();
            if (enemy != null)
            {
                slot.occupant = enemy;
                slot.isOccupied = true;
                // vẫn giữ nguyên coroutine di chuyển vào slot
                StartCoroutine(enemy.Move2Slot(slot));
            }
        }

        enemySystem.RefreshEnemies();
    }

    /// <summary>
    /// Sang wave tiếp theo, nếu hết thì sang Encounter mới
    /// </summary>
    public void NextWave()
    {
        currentWaveIndex++;

        if (currentEncounterIndex >= encounterDatabase.encounters.Length)
        {
            Debug.Log("[EnemySpawner] Không còn Encounter nào!");
            return;
        }

        // nếu hết wave thì sang Encounter mới
        if (currentWaveIndex >= encounterDatabase.encounters[currentEncounterIndex].waves.Length)
        {
            currentEncounterIndex++;
            currentWaveIndex = 0;
        }

        SpawnWave(currentEncounterIndex, currentWaveIndex);
    }
}
