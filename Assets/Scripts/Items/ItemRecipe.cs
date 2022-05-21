using System;
using UnityEngine;

namespace ResourceRun.Items
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> configuration of a recipe for upgrading an old item into a new item.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item Recipe", menuName = "Game/Item Recipe")]
    public class ItemRecipe : ScriptableObject
    {
        [Tooltip("The prefab of the old item. Used for comparison purposes only")]
        public GameObject inputItem;
        [Tooltip("The prefab of the new item. Used for comparison purposes only")]
        public GameObject outputItem;
        [Tooltip("Additional ingredients required to convert from the old item to the new item")]
        public Ingredient[] ingredients;

        /// <summary>
        /// An ingredient requirement for using this recipe
        /// </summary>
        [Serializable]
        public struct Ingredient
        {
            [Tooltip("The prefab of the required item. Used for comparison purposes only")]
            public GameObject item;
            [Tooltip("The amount of the aforementioned required item to be present in the player's inventory in order to fulfill this ingredient requirement")]
            public int requirement;
        }
    }
}