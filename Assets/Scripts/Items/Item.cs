using System.Text;
using ResourceRun.Player;
using UnityEngine;

namespace ResourceRun.Items
{
    /// <summary>
    /// The base class for all existent item instances in the <see cref="PlayerInventory"/>.
    /// </summary>
    public abstract class Item : MonoBehaviour
    {
        [Header("Item Settings")]
        [Tooltip("The maximum amount of this item that can be stored in a given inventory slot")]
        public int maxCount;
        [Tooltip("The distinguishing label of any instance of this item. Used for item comparison operations internally")]
        public string label;

        /// <summary>
        /// The amount of this item stored in this given item instance.
        /// </summary>
        public int Amount { get; private set; } = 1;

        /// <summary>
        /// A message called when this item is selected in the inventory by the player.
        /// Typically, the item should be shown visually here.
        /// </summary>
        public abstract void OnSelected();

        /// <summary>
        /// A message called when this gets deselected in the inventory by the player.
        /// Typically, the item should be hidden visually here.
        /// </summary>
        public abstract void OnDeselected();

        /// <summary>
        /// Builds out the tooltip (item usage hint) for this item instance.
        /// This is called every game tick, so dynamic data, such as tool durability, that can change, can be put in here.
        /// To compile the result of calling this method into a plain <see langword="string"/> for rendering, use the <see cref="GetTooltip"/>
        /// method.
        /// </summary>
        /// <param name="tooltip">The <see cref="StringBuilder"/> to build out this tooltip</param>
        public abstract void BuildTooltip(StringBuilder tooltip);

        /// <summary>
        /// Calls <see cref="BuildTooltip"/> and compiles its result into a simple <see langword="string"/>, that may contain <code>\n</code>
        /// line breaks.
        /// </summary>
        /// <returns>The compiled result in the form of a plain <see langword="string"/></returns>
        public string GetTooltip()
        {
            var tooltip = new StringBuilder();
            BuildTooltip(tooltip);
            return tooltip.ToString();
        }

        /// <summary>
        /// Increments the amount of this item instance by a certain margin.
        /// </summary>
        /// <param name="value">By how much exactly should the amount be increased</param>
        public void Increment(int value = 1)
        {
            Amount += value;
        }

        /// <summary>
        /// Decrements the amount of this item instance by a certain margin.
        /// </summary>
        /// <param name="value">By how much exactly should the amount be decreased</param>
        public void Decrement(int value = 1)
        {
            Amount -= value;
        }

        /// <summary>
        /// Compares the given two items for <see langword="null"/> equality or <see cref="Item.label"/> equality.
        /// </summary>
        /// <param name="first">The first item</param>
        /// <param name="second">The second item</param>
        /// <returns>Whether the two given items are of the same item type</returns>
        public static bool Same(Item first, Item second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;

            return first.label == second.label;
        }

        /// <summary>
        /// An inversion of the <see cref="Same"/> operation, giving the exact opposite results.
        /// </summary>
        /// <param name="first">The first item</param>
        /// <param name="second">The second item</param>
        /// <returns>Whether the two given items are completely different</returns>
        public static bool Different(Item first, Item second)
        {
            return !Same(first, second);
        }

        /// <summary>
        /// Instantiates a new item instances of the given amount from the given Unity Prefab and fully initializes its state.
        /// </summary>
        /// <param name="prefab">The source prefab <see cref="GameObject"/> for instantiating this new item</param>
        /// <param name="amount">The amount of the item that should be created</param>
        /// <returns>The <see cref="Item"/> script reference of this newly created item <see cref="GameObject"/></returns>
        public static Item Create(GameObject prefab, int amount = 1)
        {
            var clone = Instantiate(prefab);
            var item = clone.GetComponent<Item>();
            item.Amount = amount;
            item.OnDeselected();
            return item;
        }

        /// <summary>
        /// Instantiates a new instance of an item by calling <see cref="Create"/> and inserts that newly created instance into the given
        /// <see cref="PlayerInventory"/>.
        /// </summary>
        /// <param name="prefab">The source prefab <see cref="GameObject"/> for the new item</param>
        /// <param name="inventory">The <see cref="PlayerInventory"/> for it to be inserted into</param>
        /// <param name="amount">The amount of that item that should be created</param>
        public static void CreateAndInsert(GameObject prefab, PlayerInventory inventory, int amount = 1)
        {
            inventory.InsertItem(Create(prefab, amount));
        }

        /// <summary>
        /// Drops the given item on the ground in a form of a <see cref="DroppedItem"/> wrapper for the original item instance.
        /// </summary>
        /// <param name="droppedItemPrefab">A prefab for a generic dropped item instance</param>
        /// <param name="pos">The world position to spawn the dropped item in</param>
        /// <param name="item">The item instance to be dropped</param>
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