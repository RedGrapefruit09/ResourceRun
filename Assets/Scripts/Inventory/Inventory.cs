using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
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

    [SerializeField] private InventoryUISlot[] slots;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private GameObject tooltipBox;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemTooltipText;
    [SerializeField] private GameObject testItem;

    private readonly List<Item> _items = new List<Item>();
    private int _selectedItemSlot = 1;
    private Item _selectedItem;
    private Transform _playerTransform;
    private bool _tooltipShown;
    private const int InventorySize = 10;

    private void Start()
    {
        _playerTransform = FindObjectOfType<PlayerMovement>().transform;
        
        for (var i = 0; i < InventorySize; ++i)
        {
            _items.Add(null);
        }
        
        tooltipBox.SetActive(false);
        
        var clone = Instantiate(testItem);
        Insert(clone.GetComponent<Item>());
        
        SelectItem(1);
    }

    public Item Get(int slot)
    {
        return IsSlotInvalid(slot) ? null : _items[slot - 1];
    }

    public void Set(int slot, Item item)
    {
        if (IsSlotInvalid(slot)) return;

        _items[slot - 1] = item;
        UpdateUI();
    }
    
    public void Insert(Item insertedItem)
    {
        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = Get(i);

            // The slot is empty, so it can be filled with the inserted item
            if (item == null)
            {
                Set(i, insertedItem);
                return;
            }
            
            // The two items aren't of the same type
            if (item.label != insertedItem.label) continue;
            
            var space = item.maxCount - item.Amount;

            // There's not enough space to merge into this slot
            if (space < insertedItem.Amount) continue;
            
            // Merge two items together, adding together the amounts
            item.Increment(insertedItem.Amount);
            UpdateUI();
            Destroy(insertedItem.gameObject);
            return;
        }
    }
    
    public void SelectItem(int slot)
    {
        HideTooltip();
        
        if (_selectedItem != null)
        {
            _selectedItem.OnDeselected();
        }
        
        _selectedItemSlot = slot;
        _selectedItem = Get(slot);
        
        if (_selectedItem != null)
        {
            _selectedItem.OnSelected();
        }
        
        UpdateUI();
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
        itemTooltipText.text = _selectedItem.BuildTooltip();
    }

    private void UpdateUI()
    {
        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = Get(i);
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
        for (var i = 1; i <= SlotSelectBindings.Count; ++i)
        {
            var binding = SlotSelectBindings[i - 1];

            if (Input.GetKeyDown(binding))
            {
                SelectItem(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && _selectedItem != null)
        {
            var pos = new Vector3(_playerTransform.position.x, _playerTransform.position.y + 1f, 0f);
            var clone = Instantiate(droppedItemPrefab, pos, Quaternion.identity);
            clone.GetComponent<DroppedItem>().originalItem = _selectedItem;
            
            _selectedItem.OnDeselected();
            Set(_selectedItemSlot, null);
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

        if (_tooltipShown && _selectedItem != null)
        {
            itemTooltipText.text = _selectedItem.BuildTooltip();
        }
    }
    
    private static bool IsSlotInvalid(int slot)
    {
        if (slot > 0 && slot <= InventorySize) return false;
        
        Log.Error($"Tried to get/set an Item to/from an invalid slot: {slot}");
        return true;
    }
}

[Serializable]
public struct InventoryUISlot
{
    public Image itemImage;
    public Image selectorImage;
    public Text amountText;
}
