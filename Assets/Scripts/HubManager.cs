using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Cargo Resources")]
    [SerializeField]
    List<ResourceItem> _primaryResourceInventory;
    public List<ResourceItem> PrimaryResourceInventory
    {
        get => _primaryResourceInventory;
    }

    private BaseItem GetResourceItem(string _name)
    {
        foreach(ResourceItem _resourceItem in PrimaryResourceInventory)
        {
            if(_resourceItem.baseItem.itemName == _name)
            {
                return _resourceItem.baseItem;
            }
        }
        return null;
    }

    public void IncrementResourceItem(string _name, int _value)
    {
        foreach (ResourceItem _resourceItem in PrimaryResourceInventory)
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
        foreach (ResourceItem _resourceItem in PrimaryResourceInventory)
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
        if (GameManager.Instance.Gold < GetResourceItem(_name).cost)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        GameManager.Instance.Gold -= GetResourceItem(_name).cost;
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

        bool result = GameManager.Instance.cargoControllerPrefab.GetComponent<CargoController>().AddResourceToInventory(GetResourceItem(_name));

        if(result)
        {
            DecrementResourceItem(_name, 1);
        }
        return result;
    }

    public bool UnquipResource(string _name)
    {
        int _quantity = 0;

        foreach(BaseItem _item in GameManager.Instance.cargoControllerPrefab.GetComponent<CargoController>().ResourceInventory)
        {
            if (_item.itemName == _name)
            {
                ++_quantity;
            }
        }

        if (_quantity < 1)
        {
            Debug.Log("Not enough resources equipped.");
            return false;
        }

        BaseItem _toRemove = null;

        foreach (BaseItem _item in GameManager.Instance.cargoControllerPrefab.GetComponent<CargoController>().ResourceInventory)
        {
            if (_item.itemName == _name)
            {
                _toRemove = _item;
                break;
            }
        }

        GameManager.Instance.cargoControllerPrefab.GetComponent<CargoController>().ResourceInventory.Remove(_toRemove);

        return true;
    }

    void Start()
    {
        var spawnManager = GameManager.Instance.spawnManager;
    }
}

