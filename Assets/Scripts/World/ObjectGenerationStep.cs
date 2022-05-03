using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerationStep : GenerationStep
{
    [SerializeField] private int spawnArea;
    [SerializeField] private int excludedEdgeArea;
    
    private readonly List<Vector2Int> _occupiedPositions = new List<Vector2Int>();

    public override void Generate()
    {
        _occupiedPositions.Clear();
        OccupySpawnArea();
        
        foreach (var objectGroup in generator.season.objectGroups)
        {
            GenerateGroup(objectGroup);
        }
    }

    private void OccupySpawnArea()
    {
        var centerX = generator.worldWidth / 2;
        var centerY = generator.worldHeight / 2;

        for (var x = centerX - spawnArea; x < centerX + spawnArea; ++x)
        {
            for (var y = centerY - spawnArea; y < centerY + spawnArea; ++y)
            {
                _occupiedPositions.Add(new Vector2Int(x, y));
            }
        }
    }
    
    private void GenerateGroup(ObjectGroup group)
    {
        var parentObject = new GameObject { name = group.groupName };
        generator.RegisterWorldObject(parentObject);

        for (var x = excludedEdgeArea; x < generator.worldWidth - excludedEdgeArea; ++x)
        {
            for (var y = excludedEdgeArea; y < generator.worldHeight - excludedEdgeArea; ++y)
            {
                var gridPos = new Vector2Int(x, y);
                if (IsPositionOccupied(gridPos)) continue;

                var r = Random.Range(0, 1001);
                if (r > group.frequency) continue;

                var clone = Instantiate(group.basePrefab, parentObject.transform);
                clone.transform.position = (Vector3)CalculateObjectPosition(x, y) + group.offset;

                var sprite = GetRandomListElement(group.variants);
                clone.GetComponent<SpriteRenderer>().sprite = sprite;
                
                _occupiedPositions.Add(gridPos);
            }
        }
    }

    private bool IsPositionOccupied(Vector2Int pos)
    {
        return _occupiedPositions.Any(checkedPos => checkedPos.x == pos.x && checkedPos.y == pos.y);
    }
}
