using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private int worldWidth;
    [SerializeField] private int worldHeight;
    [SerializeField] private Season currentSeason;
    [SerializeField] private Tilemap groundTilemap;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        GenerateGrass();
        GenerateWorldBorders();
    }

    private void GenerateGrass()
    {
        groundTilemap.transform.position = new Vector3(-(worldWidth / 2), -(worldHeight / 2));
        
        for (var x = 0; x < worldWidth; ++x)
        {
            for (var y = 0; y < worldHeight; ++y)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), GetGrassTile(x, y));
            }
        }
    }

    private void GenerateWorldBorders()
    {
        var borderParentObject = new GameObject { name = "World Borders" };
        
        void CreateBorder(Vector2 pos, Vector2 size, string borderName)
        {
            var obj = new GameObject();
            obj.transform.parent = borderParentObject.transform;
            obj.transform.position = pos;
            obj.name = borderName;
            var boxCollider = obj.AddComponent<BoxCollider2D>();
            boxCollider.size = size;
        }
        
        CreateBorder(
            new Vector2(0f, -(worldHeight / 2f) - 0.5f),
            new Vector2(worldWidth, 1f),
            "Bottom World Border");
        
        CreateBorder(
            new Vector2(0f, worldHeight / 2f + 0.5f),
            new Vector2(worldWidth, 1f),
            "Top World Border");
        
        CreateBorder(
            new Vector2(-(worldWidth / 2f) - 0.5f, 0f),
            new Vector2(1f, worldHeight),
            "Left World Border");
        
        CreateBorder(
            new Vector2(worldWidth / 2f + 0.5f, 0f),
            new Vector2(1f, worldHeight),
            "Right World Border");
    }

    private Tile GetGrassTile(int x, int y)
    {
        if (y == 0)
        {
            if (x == 0)
            {
                return currentSeason.groundBottomLeftTile;
            }

            if (x == worldWidth - 1)
            {
                return currentSeason.groundBottomRightTile;
            }

            return currentSeason.groundBottomTile;
        }

        if (y == worldHeight - 1)
        {
            if (x == 0)
            {
                return currentSeason.groundTopLeftTile;
            }

            if (x == worldWidth - 1)
            {
                return currentSeason.groundTopRightTile;
            }

            return currentSeason.groundTopTile;
        }

        if (x == 0)
        {
            return currentSeason.groundLeftTile;
        }

        if (x == worldWidth - 1)
        {
            return currentSeason.groundRightTile;
        }

        return currentSeason.groundCenterTile;
    }
}