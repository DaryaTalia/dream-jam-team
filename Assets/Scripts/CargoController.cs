using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    // Cargo Vehicle
    [Header("Cargo")]
    [SerializeField]
    GameObject _cargoVehiclePrefab;
    GameObject _cargoVehicleInstance;
    Transform _cargoTransform;

    public GameObject CargoVehicle
    {
        get => _cargoVehicleInstance;
    }

    // Cargo Wellness Attributes
    [Header("Cargo Wellness")]
    [SerializeField]
    float _health = _maxHealth;
    const float _maxHealth = 50;
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

    public float MaxHealth
    {
        get => _maxHealth;
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

    public float MaxDefense
    {
        get => _maxDefense;
    }

    // Items for Delivery

    [Header("Cargo Items and Buffs")]
    [SerializeField] private Inventory itemInventory;
    public bool AddItemToInventory(BaseItem item, int quantity)
    {
        return itemInventory.AddItemToInventory(item, quantity);
    }

    public void RemoveItemFromInventory(BaseItem item, int quantity)
    {
        itemInventory.RemoveItemFromInventory(item, quantity);
    }

    public int GetItemInventoryCount()
    {
        return itemInventory.itemInventory.Count();
    }

    public Inventory GetItemInventory()
    {
        return itemInventory;
    }

    [Header("Cargo Resources")]
    [SerializeField] Inventory resourceInventory;

    // Consumable Resources

    public bool AddResourceToInventory(BaseItem item, int quantity)
    {
        // Resource Checks
        if (item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return false;
        }

        return resourceInventory.AddItemToInventory(item, quantity);
    }

    public void RemoveResourceFromInventory(BaseItem item, int quantity)
    {
        // Resource Checks
        if (item.itemType != ItemType.Resource)
        {
            Debug.Log("Invalid Item Type");
            return;
        }

        resourceInventory.RemoveItemFromInventory(item, quantity);
    }

    public Inventory GetResourceInventory()
    {
        return resourceInventory;
    }


    // Speed and Travel

    [Header("Speed and Travel")]
    // How fast can the cargo vehicle move and is that speed being modified
    [SerializeField]
    [Range(0, _maxSpeed)]
    float _speed = 1;
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
    float _heightUpperLimit = 5;
    [SerializeField]
    float _heightLowerLimit = -5;
    [SerializeField]
    [Range(0, 2f)]
    float _heightSpeed = 1f;

    // What height is the cargo vehicle moving towards currently
    [SerializeField]
    float _heightTarget;
    Vector3 _targetPosition;
    float _heightPadding = 1;

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
    void Awake()
    {
        _heightTarget = Random.Range(_heightLowerLimit, _heightUpperLimit);
        _cargoVehicleInstance = Instantiate(_cargoVehiclePrefab);
        _cargoTransform = _cargoVehicleInstance.transform;
        _cargoVehicleInstance.SetActive(false);
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.stormMode.JourneyActive && Health > 0)
        {
            Move();
        }
    }

    // Game options

    void Move()
    {
        if(Mathf.Abs(_heightTarget) - Mathf.Abs(_cargoTransform.position.z) < _heightPadding)
        {
            _heightTarget = Random.Range(_heightLowerLimit, _heightUpperLimit);
        }

        _targetPosition = new Vector3(
            _cargoTransform.position.x + (Speed / _speedDelta), 
            0,
            Mathf.SmoothStep(_cargoTransform.position.z, _heightTarget, _heightSpeed * Time.deltaTime) );
        _cargoTransform.position = _targetPosition;

        _distanceTraveled += (Speed / _speedDelta);
        //Debug.Log("Distance Traveled: " + _distanceTraveled);
    }

    public void UseResource(ItemStack item)
    {
        if (item.baseItem.prefab)
        {
            Instantiate(item.baseItem.prefab);
        }
        
        RemoveResourceFromInventory(item.baseItem, 1);
    }

    public void DeliverItem()
    {
        RemoveItemFromInventory(itemInventory.itemInventory[0].baseItem, 1);
    }

    public void DamageCargo(float value)
    {
        Health -= value;
        UpdateCargohealthSlider();
    }

    public void HealCargo(float value)
    {
        Health += value;
        UpdateCargohealthSlider();
    }

    void UpdateCargohealthSlider()
    {
        GameManager.Instance.stormMode.cargoHealthSlider.fillAmount = GameManager.Instance.cargoController.Health / GameManager.Instance.cargoController.MaxHealth;
    }

    public void Reset(GameManager _gm)
    {
        _cargoTransform.position = new Vector3(0,0,0);
        _health = _maxHealth;
        _defense = 0;
        itemInventory.ClearInventory();
        _gm.hubManager.ResetStoreInventory();
        resourceInventory.ClearInventory();
        _speed = 1;
        _speedModifier = 1f;
        _heightTarget = 0;
        _targetPosition = new Vector3(0, 0, 0);
        _distanceTraveled = 0;
        if (GameManager.Instance.stormMode.cargoHealthSlider != null)
        {
            UpdateCargohealthSlider();
        }
    }

}