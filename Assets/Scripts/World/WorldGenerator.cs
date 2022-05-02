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
        
        GenerateWorld();
    }
    
    private void ClearWorld()
    {
        foreach (var obj in _worldObjectPool)
        {
            Destroy(obj);
        }

        foreach (var step in steps)
        {
            step.Clear();
        }
        
        _worldObjectPool.Clear();
    }
    
    private void GenerateWorld()
    {
        ClearWorld();

        foreach (var step in steps)
        {
            step.Generate();
        }
    }

    public void RegisterWorldObject(GameObject obj)
    {
        _worldObjectPool.Add(obj);
    }
}