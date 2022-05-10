using System.Collections;
using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [Header("Common Settings")]
    public ToolTarget target;
    [SerializeField] private float baseGatherDelay = 1f;
    [SerializeField] private float gatherDelayReducer = 0.05f;
    [Header("Animation Settings")]
    [SerializeField] private GatherableAnimationType animationType;
    [SerializeField] private Sprite[] overlays;
    [SerializeField] private float maxFallRotation;

    public IEnumerator Gather(ToolItem tool)
    {
        var delay = baseGatherDelay - gatherDelayReducer * tool.efficiency;

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
        
        tool.Use();

        Destroy(gameObject);
    }
}

public enum GatherableAnimationType
{
    Overlay,
    Fall
}
