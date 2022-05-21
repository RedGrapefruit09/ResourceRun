using System;
using System.Linq;
using ResourceRun.Items;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceRun.Player
{
    /// <summary>
    /// A script responsible for the upgrades system implementation and UI screen.
    /// </summary>
    public class PlayerUpgrades : MonoBehaviour
    {
        [SerializeField] [Tooltip("A registry list of all existing item recipes to be queried for an applicable recipe")]
        private ItemRecipe[] recipes;

        [Header("Colors")] 
        [SerializeField] [Tooltip("The color representing the state, in which enough or more than enough of the ingredient has been obtained")]
        private Color fulfilledColor = Color.green;
        [SerializeField] [Tooltip("The color representing the state, in which the ingredient requirement has partially been fulfilled")]
        private Color partiallyFulfilledColor = Color.yellow;
        [SerializeField] [Tooltip("The color representing the state, in which no amount of the ingredient has been obtained")]
        private Color missingColor = Color.red;

        [Header("UI")]
        [SerializeField] [Tooltip("The root GameObject for the UI screen")]
        private GameObject rootUIObject;
        [SerializeField] [Tooltip("The content GameObject for the inner contents of the UI screen")]
        private GameObject contentRootObject;
        [SerializeField] [Tooltip("The reference to the name text of the outputted item")]
        private Text outputNameText;
        [SerializeField] [Tooltip("The reference to the text on the upgrade button")]
        private Text upgradeButtonText;
        [SerializeField] [Tooltip("The reference to the issue text")]
        private Text issueText;
        [SerializeField] [Tooltip("All of the ingredients' individual data wrapped in an IngredientUI struct")]
        private IngredientUI[] ingredients;
        
        private ItemRecipe _currentRecipe;
        private PlayerInventory _inventory;
        private Item _previousSelectedItem;

        private void Start()
        {
            _inventory = GetComponent<PlayerInventory>();

            rootUIObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U)) ToggleUpgradeUI();

            if (!rootUIObject.activeSelf) return;

            var selectedItem = _inventory.GetSelectedItem();

            if (selectedItem == null)
            {
                DisplayIssue("You aren't holding an item!");
                return;
            }

            if (Item.Different(selectedItem, _previousSelectedItem))
            {
                var recipe = recipes.FirstOrDefault(recipe =>
                    Item.Same(recipe.inputItem.GetComponent<Item>(), selectedItem));

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
                else if (count < ingredient.requirement) color = partiallyFulfilledColor;
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
                _inventory.ExtractItem(ingredient.item.GetComponent<Item>(), ingredient.requirement);

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
}