using System.Collections;
using System.Collections.Generic;
using ResourceRun.World.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ResourceRun.World
{
    /// <summary>
    /// The script responsible for the world fade / disappear effect.
    /// The effect is toggled on when world generation begins and off when world generation ends.
    /// </summary>
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

        /// <summary>
        /// Given a tile position, looks at its surrounding neighbors' presence, computes which ground <see cref="Tile" />
        /// from the <see cref="Season" /> matches the surroundings and applies the change.
        /// </summary>
        /// <param name="x">The tile's X coordinate</param>
        /// <param name="y">The tile's Y coordinate</param>
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
                    SetToTile(_generator.Season.groundRightAloneTile);
                else if (TilePositionIsEmpty(x, y - 1))
                    SetToTile(_generator.Season.groundBottomRightTile);
                else if (TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.Season.groundTopRightTile);
                else
                    SetToTile(_generator.Season.groundRightTile);

                return;
            }

            if (TilePositionIsEmpty(x, y + 1))
            {
                if (TilePositionIsEmpty(x - 1, y) && TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.Season.groundTopAloneTile);
                else if (TilePositionIsEmpty(x - 1, y))
                    SetToTile(_generator.Season.groundTopLeftTile);
                else if (TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.Season.groundTopRightTile);
                else
                    SetToTile(_generator.Season.groundTopTile);

                return;
            }

            if (TilePositionIsEmpty(x - 1, y))
            {
                if (TilePositionIsEmpty(x, y - 1) && TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.Season.groundLeftAloneTile);
                else if (TilePositionIsEmpty(x, y - 1))
                    SetToTile(_generator.Season.groundBottomLeftTile);
                else if (TilePositionIsEmpty(x, y + 1))
                    SetToTile(_generator.Season.groundTopLeftTile);
                else
                    SetToTile(_generator.Season.groundLeftTile);

                return;
            }

            if (TilePositionIsEmpty(x, y - 1))
            {
                if (TilePositionIsEmpty(x - 1, y) && TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.Season.groundBottomAloneTile);
                else if (TilePositionIsEmpty(x - 1, y))
                    SetToTile(_generator.Season.groundBottomLeftTile);
                else if (TilePositionIsEmpty(x + 1, y))
                    SetToTile(_generator.Season.groundBottomRightTile);
                else
                    SetToTile(_generator.Season.groundBottomTile);
            }
        }

        /// <summary>
        /// Compresses the cell boundaries of the ground tilemap and computes a <see cref="WorldBoundaries" /> instance
        /// that contains the minimum and maximum cell coordinates of the tilemap on the X and Y axes.
        /// </summary>
        /// <returns>The computed <see cref="WorldBoundaries" /></returns>
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

        /// <summary>
        /// Computes a list of <see cref="Vector2Int" /> cell positions on the ground tilemap, each corresponding to some
        /// exposed edge.
        /// </summary>
        /// <param name="boundaries">The <see cref="WorldBoundaries" />, off of which the calculations are made</param>
        /// <returns>The computed list of <see cref="Vector2Int" /> positions</returns>
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

        /// <summary>
        /// A data record for storing min and max 2D cell coordinates from the ground tilemap for further internal
        /// calculations that accompany the world fade effect.
        /// </summary>
        private struct WorldBoundaries
        {
            /// <summary>
            /// The lowest X cell coordinate
            /// </summary>
            public int MinX { get; }

            /// <summary>
            /// The lowest Y cell coordinate
            /// </summary>
            public int MinY { get; }

            /// <summary>
            /// The highest X cell coordinate
            /// </summary>
            public int MaxX { get; }

            /// <summary>
            /// The highest Y cell coordinate
            /// </summary>
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