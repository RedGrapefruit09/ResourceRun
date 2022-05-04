using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The procedural world generation system for ResourceRun.
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    [SerializeField] [Tooltip("All GenerationSteps to be invoked when using this world generator")]
    private GenerationStep[] steps;
    
    [Tooltip("A reference to the player's object so that the generator can put the player on a random position")]
    public GameObject player;
    [Tooltip("The POT (power-of-two) generated world width. If NPOT, will be converted to POT")]
    public int worldWidth;
    [Tooltip("The POT (power-of-two) generated world height. If NPOT, will be converted to POT")]
    public int worldHeight;
    [Tooltip("The Season (biome) of the world to generate. This ScriptableObject contains most generation settings")]
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
    
    /// <summary>
    /// Clears everything in the scene that was generated prior to calling this method.
    /// 
    /// The cleared objects include all positional and regular objects.
    /// 
    /// <see cref="GenerationStep"/>s can optionally define a <see cref="GenerationStep.Clear"/> method with
    /// custom clearing logic that will be executed by this method.
    /// </summary>
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
    
    /// <summary>
    /// Generates the world using all configured <see cref="GenerationStep"/>s.
    ///
    /// <see cref="ClearWorld"/> will be automatically called by this method!
    /// </summary>
    private void GenerateWorld()
    {
        ClearWorld();

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

    /// <summary>
    /// Registers a new "positional" (tied to a grid position) world object to the generator's internal registry.
    /// </summary>
    /// <param name="x">The grid X position</param>
    /// <param name="y">The grid Y position</param>
    /// <param name="obj">The registered <see cref="GameObject"/></param>
    public void AddPositionalObject(int x, int y, GameObject obj)
    {
        _positionalObjects.Add(new Vector2Int(x, y), obj);
    }

    /// <summary>
    /// Queries the generator's internal registry to see if a positional object exists (is registered)
    /// at the given grid position.
    /// </summary>
    /// <param name="x">The grid X position</param>
    /// <param name="y">The grid Y position</param>
    /// <returns>A boolean representing whether the positional object is present</returns>
    public bool PositionalObjectExistsAt(int x, int y)
    {
        var vector = new Vector2Int(x, y);
        return _positionalObjects.Any(pair => pair.Key == vector);
    }

    /// <summary>
    /// Destroys a positional object at a given position manually (typically it's done automatically
    /// by <see cref="ClearWorld"/>), if there is one.
    /// </summary>
    /// <param name="x">The grid X position</param>
    /// <param name="y">The grid Y position</param>
    public void DestroyPositionalObject(int x, int y)
    {
        var vector = new Vector2Int(x, y);

        if (_positionalObjects.TryGetValue(vector, out var obj))
        {
            _positionalObjects.Remove(vector);
            Destroy(obj);
            Log.Info($"Deleted a positional object at x={x}; y={y}");
            return;
        }
        
        Log.Warning($"Tried to delete a non-existing positional object at x={x}; y={y}");
    }

    /// <summary>
    /// Registers a new object to the generator's internal registry.
    /// </summary>
    /// <param name="obj">The Unity <see cref="GameObject"/> instance of the registered object</param>
    public void AddObject(GameObject obj)
    {
        _objects.Add(obj);
    }
}