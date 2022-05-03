using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Group", menuName = "World/Object Group")]
public class ObjectGroup : ScriptableObject
{
    public string groupName;
    public GameObject basePrefab;
    public int frequency;
    public Vector3 offset;
    public List<ObjectVariant> variants;
}

[Serializable]
public class ObjectVariant
{
    public int frequency;
    public Sprite sprite;
    public Vector2 colliderSize = Vector2.one;
}
