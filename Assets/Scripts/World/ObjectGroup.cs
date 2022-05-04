using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An <see cref="ObjectGroup"/> is a <see cref="ScriptableObject"/> that represents a group of objects with
/// shared properties. For example, ores, trees or bushes.
/// </summary>
[CreateAssetMenu(fileName = "New Object Group", menuName = "World/Object Group")]
public class ObjectGroup : ScriptableObject
{
    [Tooltip("The shared prefab GameObject to act as a base for every object in this group")]
    public GameObject basePrefab;
    [Tooltip("The X-in-a-1000 chance of an object from this ObjectGroup to be generated")]
    public int frequency;
    [Tooltip("The shared positional offset of this ObjectGroup. This applies to world position, not grid position!")]
    public Vector3 offset;
    [Tooltip("All ObjectVariants in this ObjectGroup")]
    public List<ObjectVariant> variants;
}

/// <summary>
/// An <see cref="ObjectVariant"/> is a serializable structure containing unique properties of an object inside
/// an <see cref="ObjectGroup"/>.
/// </summary>
[Serializable]
public class ObjectVariant
{
    [Tooltip("The relative weight of this variant being generated. A higher weight means a higher chance of being picked out")]
    public int weight;
    [Tooltip("The Sprite that resembles this variant")]
    public Sprite sprite;
    [Tooltip("The size of the BoxCollider2D of this variant. If a BoxCollider2D is not present on the prefab, this will simply be ignored")]
    public Vector2 colliderSize = Vector2.one;
}
