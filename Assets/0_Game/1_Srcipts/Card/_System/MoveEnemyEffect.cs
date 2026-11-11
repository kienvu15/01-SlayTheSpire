using UnityEngine;

public enum EnemyMoveDirection { KnockBack, PullForward }

[CreateAssetMenu(menuName = "Cards/Effects/Move Enemy")]
public class MoveEnemyEffect : EffectData
{
    public EnemyMoveDirection direction = EnemyMoveDirection.KnockBack;
    public int steps = 1;   // override value = số slot

    public override bool RequiresTarget => true;

    public override bool Apply(EffectContext ctx)
    {
        var enemy = ctx.target as EnemyView;
        if (enemy == null) return true;   // không có enemy hợp lệ: bỏ qua, lá vẫn được đánh

        int count = Mathf.Max(1, ctx.GetValue(steps));
        int signed = direction == EnemyMoveDirection.KnockBack ? count : -count;

        if (!enemy.CanBeMoved)
        {
            Debug.Log($"[MoveEnemy] {enemy.name} không thể bị di chuyển");
            return true;   // miễn nhiễm vẫn tính là đã đánh
        }

        SlotManager.Instance.MoveEnemy(enemy, signed);
        return true;
    }
}