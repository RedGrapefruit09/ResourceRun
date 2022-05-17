using System.Collections;
using System.Collections.Generic;
using ResourceRun.World.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ResourceRun.World
{
    public class WorldFade : MonoBehaviour
    {
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private float delay;

        private WorldGenerator _generator;

        private void Start()
        {
            _generator = FindObjectOfType<WorldGenerator>();
        }

        public void StartFade()
        {
            StartCoroutine(FadeWorld());
        }

        public void StopFade()
        {
            StopAllCoroutines();
        }

        private IEnumerator FadeWorld()
        {
            while (true) yield return FadeLayer();
        }

        private IEnumerator FadeLayer()
        {
            var boundaries = GetWorldBoundaries();
            var edgePositions = GetEdgePositions(boundaries);

            if (edgePositions.Count == 0)
            {
                StopFade();
                yield return null;
            }

            while (edgePositions.Count > 0)
            {
                yield return new WaitForSeconds(delay);

                var pos = edgePositions[Random.Range(0, edgePositions.Count)];
                edgePositions.Remove(pos);

                groundTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);

                _generator.DestroyPositionalObject(pos.x, pos.y);

                RecomputeTile(pos.x + 1, pos.y);
                RecomputeTile(pos.x - 1, pos.y);
                RecomputeTile(pos.x, pos.y + 1);
                RecomputeTile(pos.x, pos.y - 1);
            }
        }

        private void RecomputeTile(int x, int y)
        {
            if (TilePositionIsEmpty(x, y)) return;

            bool TilePositionIsEmpty(int checkedX, int checkedY)
            {
                return groundTilemap.GetTile(new Vector3Int(checkedX, checkedY, 0)) == null;
            }

            void SetToTile(TileBase tile)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }

            if (TilePositionIsEmpty(x + 1, y))
            {
                if (TilePositionIsEmpty(x, y - 1) && TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.season.groundRightAloneTile);
                else if (TilePositionIsEmpty(x, y - 1))
                    SetToTile(_generator.season.groundBottomRightTile);
                else if (TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.season.groundTopRightTile);
                else
                    SetToTile(_generator.season.groundRightTile);

                return;
            }

            if (TilePositionIsEmpty(x, y + 1))
            {
                if (TilePositionIsEmpty(x - 1, y) && TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.season.groundTopAloneTile);
                else if (TilePositionIsEmpty(x - 1, y))
                    SetToTile(_generator.season.groundTopLeftTile);
                else if (TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.season.groundTopRightTile);
                else
                    SetToTile(_generator.season.groundTopTile);

                return;
            }

            if (TilePositionIsEmpty(x - 1, y))
            {
                if (TilePositionIsEmpty(x, y - 1) && TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.season.groundLeftAloneTile);
                else if (TilePositionIsEmpty(x, y - 1))
                    SetToTile(_generator.season.groundBottomLeftTile);
                else if (TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.season.groundTopLeftTile);
                else
                    SetToTile(_generator.season.groundLeftTile);

                return;
            }

            if (TilePositionIsEmpty(x, y - 1))
            {
                if (TilePositionIsEmpty(x - 1, y) && TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.season.groundBottomAloneTile);
                else if (TilePositionIsEmpty(x - 1, y))
                    SetToTile(_generator.season.groundBottomLeftTile);
                else if (TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.season.groundBottomRightTile);
                else
                    SetToTile(_generator.season.groundBottomTile);
            }
        }

        private WorldBoundaries GetWorldBoundaries()
        {
            groundTilemap.CompressBounds();

            var cellBounds = groundTilemap.cellBounds;

            return new WorldBoundaries(
                cellBounds.xMin,
                cellBounds.yMin,
                cellBounds.xMax,
                cellBounds.yMax);
        }

        private static List<Vector2Int> GetEdgePositions(WorldBoundaries boundaries)
        {
            var positions = new List<Vector2Int>();

            void CountVertical(int x)
            {
                for (var y = boundaries.MinY; y < boundaries.MaxY; ++y) positions.Add(new Vector2Int(x, y));
            }

            void CountHorizontal(int y)
            {
                for (var x = boundaries.MinX + 1; x < boundaries.MaxX; ++x) positions.Add(new Vector2Int(x, y));
            }

            CountVertical(boundaries.MinX);
            CountVertical(boundaries.MaxX - 1);
            CountHorizontal(boundaries.MinY);
            CountHorizontal(boundaries.MaxY - 1);

            return positions;
        }

        private struct WorldBoundaries
        {
            public int MinX { get; }
            public int MinY { get; }
            public int MaxX { get; }
            public int MaxY { get; }

            public WorldBoundaries(int minX, int minY, int maxX, int maxY)
            {
                MinX = minX;
                MinY = minY;
                MaxX = maxX;
                MaxY = maxY;
            }
        }
    }
}