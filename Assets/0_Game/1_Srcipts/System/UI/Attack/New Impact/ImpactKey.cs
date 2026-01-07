//using System;
//using UnityEngine;

//[Serializable]
//public struct ImpactKey : IEquatable<ImpactKey>
//{
//    public CardType cardType;
//    public ImpactEffectKind effectKind;

//    public bool Equals(ImpactKey other)
//    {
//        return cardType == other.cardType &&
//               effectKind == other.effectKind;
//    }

//    public override bool Equals(object obj)
//    {
//        return obj is ImpactKey other && Equals(other);
//    }

//    public override int GetHashCode()
//    {
//        unchecked
//        {
//            int hash = 17;
//            hash = hash * 23 + cardType.GetHashCode();
//            hash = hash * 23 + effectKind.GetHashCode();
//            return hash;
//        }
//    }
//}
