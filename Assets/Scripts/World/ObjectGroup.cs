using UnityEngine;

[CreateAssetMenu(fileName = "New Object Group", menuName = "World/Object Group")]
public class ObjectGroup : ScriptableObject
{
    public string groupName;
    public GameObject basePrefab;
    public int frequency;
    public Vector3 offset;
    public Sprite[] variants;
}
