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
    public Sprite enemySprite;
    public Vector3 rotation;
    public float CanvasHieght;
    public float hpSlider_Hieght = 250;
    public Vector2 hpSlider_Position = new Vector2(0.5f,0);
    public Vector2 CardDrop;
    public Vector2 CardDropPosition;
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
