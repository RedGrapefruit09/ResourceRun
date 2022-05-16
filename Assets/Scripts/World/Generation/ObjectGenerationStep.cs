using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This <see cref="GenerationStep"/> is for spawning in different objects from <see cref="ObjectGroup"/>s.
/// </summary>
public class ObjectGenerationStep : GenerationStep
{
    [SerializeField] [Tooltip("The spawn area around the player which will be blocked from spawning objects in")]
    private int spawnArea;
    [SerializeField] [Tooltip("The excluded area for generating objects around every edge of the world")]
    private int excludedEdgeArea;
    
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
        var weightedBag = new WeightedRandomBag<ObjectVariant>();
        foreach (var variant in group.variants) weightedBag.AddEntry(variant, variant.weight);

        for (var x = excludedEdgeArea; x < generator.worldWidth - excludedEdgeArea; ++x)
        {
            for (var y = excludedEdgeArea; y < generator.worldHeight - excludedEdgeArea; ++y)
            {
                var basePos = new Vector2Int(x, y);

                var occupied = group.occupiedPositions
                    .Select(offset => basePos + offset)
                    .Any(IsPositionOccupied);
                if (occupied) continue;

                var r = Random.Range(0, 1001);
                if (r > group.frequency) continue;

                var clone = Instantiate(group.basePrefab);
                clone.transform.position = CalculateObjectPosition(x, y) + group.offset;

                var variant = weightedBag.GetRandom();
                clone.GetComponent<SpriteRenderer>().sprite = variant.sprite;

                if (clone.TryGetComponent<BoxCollider2D>(out var boxCollider))
                {
                    boxCollider.size = variant.colliderSize;
                }

                if (clone.TryGetComponent<Gatherable>(out var gatherable))
                {
                    gatherable.LootTable = variant.lootTable;
                }
                
                foreach (var offset in group.occupiedPositions)
                {
                    var pos = basePos + offset;
                    _occupiedPositions.Add(pos);
                    generator.AddPositionalObject(pos.x, pos.y, clone);
                }
            }
        }
    }

    private bool IsPositionOccupied(Vector2Int pos)
    {
        return _occupiedPositions.Any(checkedPos => checkedPos.x == pos.x && checkedPos.y == pos.y);
    }
}
