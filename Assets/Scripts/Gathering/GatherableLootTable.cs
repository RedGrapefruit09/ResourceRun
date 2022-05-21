using System;
using ResourceRun.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceRun.Gathering
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> configuration of a loot table for a <see cref="Gatherable"/> object.
    /// </summary>
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "Game/Loot Table")]
    public class GatherableLootTable : ScriptableObject
    {
        [Tooltip("The entries of this loot table, each representing an item type")]
        public Entry[] entries;

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

        /// <summary>
        /// An entry in a <see cref="GatherableLootTable"/>, representing a type of item.
        /// </summary>
        [Serializable]
        public struct Entry
        {
            [Tooltip("The prefab of the item being dropped")]
            public GameObject prefab;
            [Tooltip("The minimum amount of that item to be dropped")]
            public int minAmount;
            [Tooltip("The maximum amount of that item to be dropped")]
            public int maxAmount;
        }
    }
}