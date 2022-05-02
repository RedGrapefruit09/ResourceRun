using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerationStep : GenerationStep
{
    private readonly List<Vector2Int> _occupiedPositions = new List<Vector2Int>();

    public override void Generate()
    {
        _occupiedPositions.Clear();
        
        foreach (var objectGroup in generator.season.objectGroups)
        {
            GenerateGroup(objectGroup);
        }
    }

    private void GenerateGroup(ObjectGroup group)
    {
        var parentObject = new GameObject { name = group.groupName };
        generator.RegisterWorldObject(parentObject);

        for (var x = 0; x < generator.worldWidth; ++x)
        {
            for (var y = 0; y < generator.worldHeight; ++y)
            {
                var gridPos = new Vector2Int(x, y);
                if (_occupiedPositions.Contains(gridPos)) continue;

                var r = Random.Range(0, 1001);
                if (r > group.frequency) continue;

                var clone = Instantiate(group.basePrefab, parentObject.transform);
                clone.transform.position = CalculateObjectPosition(x, y);
                
                var sprite = GetRandomListElement(group.variants);
                clone.GetComponent<SpriteRenderer>().sprite = sprite;
                
                _occupiedPositions.Add(gridPos);
            }
        }
    }
}
