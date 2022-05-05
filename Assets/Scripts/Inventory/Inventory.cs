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
    [SerializeField] private GameObject testItem;

    private readonly List<Item> _items = new List<Item>();
    private int _selectedItemSlot = 1;
    private Item _selectedItem;
    private const int InventorySize = 10;

    private void Start()
    {
        for (var i = 0; i < InventorySize; ++i)
        {
            _items.Add(null);
        }
        
        SelectItem(1);
        var clone = Instantiate(testItem);
        Set(1, clone.GetComponent<Item>());
    }

    private static bool IsSlotInvalid(int slot)
    {
        if (slot > 0 && slot <= InventorySize) return false;
        
        Log.Error($"Tried to get/set an Item to/from an invalid slot: {slot}");
        return true;
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

    private void UpdateUI()
    {
        for (var i = 1; i <= InventorySize; ++i)
        {
            var item = _items[i - 1];
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

    public void SelectItem(int slot)
    {
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
    }
}

[Serializable]
public struct InventoryUISlot
{
    public Image itemImage;
    public Image selectorImage;
    public Text amountText;
}
