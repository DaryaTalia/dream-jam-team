using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    // Cargo Vehicle
    [Header("Cargo")]
    [SerializeField]
    GameObject _cargoVehiclePrefab;
    Transform _cargoTransform;

    // Cargo Wellness Attributes
    [Header("Cargo Wellness")]
    [SerializeField]
    float _health = _maxHealth;
    const float _maxHealth = 100;
    public float Health {
        get => _health;
        set
        {
            if(value > _maxHealth)
            {
                _health = _maxHealth;
            } else if(value < 0)
            {
                _health = 0;
            } else
            {
                _health = value;
            }
        }
    }

    [SerializeField]
    float _defense = 0;
    const float _maxDefense = 100;
    public float Defense
    {
        get => _defense;
        set
        {
            if (value > _maxDefense)
            {
                _defense = _maxDefense;
            }
            else if (value < 0)
            {
                _defense = 0;
            } else
            {
                _defense = value;
            }
        }
    }

    // Items for Delivery

    [Header("Cargo Items and Buffs")]
    [SerializeField]
    int _availableItemSlots = _maxItemSlots;
    const int _maxItemSlots = 6;
    [SerializeField]
    List<BaseItem> _itemInventory;
    public List<BaseItem> ItemInventory
    {
        get => _itemInventory;
    }

    public bool AddItemToInventory(BaseItem _item)
    {
        // Item Checks
        if(_item.itemType != ItemType.Deliverable || _item.itemType != ItemType.Buff)
        {
            Debug.Log("Invalid Item Type");
            return false;
        }

        if(_availableItemSlots < _item.size)
        {
            Debug.Log("No space for item");
            return false;
        }

        ItemInventory.Add(_item);
        _availableItemSlots -= _item.size;
        return true;
    }

    public void RemoveItemFromInventory(BaseItem _item)
    {
        // Item Checks
        if (_item.itemType != ItemType.Deliverable || _item.itemType != ItemType.Buff)
        {
            Debug.Log("Invalid Item Type");
            return;
        }

        ItemInventory.Remove(_item);
        _availableItemSlots += _item.size;
    }

    [Header("Cargo Resources")]
    [SerializeField]
    int _availableResourceSlots = _maxResourceSlots;
    const int _maxResourceSlots = 3;
    [SerializeField]
    List<BaseItem> _resourceInventory;
    public List<BaseItem> ResourceInventory
    {
        get => _resourceInventory;
    }

    // Consumable Resources

    public bool AddResourceToInventory(BaseItem _item)
    {
        // Resource Checks
        if (_item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return false;
        }

        if (_availableResourceSlots < _item.size)
        {
            Debug.Log("No space for resource");
            return false;
        }

        ResourceInventory.Add(_item);
        _availableResourceSlots -= _item.size;
        return true;
    }

    public void RemoveResourceFromInventory(BaseItem _item)
    {
        // Resource Checks
        if (_item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return;
        }

        ResourceInventory.Remove(_item);
        _availableResourceSlots += _item.size;
    }

    // Speed and Travel

    [Header("Speed and Travel")]
    // How fast can the cargo vehicle move and is that speed being modified
    [SerializeField]
    [Range(0, _maxSpeed)]
    float _speed = _maxSpeed / 2;
    const float _maxSpeed = 4f;
    [SerializeField]
    [Range(1, _maxSpeed)]
    float _speedModifier = 1f;
    int _speedDelta = 40;      // To slow the speed in relation to Unity's frames

    public float Speed
    {
        get => _speed * SpeedModifier;
        set
        {
            if(value < 0)
            {
                _speed = 0;
            }
            else if(value > _maxSpeed)
            {
                _speed = _maxSpeed;
            }
            else
            {
                _speed = value;
            }
        }
    }

    public float SpeedModifier
    {
        get => _speedModifier;
        set
        {
            if(value < 1)
            {
                _speedModifier = 1;
            } else
            {
                _speedModifier = value;
            }
        }
    }

    // How high or low on the screen can the cargo vehicle travel
    [SerializeField]
    float _heightUpperLimit;
    [SerializeField]
    float _heightLowerLimit;
    [SerializeField]
    [Range(1, _maxSpeed)]
    float _heightSpeed = 2f;

    // What height is the cargo vehicle moving towards currently
    [SerializeField]
    float _heightTarget;
    Vector3 _targetPosition;

    [SerializeField]
    float _distanceTraveled = 0;
    public float DistanceTraveled
    {
        get => _distanceTraveled;
        set
        {
            _distanceTraveled = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _cargoTransform = _cargoVehiclePrefab.transform;
    }


    void FixedUpdate()
    {
        if(Health > 0)
        {
            Move();
        }
    }

    // Game options

    void Move()
    {
        _targetPosition = new Vector3(_cargoTransform.position.x + (Speed / _speedDelta), Mathf.Lerp(_cargoTransform.position.y, _heightTarget, (_heightSpeed / 100)), 0);
        _cargoTransform.position = _targetPosition;
        _distanceTraveled += (Speed / _speedDelta);
        Debug.Log("Distance Traveled: " + _distanceTraveled);
    }

    // UpdateDistanceTraveled()
    // UseResource(BaseItem item)
    // DeliverItem()

}
