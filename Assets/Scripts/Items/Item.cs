using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Item Settings")]
    public int maxCount;
    public string label;

    public int Amount { get; set; } = 1;

    public abstract void OnSelected();

    public abstract void OnDeselected();

    public abstract string BuildTooltip();

    public void Increment(int value = 1)
    {
        Amount += value;
    }

    public void Decrement(int value = 1)
    {
        if (Amount > 1)
        {
            Amount -= value;
        }
    }
}
