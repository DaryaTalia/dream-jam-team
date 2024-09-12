using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public int maxItemSlots = 6;
    public int availableItemSlots;
    [SerializeField] List<ItemStack> itemInventory;

    private void Awake()
    {
        availableItemSlots = maxItemSlots;
    }

    public List<BaseItem> GetAvailableBaseItems()
    {
        return itemInventory.Select(i => i.baseItem).ToList();
    }

    public bool AddItemToInventory(BaseItem _item, int quantity)
    {
        // Resource Checks
        if (_item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return false;
        }

        if (availableItemSlots < _item.size)
        {
            Debug.Log("No space for resource");
            return false;
        }

        itemInventory.Add(new ItemStack(_item, quantity));
        availableItemSlots -= _item.size;
        return true;
    }

    public void RemoveItemFromInventory(BaseItem _item, int quantity)
    {
        // Resource Checks
        if (_item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return;
        }

        List<ItemStack> relevantStacks = itemInventory.FindAll(i => i.baseItem == _item);
        int removedCount = 0;
        foreach (ItemStack relevantStack in relevantStacks)
        {
            if (removedCount == quantity)
            {
                break;
            }

            var needToRemove = quantity - removedCount;
            if (relevantStack.quantity <= needToRemove)
            {
                removedCount += relevantStack.quantity;
                itemInventory.Remove(relevantStack);
            }
            else
            {
                removedCount += needToRemove;
                relevantStack.quantity -= needToRemove;
            }
        }
        itemInventory.Remove(itemInventory.First(i => i.baseItem == _item));
        availableItemSlots += _item.size;
    }

    public void ClearInventory()
    {
        itemInventory.Clear();
    }
}