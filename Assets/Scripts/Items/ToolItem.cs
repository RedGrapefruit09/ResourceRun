using System.Collections;
using System.Text;
using ResourceRun.Gathering;
using ResourceRun.Player;
using UnityEngine;

namespace ResourceRun.Items
{
    /// <summary>
    /// An extension of a <see cref="SimpleItem"/> with tool-specific code, such as tool usage animation and backend statistics code.
    /// </summary>
    public class ToolItem : SimpleItem
    {
        [Header("Tool Settings")]
        [SerializeField] [Tooltip("The initial amount of durability that this tool has")]
        private int initialDurability;
        [SerializeField] [Tooltip("The amount of durability that is consumed every time this tool is used to gather a resource")]
        private int consumedDurability = 20;
        [Range(1, 10)] [Tooltip("The efficiency value of this tool. This determines how quickly resources will be gathered using it")]
        public int efficiency;
        [Tooltip("The target of this tool: anything, ores only or trees only")]
        public ToolTarget target;
        [SerializeField] [Tooltip("The relative amount of rotations for when this tool is animated")]
        private int animationRotations;

        [Header("Repair Settings")]
        [SerializeField] [Tooltip("The initial amount of durability that will be repaired by the repair panel")]
        private int initialRepairEfficiency = 250;
        [SerializeField] [Tooltip("The initial amount of material that this tool is made out of that will be required by the repair panel")]
        private int initialRepairCost = 3;
        [SerializeField] [Range(0f, 1f)] [Tooltip("Every time the tool is repaired, its repair efficiency will multiplied by 1f-repairEfficiencyDegradation")]
        private float repairEfficiencyDegradation = 0.1f;
        [SerializeField] [Tooltip("By how much should the repair cost for this tool be increased after each repair")]
        private int repairCostIncrease = 2;
        
        private bool _animationPlaying;
        private int _durability;
        private int _repairEfficiency;
        private PlayerInventory _inventory;

        /// <summary>
        /// The current ingredient cost of repairing this tool
        /// </summary>
        public int RepairCost { get; private set; }
        /// <summary>
        /// The <see cref="Item.label"/> of the material that this tool was made out of
        /// </summary>
        public string MaterialLabel { get; private set; }

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

        protected override void Update()
        {
            if (!_animationPlaying) base.Update();
        }

        /// <summary>
        /// Repairs this tool and updates all repair-related internal statistics.
        /// </summary>
        public void Repair()
        {
            _durability += _repairEfficiency;

            var multiplier = 1f - repairEfficiencyDegradation;
            _repairEfficiency = Mathf.RoundToInt(_repairEfficiency * multiplier);
            RepairCost += repairCostIncrease;

            if (_durability > initialDurability) _durability = initialDurability;
        }

        /// <summary>
        /// Consumes the set amount of durability when a <see cref="Gatherable"/> is destroyed.
        /// </summary>
        public void Use()
        {
            _durability -= consumedDurability;

            if (_durability <= 0) _inventory.RemoveItem(this);
        }

        /// <summary>
        /// Checks whether this tool has full durability, meaning it cannot be repaired.
        /// </summary>
        /// <returns>Whether this tool has full durability</returns>
        public bool HasFullDurability()
        {
            return _durability >= initialDurability;
        }

        /// <summary>
        /// Starts animating this tool. The actual implementation is hidden in a coroutine (<see cref="PlayAnimationInternal"/>).
        /// </summary>
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

        /// <summary>
        /// Prematurely suspends the currently running tool usage animation.
        /// </summary>
        public void StopAnimation()
        {
            _animationPlaying = false;
            StopAllCoroutines();
        }

        public override void BuildTooltip(StringBuilder tooltip)
        {
            base.BuildTooltip(tooltip);

            var durabilityPercentage = _durability * 100 / initialDurability;
            var repairRatio = _repairEfficiency * 100 / initialDurability;

            tooltip.AppendLine($"Durability: {_durability}/{initialDurability} ({durabilityPercentage}%)");
            tooltip.AppendLine($"Efficiency: {efficiency}");
            tooltip.AppendLine($"{_repairEfficiency} durability ({repairRatio}%) will be repaired next time");
            tooltip.AppendLine($"Repair will cost {RepairCost} {MaterialLabel}");
        }
    }

    public enum ToolTarget
    {
        Ores,
        Trees,
        AnyObject
    }
}