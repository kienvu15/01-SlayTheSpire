using UnityEngine;

public struct HitContext
{
    public CardType cardType;       
    public ImpactEffectKind effectKind;

    public Vector3 position;
    public Quaternion rotation;
    public Transform target;
}
