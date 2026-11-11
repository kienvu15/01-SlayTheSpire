using UnityEngine;
using System.Collections.Generic;

public class EffectContext
{
    public Character self;
    public Character target;
    public ManaSystem manaSystem;
    public Deck deck;

    public Discard discard;
    public CardPileViewer cardPileViewer;
    public ImpactProfile impactProfile;

    public CardInstance sourceCard;

    public int level = 1;           
    public bool hasOverride;        
    public int overrideValue;

    public bool lastHit = true;

    public Character player;                
    public List<EnemyView> enemies;         
    public HashSet<Character> hitTargets = new HashSet<Character>();

    public int GetValue(int baseValue) => hasOverride ? overrideValue : baseValue;
}

public abstract class EffectData : ScriptableObject
{
    public virtual bool RequiresTarget => false;
    public virtual bool CountsAsIntend => false;

    // true = lá bài chơi hợp lệ
    public abstract bool Apply(EffectContext ctx);

    // Cho UI intent/tooltip
    public virtual int GetIntentValue(EffectContext ctx) => 0;
}