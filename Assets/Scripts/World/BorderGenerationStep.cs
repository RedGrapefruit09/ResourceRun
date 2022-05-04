using UnityEngine;

/// <summary>
/// A <see cref="GenerationStep"/> for creating artificial world borders.
///
/// Every world border is represented by a <see cref="GameObject"/> with a <see cref="BoxCollider2D"/> to avoid
/// the player from going outside the world.
/// </summary>
public class BorderGenerationStep : GenerationStep
{
    public override void Generate()
    {
        var borderParentObject = new GameObject { name = "World Borders" };
        
        CreateBorder(
            borderParentObject,
            new Vector2(0f, -(generator.worldHeight / 2f) - 0.5f),
            new Vector2(generator.worldWidth, 1f),
            "Bottom World Border");
        
        CreateBorder(
            borderParentObject,
            new Vector2(0f, generator.worldHeight / 2f + 0.5f),
            new Vector2(generator.worldWidth, 1f),
            "Top World Border");
        
        CreateBorder(
            borderParentObject,
            new Vector2(-(generator.worldWidth / 2f) - 0.5f, 0f),
            new Vector2(1f, generator.worldHeight),
            "Left World Border");
        
        CreateBorder(
            borderParentObject,
            new Vector2(generator.worldWidth / 2f + 0.5f, 0f),
            new Vector2(1f, generator.worldHeight),
            "Right World Border");
        
        generator.AddObject(borderParentObject);
    }
    
    private static void CreateBorder(GameObject borderParentObject, Vector2 pos, Vector2 size, string borderName)
    {
        var obj = new GameObject
        {
            transform =
            {
                parent = borderParentObject.transform,
                position = pos
            },
            name = borderName
        };
        
        var boxCollider = obj.AddComponent<BoxCollider2D>();
        boxCollider.size = size;
    }
}