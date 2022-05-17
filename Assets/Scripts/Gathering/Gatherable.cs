using System.Collections;
using ResourceRun.Items;
using UnityEngine;

namespace ResourceRun.Gathering
{
    public class Gatherable : MonoBehaviour
    {
        [Header("Common Settings")] public ToolTarget target;

        [SerializeField] private float baseGatherDelay = 1f;
        [SerializeField] private float gatherDelayReducer = 0.05f;
        [SerializeField] private float minimalGatherDelay = 0.05f;
        [SerializeField] private GameObject droppedItemPrefab;

        [Header("Animation Settings")] [SerializeField]
        private GatherableAnimationType animationType;

        [SerializeField] private Sprite[] overlays;
        [SerializeField] private float maxFallRotation;

        public GatherableLootTable LootTable { private get; set; }

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
                -1f, -1f,
                1f, 1f);

            Destroy(gameObject);
        }
    }

    public enum GatherableAnimationType
    {
        Overlay,
        Fall
    }
}