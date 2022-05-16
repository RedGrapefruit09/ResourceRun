using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRepair : MonoBehaviour
{
    [Header("Functionality")]
    [SerializeField] private GameObject[] ingredientRegistry;
    [SerializeField] private float processTime;
    
    [Header("Colors")]
    [SerializeField] private Color fulfilledColor = Color.green;
    [SerializeField] private Color partiallyFulfilledColor = Color.yellow;
    [SerializeField] private Color missingColor = Color.red;

    [Header("UI")]
    [SerializeField] private GameObject rootObject;
    [SerializeField] private GameObject contentObject;
    [SerializeField] private Text issueText;
    [SerializeField] private Text repairButtonText;
    [SerializeField] private Text ingredientStatusText;
    [SerializeField] private GameObject barObject;
    [SerializeField] private Image barImage;
    [SerializeField] private Image inputItemImage;
    [SerializeField] private Image ingredientItemImage;

    private PlayerInventory _inventory;
    private Item _ingredient;
    private bool _processRunning;
    private bool _canRepair;
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
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            ToggleRepairUI();
        }

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
        if (ValidateConditions() && _canRepair && !_processRunning)
        {
            StartCoroutine(RepairInternal());
        }
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