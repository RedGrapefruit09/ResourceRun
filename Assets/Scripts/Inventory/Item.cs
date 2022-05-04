using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int maxCount;
    public string label;

    public abstract void OnPickedUp();

    public abstract void OnDropped();

    public abstract void OnSelected();

    public abstract void OnDeselected();

    public abstract string BuildTooltip();
}
