using System.Collections;
using UnityEngine;

public class ToolItem : SimpleItem
{
    [Header("Tool Settings")]
    [SerializeField] private int initialDurability;
    public int efficiency;
    public ToolTarget target;
    [SerializeField] private int consumedDurability = 20;
    [SerializeField] private int animationRotations;
    
    [Header("Repair Settings")]
    [SerializeField] private int initialRepairEfficiency = 250;
    [SerializeField] private int initialRepairCost = 3;
    [SerializeField] private float repairEfficiencyDegradation = 0.1f;
    [SerializeField] private int repairCostIncrease = 2;
    
    public int RepairCost { get; private set; }
    public string MaterialLabel { get; private set; }
    
    private int _durability;
    private int _repairEfficiency;
    private PlayerInventory _inventory;
    private bool _animationPlaying;

    protected override void Start()
    {
        base.Start();
        
        _durability = initialDurability;
        
        _repairEfficiency = initialRepairEfficiency;
        if (_repairEfficiency > initialDurability) _repairEfficiency = initialDurability;

        RepairCost = initialRepairCost;

        _inventory = FindObjectOfType<PlayerInventory>();
        MaterialLabel = label.Split(' ')[0];
    }

    public void Repair()
    {
        _durability += _repairEfficiency;

        var multiplier = 1f - repairEfficiencyDegradation;
        _repairEfficiency = Mathf.RoundToInt(_repairEfficiency * multiplier);
        RepairCost += repairCostIncrease;
        
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

    public bool HasFullDurability() => _durability >= initialDurability;

    public void StartAnimation()
    {
        _animationPlaying = true;
        StartCoroutine(PlayAnimationInternal());
    }

    private IEnumerator PlayAnimationInternal()
    {
        while (true)
        {
            for (var i = 0; i < animationRotations; ++i)
            {
                transform.Rotate(0f, 0f, 2f);
                yield return new WaitForSeconds(0.01f);
            }

            for (var i = 0; i < animationRotations; ++i)
            {
                transform.Rotate(0f, 0f, -2f);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void StopAnimation()
    {
        _animationPlaying = false;
        StopAllCoroutines();
    }

    public override void BuildTooltip(ItemTooltip tooltip)
    {
        base.BuildTooltip(tooltip);
        
        var durabilityPercentage = _durability * 100 / initialDurability;
        var repairRatio = _repairEfficiency * 100 / initialDurability;
        
        tooltip.Add($"Durability: {_durability}/{initialDurability} ({durabilityPercentage}%)");
        tooltip.Add($"Efficiency: {efficiency}");
        tooltip.Add($"{_repairEfficiency} durability ({repairRatio}%) will be repaired next time");
        tooltip.Add($"Repair will cost {RepairCost} {MaterialLabel}");
    }

    protected override void Update()
    {
        if (!_animationPlaying)
        {
            base.Update();
        }
    }
}

public enum ToolTarget
{
    Ores,
    Trees,
    AnyObject
}
