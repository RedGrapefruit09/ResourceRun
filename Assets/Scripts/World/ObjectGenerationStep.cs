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
            if (objectGroup.variants.Count > 0)
            {
                GenerateGroup(objectGroup);
            }
        }
    }

    private void OccupySpawnArea()
    {
        var centerX = Mathf.RoundToInt(generator.player.transform.position.x);
        var centerY = Mathf.RoundToInt(generator.player.transform.position.y);

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
        var weightedBag = new WeightedRandomBag<ObjectVariant>();
        foreach (var variant in group.variants) weightedBag.AddEntry(variant, variant.frequency);

        for (var x = excludedEdgeArea; x < generator.worldWidth - excludedEdgeArea; ++x)
        {
            for (var y = excludedEdgeArea; y < generator.worldHeight - excludedEdgeArea; ++y)
            {
                var gridPos = new Vector2Int(x, y);
                if (IsPositionOccupied(gridPos)) continue;

                var r = Random.Range(0, 1001);
                if (r > group.frequency) continue;

                var clone = Instantiate(group.basePrefab);
                clone.transform.position = (Vector3)CalculateObjectPosition(x, y) + group.offset;

                var variant = weightedBag.GetRandom();
                clone.GetComponent<SpriteRenderer>().sprite = variant.sprite;

                if (clone.TryGetComponent<BoxCollider2D>(out var boxCollider))
                {
                    boxCollider.size = variant.colliderSize;
                }
                
                generator.AddPositionalObject(x, y, clone);
                
                _occupiedPositions.Add(gridPos);
            }
        }
    }

    private bool IsPositionOccupied(Vector2Int pos)
    {
        return _occupiedPositions.Any(checkedPos => checkedPos.x == pos.x && checkedPos.y == pos.y);
    }
}
