using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Recipe", menuName = "Game/Tool Recipe")]
public class ToolRecipe : ScriptableObject
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