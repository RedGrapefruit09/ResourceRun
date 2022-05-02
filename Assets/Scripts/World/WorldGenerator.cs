using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GenerationStep[] steps;
    public int worldWidth;
    public int worldHeight;
    public Season season;

    private readonly List<GameObject> _worldObjectPool = new List<GameObject>();

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
        foreach (var obj in _worldObjectPool)
        {
            Destroy(obj);
        }
        
        Log.Info($"Destroyed {_worldObjectPool.Count} objects from the world object pool");

        foreach (var step in steps)
        {
            step.Clear();
        }
        
        _worldObjectPool.Clear();
        
        Log.Info("World generation has been cleared");
    }
    
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

    public void RegisterWorldObject(GameObject obj)
    {
        _worldObjectPool.Add(obj);
    }
}