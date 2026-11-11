using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;

    [Header("Stats")]
    public Vector2Int maxHP;
    public int base_DEF;
    public int base_SPD;

    [Header("Condition Status")]
    public int maxConditions = 2;

    [Header("Enemy_Prefab")]
    public GameObject base_Prefab;
    public Sprite enemy_Avatar;
    public Vector3 local_Scale;
    public Vector3 avatar_localScale;

    [Header("Enemy Type")]
    public bool isBoss;

    [Header("Movement")]
    public bool canBeMoved = true;   

    [Header("Action Pattern")]
    public List<EnemyAction> actionPatternAssets;
    public List<OverrideValues> overrideValuesPattern;

}
