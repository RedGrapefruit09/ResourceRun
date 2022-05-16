using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgrades : MonoBehaviour
{
    [SerializeField] private ToolRecipe[] recipes;
    
    [Header("Colors")]
    [SerializeField] private Color fulfilledColor;
    [SerializeField] private Color beingCollectedColor;
    [SerializeField] private Color missingColor;
    
    [Header("UI")]
    [SerializeField] private GameObject rootUIObject;
    [SerializeField] private GameObject contentRootObject;
    [SerializeField] private Text outputNameText;
    [SerializeField] private Text upgradeButtonText;
    [SerializeField] private Text issueText;
    [SerializeField] private IngredientUI[] ingredients;

    private PlayerInventory _inventory;
    private Item _previousSelectedItem;
    private ToolRecipe _currentRecipe;
    
    private void Start()
    {
        _inventory = GetComponent<PlayerInventory>();
        
        rootUIObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgradeUI();
        }

        if (!rootUIObject.activeSelf) return;

        var selectedItem = _inventory.GetSelectedItem();
        
        if (selectedItem == null)
        {
            DisplayIssue("You aren't holding an item!");
            return;
        }

        if (Item.Different(selectedItem, _previousSelectedItem))
        {
            var recipe = recipes.FirstOrDefault(recipe => Item.Same(recipe.inputItem.GetComponent<Item>(), selectedItem));
            
            if (recipe == null)
            {
                DisplayIssue($"No recipes are available for a {selectedItem.label}");
                return;
            }
            
            _currentRecipe = recipe;
        }

        _previousSelectedItem = selectedItem;

        if (_currentRecipe == null) return;
        
        issueText.gameObject.SetActive(false);
        contentRootObject.SetActive(true);
        
        SynchronizeData();
    }

    private void DisplayIssue(string issue)
    {
        contentRootObject.SetActive(false);
        issueText.gameObject.SetActive(true);
        issueText.text = issue;
    }

    private void SynchronizeData()
    {
        outputNameText.text = _currentRecipe.outputItem.GetComponent<Item>().label;
        upgradeButtonText.color = AreIngredientsFulfilled() ? fulfilledColor : missingColor;

        for (var i = 0; i < ingredients.Length; ++i)
        {
            var ui = ingredients[i];
        
            if (i >= _currentRecipe.ingredients.Length)
            {
                ui.iconImage.gameObject.SetActive(false);
                ui.nameText.gameObject.SetActive(false);
                ui.statusText.gameObject.SetActive(false);
                continue;
            }

            var ingredient = _currentRecipe.ingredients[i];

            var count = _inventory.CountOfItem(ingredient.item.GetComponent<Item>());
            if (count > ingredient.requirement) count = ingredient.requirement;
            
            Color color;
            if (count == 0) color = missingColor;
            else if (count < ingredient.requirement) color = beingCollectedColor;
            else color = fulfilledColor;

            ui.iconImage.sprite = ingredient.item.GetComponent<SpriteRenderer>().sprite;
            ui.nameText.text = ingredient.item.GetComponent<Item>().label;
            ui.nameText.color = color;
            ui.statusText.text = $"{count}/{ingredient.requirement}";
            ui.statusText.color = color;
        }
    }

    private bool AreIngredientsFulfilled()
    {
        var fulfilledIngredients =
            from ingredient in _currentRecipe.ingredients
            let count = _inventory.CountOfItem(ingredient.item.GetComponent<Item>())
            where count >= ingredient.requirement
            select ingredient;

        return fulfilledIngredients.Count() == _currentRecipe.ingredients.Length;
    }

    public void ToggleUpgradeUI()
    {
        rootUIObject.SetActive(!rootUIObject.activeSelf);
    }

    public void Upgrade()
    {
        if (_currentRecipe == null) return;
        if (!AreIngredientsFulfilled()) return;

        foreach (var ingredient in _currentRecipe.ingredients)
        {
            _inventory.ExtractItem(ingredient.item.GetComponent<Item>(), ingredient.requirement);
        }

        _inventory.ExtractItem(_currentRecipe.inputItem.GetComponent<Item>());
        
        Item.CreateAndInsert(_currentRecipe.outputItem, _inventory);
    }
    
    [Serializable]
    public struct IngredientUI
    {
        public Image iconImage;
        public Text nameText;
        public Text statusText;
    }
}
