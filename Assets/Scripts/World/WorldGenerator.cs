using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GenerationStep[] steps;
    
    public GameObject player;
    public int worldWidth;
    public int worldHeight;
    public Season season;

    private readonly Dictionary<Vector2Int, GameObject> _positionalObjects = new Dictionary<Vector2Int, GameObject>();
    private readonly List<GameObject> _objects = new List<GameObject>();

    private void Start()
    {
        foreach (var step in steps)
        {
            step.generator = this;
        }
        
        Log.Info("Initialized world generator");
        
        GenerateWorld();
    }
    
    private void ClearWorld()
    {
        foreach (var pair in _positionalObjects)
        {
            Destroy(pair.Value);
        }
        
        Log.Info($"Deleted {_positionalObjects.Count} positional objects");
        _positionalObjects.Clear();

        foreach (var obj in _objects)
        {
            Destroy(obj);
        }
        
        Log.Info($"Deleted {_objects.Count} objects");
        _objects.Clear();
        
        foreach (var step in steps)
        {
            step.Clear();
        }
        
        Log.Info("World generation has been cleared");
    }
    
    private void GenerateWorld()
    {
        ClearWorld();

        var playerX = Random.Range(-(worldWidth / 2f), worldWidth / 2f);
        var playerY = Random.Range(-(worldHeight / 2f), worldHeight / 2f);
        player.transform.position = new Vector3(playerX, playerY, player.transform.position.z);
        
        if (worldWidth % 2 != 0)
        {
            Log.Warning($"Non-power-of-two world widths aren't supported. {worldWidth} will be converted to {worldWidth + 1}");
            worldWidth++;
        }

        if (worldHeight % 2 != 0)
        {
            Log.Warning($"Non-power-of-two world heights aren't supported. {worldHeight} will be converted to {worldHeight + 1}");
            worldHeight++;
        }
        
        foreach (var step in steps)
        {
            step.Generate();
        }
        
        Log.Info($"Generated world: {worldWidth}x{worldHeight}, {season.seasonName}");
    }

    public void AddPositionalObject(int x, int y, GameObject obj)
    {
        _positionalObjects.Add(new Vector2Int(x, y), obj);
    }

    public bool PositionalObjectExistsAt(int x, int y)
    {
        var vector = new Vector2Int(x, y);
        return _positionalObjects.Any(pair => pair.Key == vector);
    }

    public void DeletePositionalObject(int x, int y)
    {
        var vector = new Vector2Int(x, y);

        if (_positionalObjects.TryGetValue(vector, out var obj))
        {
            Destroy(obj);
            Log.Info($"Deleted a positional object at x={x}; y={y}");
            return;
        }
        
        Log.Warning($"Tried to delete a non-existing positional object at x={x}; y={y}");
    }

    public void AddObject(GameObject obj)
    {
        _objects.Add(obj);
    }
}