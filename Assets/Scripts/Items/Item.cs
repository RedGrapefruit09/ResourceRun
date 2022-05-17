using ResourceRun.Player;
using UnityEngine;

namespace ResourceRun.Items
{
    public abstract class Item : MonoBehaviour
    {
        [Header("Item Settings")] public int maxCount;

        public string label;

        public int Amount { get; set; } = 1;

        public abstract void OnSelected();

        public abstract void OnDeselected();

        public abstract void BuildTooltip(ItemTooltip tooltip);

        public string GetTooltip()
        {
            var tooltip = new ItemTooltip();
            BuildTooltip(tooltip);
            return tooltip.Get();
        }

        public void Increment(int value = 1)
        {
            Amount += value;
        }

        public void Decrement(int value = 1)
        {
            Amount -= value;
        }

        public static bool Same(Item first, Item second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;

            return first.label == second.label;
        }

        public static bool Different(Item first, Item second)
        {
            return !Same(first, second);
        }

        public static Item Create(GameObject prefab, int amount = 1)
        {
            var clone = Instantiate(prefab);
            var item = clone.GetComponent<Item>();
            item.Amount = amount;
            item.OnDeselected();
            return item;
        }

        public static void CreateAndInsert(GameObject prefab, PlayerInventory inventory, int amount = 1)
        {
            inventory.InsertItem(Create(prefab, amount));
        }

        public static void Drop(GameObject droppedItemPrefab, Vector3 pos, Item item)
        {
            item.gameObject.SetActive(false);
            item.OnDeselected();
            var clone = Instantiate(droppedItemPrefab, pos, Quaternion.identity);
            var droppedItem = clone.GetComponent<DroppedItem>();
            droppedItem.OriginalItem = item;
        }
    }
}