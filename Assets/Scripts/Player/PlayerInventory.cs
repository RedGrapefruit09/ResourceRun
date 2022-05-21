using System;
using System.Collections;
using System.Collections.Generic;
using ResourceRun.Items;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceRun.Player
{
    /// <summary>
    /// The sophisticated management class for the player inventory system, which deals with the internal inventory state, the front-facing
    /// API, the backend logic and controls the inventory UI/HUD.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        #region Data & State

        /// <summary>
        /// A list of input <see cref="KeyCode"/>s that correspond to keyboard keys for selecting inventory slots.
        /// It is queried by index starting from zero, meaning that, for slot 1, the 0th index will be looked up.
        /// This must be hard-coded as there's currently no way of automatically grouping together these alphanumeric <see cref="KeyCode"/>s.
        /// </summary>
        private static readonly List<KeyCode> SlotSelectBindings = new List<KeyCode>
        {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        /// <summary>
        /// The hard-coded constant inventory size.
        /// This inventory is only represented by the hotbar, there is no extra big popup UI with the rest of the slots.
        /// </summary>
        private const int InventorySize = 10;

        [SerializeField] [Tooltip("A list of UI descriptions of every slot with links to the relevant UI components")]
        private InventoryUISlot[] slots;
        [SerializeField] [Tooltip("A prefab for dropped item instances")]
        private GameObject droppedItemPrefab;
        [SerializeField] [Tooltip("The GameObject for the tooltip UI/box")]
        private GameObject tooltipBox;
        [SerializeField] [Tooltip("The Text of the label/name of the shown item")]
        private Text itemNameText;
        [SerializeField] [Tooltip("The Text containing the primary tooltip string")]
        private Text itemTooltipText;
        [SerializeField] [Tooltip("A list of item prefabs to be instantiated and given out when the game starts (initial set of items)")]
        private GameObject[] startupItems;
        [SerializeField] [Tooltip("A reference to the default Unity UISprite placeholder")]
        private Sprite uiSprite;
        [SerializeField] [Tooltip("The GameObject of the button for canceling the swap selection to be shown and hidden")]
        private GameObject cancelButton;

        private readonly List<Item> _items = new List<Item>();
        private int _selectedItemSlot = 1;
        private Item _selectedItem;
        private Transform _playerTransform;
        private bool _tooltipShown;
        private int _firstSwapSlot = -1;
        private int _secondSwapSlot = -1;
        private bool _isSwapping;

        public bool BlockSelection { private get; set; }

        #endregion

        private void Start()
        {
            _playerTransform = FindObjectOfType<PlayerMovement>().transform;

            for (var i = 0; i < InventorySize; ++i) _items.Add(null);

            tooltipBox.SetActive(false);
            cancelButton.SetActive(false);

            foreach (var startupItem in startupItems) Item.CreateAndInsert(startupItem, this);

            SelectItem(1);
        }
        
        #region API

        /// <summary>
        /// Checks if the given slot is valid and returns the item at the given slot in the inventory.
        /// </summary>
        /// <param name="slot">The slot to be queried</param>
        /// <returns>The item currently at that given slot</returns>
        /// <remarks><see langword="null"/> can also be returned if the slot is invalid or if there is no item at that slot</remarks>
        public Item GetItem(int slot)
        {
            return IsSlotInvalid(slot) ? null : _items[slot - 1];
        }

        /// <summary>
        /// Sets an item to the given slot, if that slot is valid.
        /// If the given slot is the one being selected, the internal cache will also be updated and a notification may be sent out.
        /// </summary>
        /// <param name="slot">The inventory slot to be updated</param>
        /// <param name="item">The new <see cref="Item"/> value. Can be set to <see langword="null"/></param>
        public void SetItem(int slot, Item item)
        {
            if (IsSlotInvalid(slot)) return;

            _items[slot - 1] = item;

            if (slot == _selectedItemSlot)
            {
                _selectedItem = item;

                if (item != null) _selectedItem.OnSelected();
            }
        }

        /// <summary>
        /// If there's enough space in the inventory, inserts the given item at an empty slot or a slot with enough space.
        /// </summary>
        /// <param name="insertedItem">The inserted <see cref="Item"/> instance, must not be <see langword="null"/>!</param>
        /// <returns><see langword="true"/> if the insertion operation was successful, <see langword="false"/> otherwise</returns>
        public bool InsertItem(Item insertedItem)
        {
            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);

                // The slot is empty, so it can be filled with the inserted item
                if (item == null)
                {
                    SetItem(i, insertedItem);
                    CombineDuplicates();
                    return true;
                }

                // The two items aren't of the same type
                if (Item.Different(item, insertedItem)) continue;

                var space = item.maxCount - item.Amount;

                // There's not enough space to merge into this slot
                if (space < insertedItem.Amount) continue;

                // Merge two items together, adding together the amounts
                item.Increment(insertedItem.Amount);
                Destroy(insertedItem.gameObject);
                CombineDuplicates();
                return true;
            }

            return false;
        }

        /// <summary>
        /// If possible, extracts a certain amount of the given item from the inventory.
        /// </summary>
        /// <param name="extractedItem">The reference instance of the extracted <see cref="Item"/>. Will only be used for comparison</param>
        /// <param name="amount">The desired amount of the given item to be extracted</param>
        public void ExtractItem(Item extractedItem, int amount = 1)
        {
            CombineDuplicates();

            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);

                if (item == null) continue;
                if (Item.Different(item, extractedItem)) continue;

                item.Decrement(amount);

                if (item.Amount <= 0)
                {
                    SetItem(i, null);
                    Destroy(item.gameObject);
                }
            }
        }

        /// <summary>
        /// Counts the amount of the given item that is currently present in the inventory.
        /// </summary>
        /// <param name="countedItem">The reference instance of the <see cref="Item"/>, only used for comparison</param>
        /// <returns>The exact amount of that item currently present</returns>
        public int CountOfItem(Item countedItem)
        {
            var amount = 0;

            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);

                if (Item.Same(item, countedItem)) amount += item.Amount;
            }

            return amount;
        }

        /// <summary>
        /// Shifts all slots' contents from the position after the given slot by the given offset backwards.
        /// No checks are made in regards for the offset, so be careful when calling this!
        /// </summary>
        /// <param name="slot">The slot, after which the shifted slots start</param>
        /// <param name="offset">The exact offset, by which the proceeding slots should be shifted backwards</param>
        private void ShiftBackFrom(int slot, int offset)
        {
            if (slot >= InventorySize) return;

            for (var i = slot + 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);
                SetItem(i, null);
                SetItem(i - offset, item);
            }
        }

        /// <summary>
        /// Compresses and compacts together everything in the inventory.
        /// This involves combining duplicates and shifting backwards all items to the left as much as possible.
        /// </summary>
        /// <seealso cref="ShiftBackFrom"/>
        public void Compress()
        {
            CombineDuplicates();

            for (var k = 0; k < InventorySize; ++k)
            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);

                if (item != null) continue;

                var offset = 0;

                for (var j = i; j > 0; --j)
                {
                    if (GetItem(j) != null) break;
                    offset++;
                }

                ShiftBackFrom(i, offset);
            }
        }

        /// <summary>
        /// Combines duplicate items of the same type (label) together into a single item representing that type, if the
        /// <see cref="Item.Amount"/> and <see cref="Item.maxCount"/> allow it.
        /// </summary>
        private void CombineDuplicates()
        {
            var encounters = new Dictionary<string, int>();

            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);

                if (item == null) continue;

                if (encounters.ContainsKey(item.label))
                {
                    var slot = encounters[item.label];
                    var sourceItem = GetItem(slot);
                    sourceItem.Increment(item.Amount);
                    RemoveItem(item);
                }
                else
                {
                    encounters.Add(item.label, i);
                }
            }
        }

        /// <summary>
        /// Destroys and removes the item at the given slot from the inventory.
        /// The slot of the given item is detected automatically.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> instance to be destroyed and removed</param>
        public void RemoveItem(Item item)
        {
            var slot = _items.IndexOf(item) + 1;
            SetItem(slot, null);
            Destroy(item.gameObject);
        }

        /// <summary>
        /// Returns the currently selected (held) item, with no guarantees of it being not <see langword="null"/>.
        /// </summary>
        /// <returns>The currently selected item</returns>
        public Item GetSelectedItem()
        {
            return _selectedItem;
        }

        #endregion

        #region Tooltips

        /// <summary>
        /// If the tooltip for the selected item is currently being shown, hides it, else, shows it.
        /// </summary>
        public void TriggerTooltip()
        {
            if (_tooltipShown)
                HideTooltip();
            else
                ShowTooltip();
        }

        private void HideTooltip()
        {
            tooltipBox.SetActive(false);
            _tooltipShown = false;
        }

        private void ShowTooltip()
        {
            if (_selectedItem == null) return;

            _tooltipShown = true;
            tooltipBox.SetActive(true);
            itemNameText.text = _selectedItem.label;
            itemTooltipText.text = _selectedItem.GetTooltip();
        }

        private void UpdateTooltipUI()
        {
            if (_tooltipShown)
            {
                if (_selectedItem != null)
                {
                    itemTooltipText.text = _selectedItem.GetTooltip();
                }
                else
                {
                    _tooltipShown = false;
                    HideTooltip();
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
            {
                if (_tooltipShown)
                    HideTooltip();
                else
                    ShowTooltip();
            }
        }

        #endregion

        #region Item Swapping

        /// <summary>
        /// Tries to swap the item at the given slot with the previous registered slot.
        /// </summary>
        /// <param name="slot">The attempted slot to be swapped from/with.</param>
        public void TrySwapItem(int slot)
        {
            cancelButton.SetActive(true);

            if (_firstSwapSlot == -1)
            {
                _firstSwapSlot = slot;
                return;
            }

            _secondSwapSlot = slot;

            if (!_isSwapping) StartCoroutine(PerformItemSwap());
        }

        private IEnumerator PerformItemSwap()
        {
            _isSwapping = true;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            if (_firstSwapSlot == -1 && _secondSwapSlot == -1) yield break;

            var firstItem = GetItem(_firstSwapSlot);
            var secondItem = GetItem(_secondSwapSlot);

            SetItem(_firstSwapSlot, secondItem);
            SetItem(_secondSwapSlot, firstItem);

            if (_firstSwapSlot == _selectedItemSlot && firstItem != null) firstItem.OnDeselected();

            if (_secondSwapSlot == _selectedItemSlot && secondItem != null) secondItem.OnDeselected();

            _firstSwapSlot = -1;
            _secondSwapSlot = -1;

            _isSwapping = false;

            CancelSwap();
        }

        /// <summary>
        /// Cancels or suspends the swap or swap setup that is currently taking place.
        /// </summary>
        public void CancelSwap()
        {
            cancelButton.SetActive(false);
            _firstSwapSlot = -1;
            _secondSwapSlot = -1;
            StopCoroutine(nameof(PerformItemSwap));
            _isSwapping = false;
        }

        #endregion

        #region Item Operations

        /// <summary>
        /// Selects the item at the given slot, no matter whether it is <see langword="null"/>.
        /// </summary>
        /// <param name="slot">The slot to be selected</param>
        public void SelectItem(int slot)
        {
            if (BlockSelection) return;

            HideTooltip();

            if (_selectedItem != null) _selectedItem.OnDeselected();

            _selectedItemSlot = slot;
            _selectedItem = GetItem(slot);

            if (_selectedItem != null) _selectedItem.OnSelected();
        }

        private void SelectWithKeyboard()
        {
            if (BlockSelection) return;

            for (var i = 1; i <= SlotSelectBindings.Count; ++i)
            {
                var binding = SlotSelectBindings[i - 1];

                if (Input.GetKeyDown(binding)) SelectItem(i);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                var newSlot = _selectedItemSlot + 1;
                if (newSlot > InventorySize) newSlot = 1;
                SelectItem(newSlot);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                var newSlot = _selectedItemSlot - 1;
                if (newSlot < 1) newSlot = InventorySize;
                SelectItem(newSlot);
            }
        }

        private void DropItem()
        {
            var pos = new Vector3(_playerTransform.position.x, _playerTransform.position.y + 1f, 0f);
            Item.Drop(droppedItemPrefab, pos, _selectedItem);

            _selectedItem = null;
            SetItem(_selectedItemSlot, null);
        }

        #endregion

        #region Updates

        private void UpdateUI()
        {
            for (var i = 1; i <= InventorySize; ++i)
            {
                var item = GetItem(i);
                var slot = slots[i - 1];

                slot.selectorImage.gameObject.SetActive(i == _selectedItemSlot);

                if (i == _firstSwapSlot)
                {
                    slot.swapHintText.gameObject.SetActive(true);
                    slot.swapHintText.text = "1";
                }
                else
                {
                    if (i == _secondSwapSlot)
                    {
                        slot.swapHintText.gameObject.SetActive(true);
                        slot.swapHintText.text = "2";
                    }
                    else
                    {
                        slot.swapHintText.gameObject.SetActive(false);
                    }
                }

                if (item == null)
                {
                    slot.amountText.gameObject.SetActive(false);
                    slot.itemImage.sprite = uiSprite;
                    slot.itemImage.color = new Color(0, 0, 0, 0);
                    continue;
                }

                slot.amountText.gameObject.SetActive(true);
                slot.amountText.text = item.Amount.ToString();
                slot.itemImage.gameObject.SetActive(true);
                slot.itemImage.color = Color.white;
                slot.itemImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
            }
        }

        private void Update()
        {
            SelectWithKeyboard();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q) && _selectedItem != null) DropItem();
            UpdateUI();
            UpdateTooltipUI();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) Compress();
        }

        #endregion
        
        private static bool IsSlotInvalid(int slot)
        {
            if (slot >= -1 && slot <= InventorySize) return false;

            Debug.LogError($"Tried to get/set an Item to/from an invalid slot: {slot}");
            return true;
        }

        [Serializable]
        public struct InventoryUISlot
        {
            public Image itemImage;
            public Image selectorImage;
            public Text amountText;
            public Text swapHintText;
        }
    }
}