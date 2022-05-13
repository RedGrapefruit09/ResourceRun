using UnityEngine;

public class ResourceItem : SimpleItem
{
    [SerializeField] private int worth;

    public override string BuildTooltip()
    {
        var worthString = $"Worth {worth}$ of in-game currency";
        var baseString = base.BuildTooltip();
        return $"{baseString}\n{worthString}";
    }
}