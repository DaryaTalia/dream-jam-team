using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Cargo Resources")]
    [SerializeField] private List<BaseItem> possibleResources;
    [SerializeField] private Inventory storeInventory;

    public bool BuyResource(BaseItem item)
    {
        if (GameManager.Instance.Gold < item.cost)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        GameManager.Instance.Gold -= item.cost;
        //IncrementResourceItem(_name, 1);
        //TODO: Increment inventory in cargoController
        storeInventory.RemoveItemFromInventory(item, 1);
        return true;
    }

    public void ResetStoreInventory()
    {
        storeInventory.ClearInventory();
        foreach (BaseItem resource in possibleResources)
        {
            storeInventory.AddItemToInventory(resource, 1);
        }
    }
    
}

