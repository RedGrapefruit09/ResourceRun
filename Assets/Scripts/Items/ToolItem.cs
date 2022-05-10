using UnityEngine;

public class ToolItem : SimpleItem
{
    [Header("Tool Settings")]
    [SerializeField] private int initialDurability;
    public int efficiency;
    public ToolTarget target;
    [SerializeField] private int consumedDurability = 25;
    [SerializeField] private int repairedDurability = 50;

    private int _durability;

    protected override void Start()
    {
        base.Start();
        _durability = initialDurability;
    }

    public void Repair()
    {
        _durability += repairedDurability;
        
        if (_durability > initialDurability)
        {
            _durability = initialDurability;
        }
    }

    public void Use()
    {
        _durability -= consumedDurability;
        
        if (_durability < 0)
        {
            _durability = 0;
        }
    }
}

public enum ToolTarget
{
    Ores,
    Trees,
    AnyObject
}
