using System.Collections;
using System.Linq;
using ResourceRun.Items;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceRun.Player
{
    /// <summary>
    /// The script responsible for the repair mechanic, UI and implementation-wise
    /// </summary>
    public class PlayerRepair : MonoBehaviour
    {
        [Header("Functionality")]
        [SerializeField] [Tooltip("A registry of prefabs for all items that can serve as ingredients for tool repair")]
        private GameObject[] ingredientRegistry;

        [SerializeField] [Tooltip("The processing time in seconds")]
        private float processTime;

        [Header("Colors")]
        [SerializeField] [Tooltip("The color to show when there's enough of an ingredient")]
        private Color fulfilledColor = Color.green;
        [SerializeField] [Tooltip("The color to show when the ingredient requirement has partially been fulfilled")]
        private Color partiallyFulfilledColor = Color.yellow;
        [SerializeField] [Tooltip("The color to show when the ingredient is completely missing")]
        private Color missingColor = Color.red;

        [Header("UI")]
        [SerializeField] [Tooltip("The GameObject of the root UI screen")]
        private GameObject rootObject;
        [SerializeField] [Tooltip("The GameObject containing the root screen's primary contents")]
        private GameObject contentObject;
        [SerializeField] [Tooltip("The Text to be displayed instead of the contentObject when something goes wrong or a requirement isn't met")]
        private Text issueText;
        [SerializeField] [Tooltip("The Text on the repair Button, which is used for color manipulation to indicate the ingredient status")]
        private Text repairButtonText;
        [SerializeField] [Tooltip("The Text displaying the current ingredient status")]
        private Text ingredientStatusText;
        [SerializeField] [Tooltip("The GameObject containing both the background and the actual filled-type bar Image")]
        private GameObject barObject;
        [SerializeField] [Tooltip("The progress bar Image")]
        private Image barImage;
        [SerializeField] [Tooltip("The Image for the icon of the item being repaired")]
        private Image inputItemImage;
        [SerializeField] [Tooltip("The Image for the icon of the item that is treated as an ingredient for the repair")]
        private Image ingredientItemImage;
        
        private bool _canRepair;
        private Item _ingredient;
        private PlayerInventory _inventory;
        private bool _processRunning;
        private ToolItem _tool;

        private void Start()
        {
            _inventory = GetComponent<PlayerInventory>();

            rootObject.SetActive(false);
            issueText.gameObject.SetActive(false);
            barObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) ToggleRepairUI();

            if (!ValidateConditions()) return;

            issueText.gameObject.SetActive(false);
            contentObject.SetActive(true);

            SynchronizeData();
        }

        private bool ValidateConditions()
        {
            if (!rootObject.activeSelf) return false;

            var selectedItem = _inventory.GetSelectedItem();

            if (selectedItem == null)
            {
                DisplayIssue("You're not holding an item!");
                return false;
            }

            if (!(selectedItem is ToolItem tool))
            {
                DisplayIssue($"{selectedItem.label} is not a tool!");
                return false;
            }

            _tool = tool;

            if (_tool.HasFullDurability())
            {
                DisplayIssue($"{selectedItem.label} has full durability!");
                return false;
            }

            return true;
        }

        private void SynchronizeData()
        {
            inputItemImage.sprite = _tool.GetComponent<SpriteRenderer>().sprite;

            _ingredient = ingredientRegistry
                .Select(entry => entry.GetComponent<Item>())
                .FirstOrDefault(entryItem => entryItem.label == _tool.MaterialLabel);

            if (_ingredient == null)
            {
                DisplayIssue($"Couldn't find an ingredient for repairing {_tool.label}!");
                return;
            }

            ingredientItemImage.sprite = _ingredient.GetComponent<SpriteRenderer>().sprite;

            var ingredientAmount = _inventory.CountOfItem(_ingredient);
            if (ingredientAmount > _tool.RepairCost) ingredientAmount = _tool.RepairCost;

            Color statusColor;
            if (ingredientAmount == _tool.RepairCost) statusColor = fulfilledColor;
            else if (ingredientAmount > 0 && ingredientAmount < _tool.RepairCost) statusColor = partiallyFulfilledColor;
            else statusColor = missingColor;

            ingredientStatusText.color = statusColor;
            ingredientStatusText.text = $"{ingredientAmount}/{_tool.RepairCost}";

            _canRepair = ingredientAmount == _tool.RepairCost;

            repairButtonText.color = _canRepair ? fulfilledColor : missingColor;
        }

        public void ToggleRepairUI()
        {
            rootObject.SetActive(!rootObject.activeSelf);
        }

        public void Repair()
        {
            if (ValidateConditions() && _canRepair && !_processRunning) StartCoroutine(RepairInternal());
        }

        private IEnumerator RepairInternal()
        {
            _processRunning = true;

            barObject.SetActive(true);
            barImage.fillAmount = 0f;

            while (barImage.fillAmount < 1f)
            {
                barImage.fillAmount += 0.01f / processTime;
                yield return new WaitForSeconds(0.01f);
            }

            barImage.fillAmount = 1f;
            barObject.SetActive(false);

            _inventory.ExtractItem(_ingredient, _tool.RepairCost);
            _tool.Repair();

            _processRunning = false;
        }

        private void DisplayIssue(string issue)
        {
            contentObject.SetActive(false);
            issueText.gameObject.SetActive(true);
            issueText.text = issue;
        }
    }
}