using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    #region Data & State
    
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
    
    private const int InventorySize = 10;

    [SerializeField] private InventoryUISlot[] slots;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private GameObject tooltipBox;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemTooltipText;
    [SerializeField] private GameObject[] startupItems;

    private readonly List<Item> _items = new List<Item>();
    private int _selectedItemSlot = 1;
    private Item _selectedItem;
    private Transform _playerTransform;
    private bool _tooltipShown;
    
    public bool BlockSelection { private get; set; }

    #endregion

    private void Start()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        
        for (var i = 0; i < InventorySize; ++i)
        {
            _items.Add(null);
        }
        
        tooltipBox.SetActive(false);
        
        foreach (var startupItem in startupItems)
        {
            Item.CreateAndInsert(startupItem, inventory: this);
        }
        
        SelectItem(1);
    }

    #region API

    public Item GetItem(int slot)
    {
        return IsSlotInvalid(slot) ? null : _items[slot - 1];
    }

    public void SetItem(int slot, Item item)
    {
        if (IsSlotInvalid(slot)) return;

        _items[slot - 1] = item;

        if (slot == _selectedItemSlot)
        {
            _selectedItem = item;

            if (item != null)
            {
                _selectedItem.OnSelected();
            }
        }
    }
    
    public bool InsertItem(Item insertedItem)
    {
        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = GetItem(i);

            // The slot is empty, so it can be filled with the inserted item
            if (item == null)
            {
                SetItem(i, insertedItem);
                Compress();
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
            Compress();
            return true;
        }

        return false;
    }

    public void ExtractItem(Item extractedItem, int amount)
    {
        Compress();

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

    public int CountOfItem(Item countedItem)
    {
        var amount = 0;

        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = GetItem(i);

            if (Item.Same(item, countedItem))
            {
                amount += item.Amount;
            }
        }
        
        return amount;
    }

    private void ShiftBackFrom(int slot)
    {
        if (slot >= InventorySize) return;
        
        for (var i = slot + 1; i <= InventorySize; ++i)
        {
            var item = GetItem(i);
            SetItem(i, null);
            SetItem(i - 1, item);
        }
    }
    
    private void Compress()
    {
        var encounters = new Dictionary<string, int>();

        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = GetItem(i);

            if (item == null)
            {
                ShiftBackFrom(i);
                continue;
            }

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

    public void RemoveItem(Item item)
    {
        var slot = _items.IndexOf(item) + 1;
        SetItem(slot, null);
        Destroy(item.gameObject);
    }

    public Item GetSelectedItem()
    {
        return _selectedItem;
    }
    
    #endregion

    #region Tooltips

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
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (_tooltipShown)
            {
                HideTooltip();
            }
            else
            {
                ShowTooltip();
            }
        }
    }
    
    #endregion

    #region Item Operations

    public void SelectItem(int slot)
    {
        if (BlockSelection) return;
        
        HideTooltip();
        
        if (_selectedItem != null)
        {
            _selectedItem.OnDeselected();
        }
        
        _selectedItemSlot = slot;
        _selectedItem = GetItem(slot);
        
        if (_selectedItem != null)
        {
            _selectedItem.OnSelected();
        }
    }

    private void SelectWithKeyboard()
    {
        if (BlockSelection) return;
        
        for (var i = 1; i <= SlotSelectBindings.Count; ++i)
        {
            var binding = SlotSelectBindings[i - 1];

            if (Input.GetKeyDown(binding))
            {
                SelectItem(i);
            }
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

            if (item == null)
            {
                slot.amountText.gameObject.SetActive(false);
                slot.itemImage.gameObject.SetActive(false);
                continue;
            }

            slot.amountText.gameObject.SetActive(true);
            slot.amountText.text = item.Amount.ToString();
            slot.itemImage.gameObject.SetActive(true);
            slot.itemImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void Update()
    {
        SelectWithKeyboard();
        if (Input.GetKeyDown(KeyCode.Q) && _selectedItem != null) DropItem();
        UpdateUI();
        UpdateTooltipUI();
    }
    
    #endregion
    
    private static bool IsSlotInvalid(int slot)
    {
        if (slot > 0 && slot <= InventorySize) return false;
        
        Debug.LogError($"Tried to get/set an Item to/from an invalid slot: {slot}");
        return true;
    }
    
    [Serializable]
    public struct InventoryUISlot
    {
        public Image itemImage;
        public Image selectorImage;
        public Text amountText;
    }
}
