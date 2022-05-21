using UnityEngine;
using UnityEngine.Tilemaps;

namespace ResourceRun.World.Generation
{
    /// <summary>
    /// A <see cref="GenerationStep" /> for generating ground tiles.
    /// For optimization purposes, instead of creating a <see cref="GameObject" /> with a <see cref="SpriteRenderer" />
    /// for every ground tile (which stresses the engine a lot), this <see cref="GenerationStep" /> uses a pre-defined
    /// <see cref="Tilemap" /> on the scene and streams tiles set from the <see cref="Season" /> into the
    /// <see cref="Tilemap" />
    /// at runtime.
    /// This also does automatic tiling to get the corners and up, right, left, down sides of the ground being set
    /// correctly.
    /// </summary>
    public class GroundGenerationStep : GenerationStep
    {
        [SerializeField] private Tilemap groundTilemap;

        public override void Generate()
        {
            for (var x = 0; x < generator.worldWidth; ++x)
            for (var y = 0; y < generator.worldHeight; ++y)
                groundTilemap.SetTile(new Vector3Int(x, y, 0), GetGroundTile(x, y));
        }

        public override void Clear()
        {
            foreach (var pos in groundTilemap.cellBounds.allPositionsWithin) groundTilemap.SetTile(pos, null);
        }

        private Tile GetGroundTile(int x, int y)
        {
            if (y == 0)
            {
                if (x == 0) return generator.Season.groundBottomLeftTile;

                if (x == generator.worldWidth - 1) return generator.Season.groundBottomRightTile;

                return generator.Season.groundBottomTile;
            }

            if (y == generator.worldHeight - 1)
            {
                if (x == 0) return generator.Season.groundTopLeftTile;

                if (x == generator.worldWidth - 1) return generator.Season.groundTopRightTile;

                return generator.Season.groundTopTile;
            }

            if (x == 0) return generator.Season.groundLeftTile;

            if (x == generator.worldWidth - 1) return generator.Season.groundRightTile;

            return generator.Season.groundCenterTile;
        }
    }
}