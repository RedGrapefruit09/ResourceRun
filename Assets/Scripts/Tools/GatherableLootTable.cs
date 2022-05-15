using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Game/Loot Table")]
public class GatherableLootTable : ScriptableObject
{
    public Entry[] entries;

    [Serializable]
    public struct Entry
    {
        public GameObject prefab;
        public int minAmount;
        public int maxAmount;
    }
    
    public void Generate(
        GameObject droppedItemPrefab,
        Vector3 basePos,
        float minXSpread,
        float minYSpread,
        float maxXSpread,
        float maxYSpread)
    {
        foreach (var entry in entries)
        {
            var xSpread = Random.Range(minXSpread, maxXSpread);
            var ySpread = Random.Range(minYSpread, maxYSpread);
            var pos = new Vector3(basePos.x + xSpread, basePos.y + ySpread);
            
            var amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            if (amount == 0) continue;
            
            var item = Item.Create(entry.prefab, amount);
            Item.Drop(droppedItemPrefab, pos, item);
        }
    }
}