using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopRoomDatabase", menuName = "Map/ShopRoomDatabase")]
public class ShopRoomDatabase : ScriptableObject
{
    public List<GameObject> shopRoomPrefabs = new List<GameObject>();

    public GameObject GetRandomShopRoom()
    {
        if (shopRoomPrefabs == null || shopRoomPrefabs.Count == 0) return null;
        return shopRoomPrefabs[Random.Range(0, shopRoomPrefabs.Count)];
    }
}