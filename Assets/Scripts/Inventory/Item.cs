using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int maxCount;
    public string label;

    public int Amount { get; private set; } = 1;

    public abstract void OnSelected();

    public abstract void OnDeselected();

    public abstract string BuildTooltip();

    public bool Increment(int value = 1)
    {
        if (Amount + value > maxCount) return false;
        
        Amount += value;
        return true;
    }

    public void Decrement(int value = 1)
    {
        if (Amount > 1)
        {
            Amount -= value;
        }
    }
}
