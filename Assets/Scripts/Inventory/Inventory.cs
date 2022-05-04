using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUISlot[] slots;
    [SerializeField] private GameObject testItem;

    private readonly List<ItemStack> _stacks = new List<ItemStack>();
    private const int InventorySize = 10;

    private void Start()
    {
        for (var i = 0; i < InventorySize; ++i)
        {
            _stacks.Add(null);
        }

        foreach (var slot in slots)
        {
            slot.amountText.gameObject.SetActive(false);
            slot.itemImage.gameObject.SetActive(false);
            slot.selectorImage.gameObject.SetActive(false);
        }
    }

    private bool IsSlotInvalid(int slot)
    {
        if (slot > 0 && slot <= InventorySize) return false;
        
        Log.Error($"Tried to query an ItemStack from an invalid slot: {slot}");
        return true;
    }

    public ItemStack Get(int slot)
    {
        if (IsSlotInvalid(slot)) return null;

        return _stacks[slot + 1];
    }

    public void Set(int slot, ItemStack stack)
    {
        if (IsSlotInvalid(slot)) return;

        _stacks[slot + 1] = stack;
    }
}

[Serializable]
public struct InventoryUISlot
{
    public Image itemImage;
    public Image selectorImage;
    public Text amountText;
}
