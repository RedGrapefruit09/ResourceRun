using System;
using UnityEngine;

namespace ResourceRun.Items
{
    [CreateAssetMenu(fileName = "New Item Recipe", menuName = "Game/Item Recipe")]
    public class ItemRecipe : ScriptableObject
    {
        public GameObject inputItem;
        public GameObject outputItem;
        public Ingredient[] ingredients;

        [Serializable]
        public struct Ingredient
        {
            public GameObject item;
            public int requirement;
        }
    }
}