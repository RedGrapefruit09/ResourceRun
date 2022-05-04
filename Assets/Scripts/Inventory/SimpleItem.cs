using UnityEngine;

public class SimpleItem : Item
{
    [SerializeField] private string tooltip;
    
    public override void OnPickedUp()
    {
        
    }

    public override void OnDropped()
    {
        
    }

    public override void OnSelected()
    {
        
    }

    public override void OnDeselected()
    {
        
    }

    public override string BuildTooltip()
    {
        return tooltip;
    }
}