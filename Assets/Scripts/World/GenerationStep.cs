using UnityEngine;

public abstract class GenerationStep : MonoBehaviour
{
    [HideInInspector] public WorldGenerator generator;

    public abstract void Generate();
    
    public virtual void Clear() {}

    protected Vector2 CalculateObjectPosition(int x, int y)
    {
        return new Vector2(
            x - generator.worldWidth / 2f + 0.5f,
            y - generator.worldHeight / 2f + 0.5f);
    }

    protected T GetRandomListElement<T>(T[] list)
    {
        return list[Random.Range(0, list.Length)];
    }
}
