using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [SerializeField] private GatherableAnimationType animationType;
    [SerializeField] private Sprite[] overlays;
    [SerializeField] private float maxFallRotation;
}

public enum GatherableAnimationType
{
    Overlay,
    Fall
}
