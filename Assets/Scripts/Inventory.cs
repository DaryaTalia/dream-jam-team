using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [SerializeField] ItemType allowedItemTypes;
    public int maxItemSlots = 6;
    private int availableItemSlots;
    public List<ItemStack> itemInventory;

    private void Awake()
    {
        availableItemSlots = maxItemSlots;
    }

    public List<BaseItem> GetAvailableBaseItems()
    {
        return itemInventory.Select(i => i.baseItem).ToList();
    }

    public int GetAvailableItemSlots()
    {
        return availableItemSlots;
    }

    public bool AddItemToInventory(BaseItem item, int quantity)
    {
        // Test if allowed item type
        if (!allowedItemTypes.HasFlag(item.itemType))
        {
            return false;
        }
        
        // Resource Checks
        if (item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return false;
        }

        if (availableItemSlots < item.size)
        {
            Debug.Log("No space for resource");
            return false;
        }

        itemInventory.Add(new ItemStack(item, quantity));
        availableItemSlots -= item.size;
        return true;
    }

    public void RemoveItemFromInventory(BaseItem item, int quantity)
    {
        // Resource Checks
        if (item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return;
        }

        List<ItemStack> relevantStacks = itemInventory.FindAll(i => i.baseItem == item);
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
        itemInventory.Remove(itemInventory.First(i => i.baseItem == item));
        availableItemSlots += item.size;
    }

    public void ClearInventory()
    {
        availableItemSlots = maxItemSlots;
        itemInventory.Clear();
    }
}