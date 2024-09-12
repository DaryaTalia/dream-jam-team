using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Cargo Resources")]
    [SerializeField]
    List<ItemStack> _primaryResourceInventory;
    public List<ItemStack> PrimaryResourceInventory
    {
        get => _primaryResourceInventory;
    }

    private ItemStack GetResourceItem(string _name)
    {
        foreach(ItemStack _resourceItem in PrimaryResourceInventory)
        {
            if(_resourceItem.baseItem.itemName == _name)
            {
                return _resourceItem;
            }
        }
        return null;
    }

    public void IncrementResourceItem(string _name, int _value)
    {
        foreach (ItemStack _resourceItem in PrimaryResourceInventory)
        {
            if (_resourceItem.baseItem.itemName == _name)
            {
                _resourceItem.quantity += Mathf.Abs(_value);
                return;
            }
        }
    }

    public void DecrementResourceItem(string _name, int _value)
    {
        foreach (ItemStack _resourceItem in PrimaryResourceInventory)
        {
            if (_resourceItem.baseItem.itemName == _name)
            {
                _resourceItem.quantity -= Mathf.Abs(_value);
                return;
            }
        }
    }

    public bool BuyResource(string _name)
    {
        if (GameManager.Instance.Gold < GetResourceItem(_name).baseItem.cost)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        GameManager.Instance.Gold -= GetResourceItem(_name).baseItem.cost;
        IncrementResourceItem(_name, 1);
        return true;
    }

    public bool EquipResource(string _name)
    {
        if (GetResourceItem(_name).quantity < 1)
        {
            Debug.Log("Not enough resources.");
            return false;
        }

        bool result = GameManager.Instance.cargoController.AddResourceToInventory(GetResourceItem(_name).baseItem, 1);

        if(result)
        {
            DecrementResourceItem(_name, 1);
        }
        return result;
    }

    public bool UnquipResource(string _name)
    {     

        if (GameManager.Instance.cargoController.ResourceInventory.Find(i => i.baseItem.itemName == _name).quantity < 1)
        {
            Debug.Log("Not enough resources equipped.");
            return false;
        }

        ItemStack _toRemove = GameManager.Instance.cargoController.ResourceInventory.Find(i => i.baseItem.itemName == _name);

        GameManager.Instance.cargoController.ResourceInventory.Remove(_toRemove);

        return true;
    }

    void Start()
    {
        var spawnManager = GameManager.Instance.spawnManager;
    }
}

