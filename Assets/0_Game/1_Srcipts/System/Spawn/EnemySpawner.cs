using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public EncounterDatabase encounterDatabase;
    public EnemySystem enemySystem;
    public EnemyDatabase enemyDatabase;

    private EncounterData currentEncounter;
    private int currentWaveIndex = 0;

    private void Start()
    {
        if (enemySystem != null)
            enemySystem.spawner = this;

        int count = MapUIManager.Instance != null ? MapUIManager.Instance.battleRoomCount : 0;
        currentEncounter = encounterDatabase.GetEncounterForStage(count);
        currentWaveIndex = 0;

        if (currentEncounter != null)
            SpawnWave(currentWaveIndex);
        else
            Debug.LogError("[EnemySpawner] Không có Encounter hợp lệ!");
    }

    public void SpawnWave(int waveIndex)
    {
        if (currentEncounter == null ||
            currentEncounter.waves == null ||
            waveIndex >= currentEncounter.waves.Length)
        {
            Debug.Log("[EnemySpawner] Encounter hoặc wave rỗng!");
            return;
        }

        WaveData wave = currentEncounter.waves[waveIndex];
        if (wave == null) return;

        GameObject systemCanvas = GameObject.Find("EnemyFM");
        if (systemCanvas == null)
        {
            Debug.LogError("Không tìm thấy Canvas tên 'EnemyFM'!");
            return;
        }

        Spot[] slots = SlotManager.Instance.GetAllSlots();
        List<(EnemyType type, Spot slot)> assigned = SlotAssigner.AssignSlots(wave.enemyTypes, slots);

        foreach (var pair in assigned)
        {
            EnemyType type = pair.type;
            Spot slot = pair.slot;
            if (slot == null) continue;

            List<GameObject> candidates = enemyDatabase.enemyPrefabs
                .FindAll(p => p.GetComponent<EnemyView>()?.enemyType == type);

            if (candidates.Count == 0) continue;

            GameObject prefab = candidates[Random.Range(0, candidates.Count)];
            if (prefab == null) continue;

            GameObject enemyGO = Instantiate(prefab, systemCanvas.transform);
            EnemyDropZone dropZone = enemyGO.GetComponentInChildren<EnemyDropZone>();
            dropZone.deck = UIManager.Instance.deck;
            dropZone.discard = UIManager.Instance.discardScript;
            RectTransform enemyRect = enemyGO.GetComponent<RectTransform>();

            Vector3 localPos = enemyRect.localPosition;
            localPos.z = 0;
            enemyRect.localPosition = localPos;
            enemyRect.anchoredPosition = Vector2.zero;

            EnemyView enemy = enemyGO.GetComponent<EnemyView>();
            if (enemy != null)
            {
                slot.occupant = enemy;
                slot.isOccupied = true;
                StartCoroutine(enemy.Move2Slot(slot));
            }
        }

        enemySystem.RefreshEnemies();
    }

    /// Chuyển sang wave tiếp theo
    public void NextWave()
    {
        if (currentEncounter == null)
        {
            Debug.Log("[EnemySpawner] Không có Encounter!");
            EndBattle();
            return;
        }

        currentWaveIndex++;

        // Nếu đã hết wave trong encounter hiện tại → kết thúc
        if (currentWaveIndex >= currentEncounter.waves.Length)
        {
            Debug.Log("[EnemySpawner] Encounter đã xong!");
            EndBattle();
            return;
        }

        // còn wave thì spawn tiếp
        SpawnWave(currentWaveIndex);
    }

    private void EndBattle()
    {
        GameFlowManager.Instance.isOnBattle = false;
        if(GameFlowManager.Instance.isOnBattle == false)
        {
            Debug.Log("Go go");
        }
        UIManager.Instance.StartCoroutine(UIManager.Instance.AnimationButtonMoveAfterBattle());

        GameFlowManager.Instance.StartCoroutine(GameFlowManager.Instance.AfterBattleEvent());
        MapUIManager.Instance?.HideBattleCanvas();
    }
}
