﻿using System.Collections.Generic;
using ResourceRun.Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ResourceRun.World.Generation
{
    /// <summary>
    /// The procedural world generation system for ResourceRun.
    /// </summary>
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] [Tooltip("All GenerationSteps to be invoked when using this world generator")]
        private GenerationStep[] steps;

        [Tooltip("The POT (power-of-two) generated world width. If NPOT, will be converted to POT")]
        public int worldWidth;

        [Tooltip("The POT (power-of-two) generated world height. If NPOT, will be converted to POT")]
        public int worldHeight;

        [Tooltip("All of the seasons in the game. These will be generated sequentially")]
        public List<Season> allSeasons;

        /// <summary>
        /// The <see cref="Season"/> that was last generated or is currently being generated.
        /// </summary>
        public Season Season { get; private set; }

        private readonly List<GameObject> _objects = new List<GameObject>();

        private readonly Dictionary<Vector2Int, GameObject> _positionalObjects =
            new Dictionary<Vector2Int, GameObject>();

        private WorldFade _fade;
        private PlayerMovement _player;

        private void Start()
        {
            _fade = FindObjectOfType<WorldFade>();
            _player = FindObjectOfType<PlayerMovement>();

            foreach (var step in steps) step.generator = this;

            Debug.Log("Initialized world generator");

            GenerateNextSeason();
        }

        public void GenerateNextSeason()
        {
            if (Season != null)
            {
                var nextIndex = allSeasons.IndexOf(Season) + 1;

                if (nextIndex >= allSeasons.Count)
                {
                    Application.Quit();
#if UNITY_EDITOR
                    EditorApplication.ExitPlaymode();
#endif
                    return;
                }

                Season = allSeasons[nextIndex];
            }
            else
            {
                Season = allSeasons[0];
            }
            
            GenerateWorld();
        }

        /// <summary>
        /// Clears everything in the scene that was generated prior to calling this method.
        /// The cleared objects include all positional and regular objects.
        /// <see cref="GenerationStep" />s can optionally define a <see cref="GenerationStep.Clear" /> method with
        /// custom clearing logic that will be executed by this method.
        /// </summary>
        private void ClearWorld()
        {
            foreach (var pair in _positionalObjects) Destroy(pair.Value);

            Debug.Log($"Deleted {_positionalObjects.Count} positional objects");
            _positionalObjects.Clear();

            foreach (var obj in _objects) Destroy(obj);

            Debug.Log($"Deleted {_objects.Count} objects");
            _objects.Clear();

            foreach (var step in steps) step.Clear();

            Debug.Log("World generation has been cleared");
        }

        /// <summary>
        /// Generates the world using all configured <see cref="GenerationStep" />s.
        /// <see cref="ClearWorld" /> will be automatically called by this method!
        /// </summary>
        private void GenerateWorld()
        {
            _fade.StopFade();

            ClearWorld();

            if (worldWidth % 2 != 0)
            {
                Debug.LogWarning(
                    $"Non-power-of-two world widths aren't supported. {worldWidth} will be converted to {worldWidth + 1}");
                worldWidth++;
            }

            if (worldHeight % 2 != 0)
            {
                Debug.LogWarning(
                    $"Non-power-of-two world heights aren't supported. {worldHeight} will be converted to {worldHeight + 1}");
                worldHeight++;
            }

            foreach (var step in steps) step.Generate();

            Debug.Log($"Generated world: {worldWidth}x{worldHeight}, {Season.seasonName}");

            _fade.StartFade();
            _player.transform.position = new Vector3(worldWidth / 2f, worldHeight / 2f);
        }

        /// <summary>
        /// Registers a new "positional" (tied to a grid position) world object to the generator's internal registry.
        /// </summary>
        /// <param name="x">The grid X position</param>
        /// <param name="y">The grid Y position</param>
        /// <param name="obj">The registered <see cref="GameObject" /></param>
        public void AddPositionalObject(int x, int y, GameObject obj)
        {
            _positionalObjects.Add(new Vector2Int(x, y), obj);
        }

        /// <summary>
        /// Destroys a positional object at a given position manually (typically it's done automatically
        /// by <see cref="ClearWorld" />), if there is one.
        /// </summary>
        /// <param name="x">The grid X position</param>
        /// <param name="y">The grid Y position</param>
        public void DestroyPositionalObject(int x, int y)
        {
            var pos = new Vector2Int(x, y);

            if (_positionalObjects.TryGetValue(pos, out var obj))
            {
                if (obj == null) return;

                _positionalObjects.Remove(pos);
                Destroy(obj);
                Debug.Log($"Deleted a positional object at x={x}; y={y}");
            }
        }

        /// <summary>
        /// Registers a new object to the generator's internal registry.
        /// </summary>
        /// <param name="obj">The Unity <see cref="GameObject" /> instance of the registered object</param>
        public void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }
    }
}