using System.Collections;
using UnityEngine;

public class ToolItem : SimpleItem
{
    [Header("Tool Settings")]
    [SerializeField] private int initialDurability;
    public int efficiency;
    public ToolTarget target;
    [SerializeField] private int consumedDurability = 25;
    [SerializeField] private int repairedDurability = 50;
    [SerializeField] private int animationRotations;

    private int _durability;
    private PlayerInventory _inventory;
    private bool _animationPlaying;

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

    public override string BuildTooltip()
    {
        var baseTooltip = base.BuildTooltip();
        var durabilityPercent = _durability * 100 / initialDurability;
        return $"{baseTooltip}\n{durabilityPercent}% durability";
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
