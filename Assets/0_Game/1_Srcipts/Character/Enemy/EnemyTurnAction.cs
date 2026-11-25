using System.Collections;
using UnityEngine;

public class EnemyTurnAction : GameAction
{
    public override IEnumerator Execute(ActionQueue q)
    {
        var enemySystem = q.match.enemySystem;
        var player = q.match.player;

        // Start turn: trigger condition của enemy
        foreach (var e in enemySystem.enemies.ToArray())
            if (e != null && e.stats.currentHP > 0) e.TriggerConditionEffects();

        // Mỗi enemy hành động theo thứ tự slot (lấy lại danh sách mỗi lượt vì có thể bị đổi chỗ/chết)
        foreach (var e in SlotManager.Instance.GetEnemiesInOrder())
        {
            if (e == null || e.stats.currentHP <= 0) continue;
            if (player.stats.currentHP <= 0) break;

            if (e.isStunned) { e.TickStun(); continue; }   // xem lưu ý 2

            e.PerformAction(player);
            yield return new WaitForSeconds(0.3f);                                     // nhịp tối thiểu (animation Attack)
            yield return NewAttackImpactManager.Instance.WaitForAllImpacts(1.5f);
            yield return new WaitForSeconds(0.2f);                                     // thở một chút
        }

        // End turn: giảm condition
        foreach (var e in enemySystem.enemies.ToArray())
        {
            if (e == null) continue;
            e.DecreasePlayerConditions();
            e.DecreaseEnemySelfConditions();
        }
        player.DecreasePlayerSelfConditions();
    }
}