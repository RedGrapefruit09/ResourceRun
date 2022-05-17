using UnityEngine;

namespace ResourceRun.World.Generation
{
    /// <summary>
    ///     A <see cref="GenerationStep" /> is a notable phase of world generation (for example, generating trees or the
    ///     ground).
    /// </summary>
    public abstract class GenerationStep : MonoBehaviour
    {
        [HideInInspector] public WorldGenerator generator;

        /// <summary>
        ///     Your <see cref="GenerationStep" />'s generation code should be put inside this method.
        /// </summary>
        public abstract void Generate();

        /// <summary>
        ///     Additional logic that will be invoked when the world is cleared (<see cref="WorldGenerator.ClearWorld" />).
        /// </summary>
        public virtual void Clear()
        {
        }

        /// <summary>
        ///     Converts a grid position to a world (actual) position.
        /// </summary>
        /// <param name="x">The grid X position</param>
        /// <param name="y">The grid Y position</param>
        /// <returns>The converted <see cref="Vector2" /></returns>
        protected static Vector3 CalculateObjectPosition(int x, int y)
        {
            return new Vector3(x + 0.5f, y + 0.5f);
        }
    }
}