using System.Collections;
using ResourceRun.Items;
using UnityEngine;

namespace ResourceRun.Gathering
{
    /// <summary>
    /// A <see cref="Gatherable"/> is an object in the game world that can be gathered (collected) using a <see cref="ToolItem"/>.
    /// </summary>
    public class Gatherable : MonoBehaviour
    {
        [Header("Common Settings")]
        [Tooltip("Which tool target fits this Gatherable")]
        public ToolTarget target;
        [SerializeField] [Tooltip("The base delay in seconds for switching overlay frames or rotations")]
        private float baseGatherDelay = 1f;
        [SerializeField] [Tooltip("By how much a single unit of tool efficiency reduces the gather delay")]
        private float gatherDelayReducer = 0.05f;
        [SerializeField] [Tooltip("The lowest value that a gather delay could possibly have")]
        private float minimalGatherDelay = 0.05f;
        [SerializeField] [Tooltip("A prefab used for dropping the collected items from gathering this object")]
        private GameObject droppedItemPrefab;

        [Header("Animation Settings")]
        [SerializeField] [Tooltip("The method of animation used for animating the gathering animation for this object")]
        private GatherableAnimationType animationType;
        [SerializeField] [Tooltip("For animationType=Overlay. All the overlay animation frames")]
        private Sprite[] overlays;
        [SerializeField] [Tooltip("For animationType=Fall. The maximum Z rotation degree of the fall")]
        private float maxFallRotation;

        /// <summary>
        /// The <see cref="GatherableLootTable"/> that is bound to this <see cref="Gatherable"/> object.
        /// </summary>
        public GatherableLootTable LootTable { get; set; }

        /// <summary>
        /// A coroutine that plays the gathering animation, drops the <see cref="LootTable"/> and destroys this <see cref="Gatherable"/>.
        /// </summary>
        /// <param name="tool">The <see cref="ToolItem"/> used to gather this <see cref="Gatherable"/> object</param>
        public IEnumerator Gather(ToolItem tool)
        {
            var delay = baseGatherDelay - gatherDelayReducer * tool.efficiency;
            if (delay <= 0f) delay = minimalGatherDelay;

            if (animationType == GatherableAnimationType.Overlay)
            {
                var overlayObject = new GameObject
                {
                    transform =
                    {
                        parent = transform,
                        localPosition = Vector2.zero
                    }
                };

                var overlayRenderer = overlayObject.AddComponent<SpriteRenderer>();
                overlayRenderer.sortingOrder = 1;

                foreach (var overlay in overlays)
                {
                    overlayRenderer.sprite = overlay;
                    yield return new WaitForSeconds(delay);
                }

                Destroy(overlayObject);
            }

            if (animationType == GatherableAnimationType.Fall)
            {
                Destroy(GetComponent<BoxCollider2D>());

                while (Mathf.Abs(transform.rotation.eulerAngles.z - maxFallRotation) > 0.1f)
                {
                    transform.Rotate(0f, 0f, 0.4f);
                    yield return new WaitForSeconds(0.01f);
                }
            }

            tool.Use();

            LootTable.Generate(
                droppedItemPrefab,
                transform.position,
                minXSpread: -1f, minYSpread: -1f,
                maxXSpread: 1f, maxYSpread: 1f);

            Destroy(gameObject);
        }
    }

    public enum GatherableAnimationType
    {
        Overlay,
        Fall
    }
}