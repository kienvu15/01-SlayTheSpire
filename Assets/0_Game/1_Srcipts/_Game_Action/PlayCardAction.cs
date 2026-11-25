using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayCardAction : GameAction
{
    private readonly CardDisplay display;
    private readonly CardInstance card;
    private Character target;
    private readonly bool needsEnemy;

    public PlayCardAction(CardDisplay display, Character target, bool needsEnemy)
    {
        this.display = display;
        this.card = display.cardInstance;
        this.target = target;
        this.needsEnemy = needsEnemy;
    }

    public override IEnumerator Execute(ActionQueue q)
    {
        var match = q.match;

        // 1. Target có thể đã chết do lá trước trong queue -> đổi sang con đứng đầu hàng
        if (needsEnemy && (target == null || target.stats.currentHP <= 0))
            target = SlotManager.Instance.GetFrontEnemy();

        bool success = true;

        // target == null và cần enemy = hết enemy: bỏ qua effect, lá vẫn được discard
        if (!needsEnemy || target != null)
        {
            yield return CardEffectRunner.RunRoutine(
                card, match.player, needsEnemy ? target : match.player,
                q.mana, q.deck, match.player, match.enemySystem.enemies,
                ok => success = ok, q.gapBetweenEffects);
        }

        if (NewAttackImpactManager.Instance != null)
            yield return NewAttackImpactManager.Instance.WaitForAllImpacts(1.5f);

        // 2. Thất bại: hoàn mana, trả lá về tay
        if (!success)
        {
            q.mana.GainMana(card.ManaCost);
            q.deck.ReturnCardToHand(display.gameObject);
            yield break;
        }

        // 3. Vào discard (model) rồi bay animation, không chờ animation
        q.discard.AddDirect(card);
        PlayFlyToDiscard(q);
    }

    private void PlayFlyToDiscard(ActionQueue q)
    {
        if (display == null || q?.discard == null) return;

        RectTransform rect = display.GetComponent<RectTransform>();
        RectTransform pile = q.discard.GetComponent<RectTransform>();

        if (rect == null || pile == null || rect.parent == null) return;

        GameObject go = rect.gameObject;
        Transform parent = rect.parent;

        Vector3 start = rect.localPosition;
        Vector3 end = parent.InverseTransformPoint(pile.position);

        // Impact random như phiên bản ban đầu.
        Vector3 impact = end + new Vector3(
            -Random.Range(200f, 500f),
             Random.Range(200f, 500f),
             0f
        );

        // Pha 1: Đường cong bay tới impact.
        const float arcHeight = 450f;
        const float sideSwing = -250f;

        Vector3 control1 = (start + impact) * 0.5f
                         + new Vector3(sideSwing, arcHeight, 0f);

        // Hướng tiếp tuyến tại điểm kết thúc pha 1.
        Vector3 tangent = impact - control1;

        // Pha 2: Tiếp tục đường cong theo hướng tiếp tuyến.
        Vector3 control2 = impact + tangent * 0.35f;

        const float duration = 0.8f;
        const float arcPortion = 0.7f;
        const float popScale = 1.15f;

        Vector3 startScale = rect.localScale;

        Tween tween = DOVirtual.Float(0f, 1f, duration, p =>
        {
            if (rect == null) return;

            Vector3 position;

            if (p <= arcPortion)
            {
                // Pha 1: Bay vòng cung tới impact.
                float t = p / arcPortion;
                float u = 1f - t;

                position =
                    u * u * start +
                    2f * u * t * control1 +
                    t * t * impact;
            }
            else
            {
                // Pha 2: Cong mượt từ impact về discard.
                float t = (p - arcPortion) / (1f - arcPortion);
                float u = 1f - t;

                position =
                    u * u * u * impact +
                    3f * u * u * t * control2 +
                    3f * u * t * t * end +
                    t * t * t * end;
            }

            rect.localPosition = position;

            // Scale.
            float scale = p < 0.15f
                ? Mathf.Lerp(1f, popScale, p / 0.15f)
                : Mathf.Lerp(popScale, 0.1f, (p - 0.15f) / 0.85f);

            rect.localScale = startScale * scale;

        })
        .SetEase(Ease.Linear)
        .SetLink(go);

        tween.OnComplete(() =>
        {
            Object.Destroy(go);
        });
    }
}