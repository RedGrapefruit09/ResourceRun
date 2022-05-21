using System.Text;
using UnityEngine;

namespace ResourceRun.Items
{
    /// <summary>
    /// An extension of a <see cref="SimpleItem"/> for all resource items that contains resource-specific statistics.
    /// </summary>
    public class ResourceItem : SimpleItem
    {
        [SerializeField] [Tooltip("How much a single unit of this resource is worth in in-game currency")]
        private int worth;

        public override void BuildTooltip(StringBuilder tooltip)
        {
            base.BuildTooltip(tooltip);
            tooltip.AppendLine($"Worth {worth}$ of in-game currency");
        }
    }
}