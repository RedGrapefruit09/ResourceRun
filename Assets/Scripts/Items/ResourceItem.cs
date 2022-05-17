using UnityEngine;

namespace ResourceRun.Items
{
    public class ResourceItem : SimpleItem
    {
        [SerializeField] private int worth;

        public override void BuildTooltip(ItemTooltip tooltip)
        {
            base.BuildTooltip(tooltip);
            tooltip.Add($"Worth {worth}$ of in-game currency");
        }
    }
}