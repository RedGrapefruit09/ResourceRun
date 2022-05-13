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
        Func<GameObject, Vector3, GameObject> instantiateFunction,
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
            if (amount == 0) return;
            
            var itemObject = instantiateFunction.Invoke(entry.prefab, Vector3.zero);
            var item = itemObject.GetComponent<Item>();
            item.Amount = amount;
            itemObject.SetActive(false);
            
            var droppedItem = instantiateFunction.Invoke(droppedItemPrefab, pos).GetComponent<DroppedItem>();
            droppedItem.OriginalItem = item;
        }
    }
}