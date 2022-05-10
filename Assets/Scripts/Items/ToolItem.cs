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
    private PlayerInventory _inventory;

    protected override void Start()
    {
        base.Start();
        _durability = initialDurability;
        _inventory = FindObjectOfType<PlayerInventory>();
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
        
        if (_durability <= 0)
        {
            _inventory.RemoveItem(this);
        }
    }

    public override string BuildTooltip()
    {
        var baseTooltip = base.BuildTooltip();
        var durabilityPercent = _durability * 100 / initialDurability;
        return $"{baseTooltip}\n{durabilityPercent}% durability";
    }
}

public enum ToolTarget
{
    Ores,
    Trees,
    AnyObject
}
