using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HubManager : MonoBehaviour
{

    private void Start()
    {
        menuState = HubMenuState.GameMode;
        gameModeMenu.SetActive(true);
        storyDeliveryMenu.SetActive(false);
        randomDeliveryMenu.SetActive(false);
        customDeliveryMenu.SetActive(false);

        resourceTooltipGO.SetActive(false);

        LoadResources();
    }

    private void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        if (raycastResultList.Count == 0)
        {
            resourceTooltipGO.SetActive(false);
        }

        foreach (RaycastResult result in raycastResultList)
        {
            // Check for resource gameobject
            if(result.gameObject.GetComponent<MyResource>())
            {
                TextMeshProUGUI[] _text = resourceTooltipGO.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (TextMeshProUGUI text in _text)
                {
                    if (text.gameObject.name == "ResourceName")
                    {
                        text.text = result.gameObject.GetComponent<MyResource>().MyItem.baseItem.itemName;
                    }
                    else if (text.gameObject.name == "ResourceDescription")
                    {
                        text.text = "Delivery Reward: " + result.gameObject.GetComponent<MyResource>().MyItem.baseItem.deliveryReward.ToString();
                    }
                }
                resourceTooltipGO.SetActive(true);
            }
        }
    }


    [Header("Cargo Resources")]
    [SerializeField] private List<BaseItem> possibleResources;
    [SerializeField] private Inventory storeInventory;

    [SerializeField]
    GameObject resourceTooltipGO;

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

    [Header("Delivery Items")]
    [SerializeField]
    List<BaseItem> deliveryItems;



    #region UI

    [Header("Hub UI")]

    [SerializeField]
    GameObject resourceListContainer;
    [SerializeField]
    GameObject equipmentListContainer;

    public enum HubMenuState { GameMode, StoryDeliveryMode, RandomDeliveryMode, CustomDeliveryMode };
    [SerializeField]
    public HubMenuState menuState;
    HubMenuState selectedMenuState;
    [SerializeField]
    GameObject gameModeMenu;
    [SerializeField]
    GameObject storyDeliveryMenu;
    [SerializeField]
    GameObject randomDeliveryMenu;
    [SerializeField]
    GameObject customDeliveryMenu;

    [SerializeField]
    Button backToGameModeMenuBtn;
    [SerializeField]
    Button startDeliveryBtn;

    [Header("Resource and Equipment List Properties")]
    [SerializeField]
    GameObject resourceItemPrefab;
    [SerializeField]
    GameObject equipmentItemPrefab;

    public void LoadResources()
    {
        GameObject resourceContent;
        GameObject equipmentResource;
        foreach(ItemStack resource in GameManager.Instance.Resources)
        {
            resourceContent = Instantiate(resourceItemPrefab, resourceListContainer.transform);
            resourceContent.GetComponentInChildren<Image>().sprite = resource.baseItem.iconSprite;

            TextMeshProUGUI[] texts = resourceContent.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(TextMeshProUGUI text in texts)
            {
                if(text.gameObject.name == "ResourceNameText")
                {
                    text.gameObject.name = resource.baseItem.itemName;
                    text.text = resource.baseItem.itemName;
                }
                else if (text.gameObject.name == "ResourceCostText")
                {
                    text.text = resource.baseItem.cost.ToString() + " " + resource.baseItem.currency.ToString();
                }
                else if (text.gameObject.name == "ResourceQuantityText")
                {
                    text.text = resource.quantity.ToString();
                }
            }

            resourceContent.GetComponent<MyResource>().MyItem = resource;

            resourceContent.GetComponent<Button>().onClick.AddListener(() => BuyResource(resource.baseItem));

            // Load Equipment List
            equipmentResource = Instantiate(equipmentItemPrefab, equipmentListContainer.transform);
            Image[] images = equipmentResource.GetComponentsInChildren<Image>();
            foreach(Image img in images)
            {
                if (img.gameObject.name == "EquipmentIcon")
                {
                    img.sprite = resource.baseItem.iconSprite;
                    break;
                }
            }

            equipmentResource.GetComponentInChildren<TextMeshProUGUI>().text = resource.quantity.ToString();
        }
    }


    [Header("Custom Delivery Properties")]

    // Custom Delivery Properties
    [SerializeField]
    List<Destination> destinations;
    [SerializeField]
    GameObject destinationsPanel;
    [SerializeField]
    GameObject newDestinationPrefab; // includes children DestinationName and BaseDistance
    Destination selectedDestination;

    [SerializeField]
    GameObject deliveryItemsPanel;
    [SerializeField]
    GameObject deliveryItemChoicePrefab; // Includes Image for Icon and TextMeshProUGUI for "Size: x"

    [SerializeField]
    GameObject goldRewardGO; // enable or disable as needed
    [SerializeField]
    TextMeshProUGUI goldRewardText;
    float destinationGoldMultipler = .4f; // destination.distance * destinationGoldMultipler + baseGoldReward
    int baseGoldReward; // 20
    int customGoldReward;

    // GameObject silverRewardGO;
    // TextMeshProUGUI silverRewardText;
    // loat destinationSilverMultipler; // destination.distance * destinationSilverMultipler + baseSilverReward
    // int baseSilverReward; // 20
    // int customSilverReward;

    // GameObject platinumRewardGO;
    // TextMeshProUGUI platinumRewardText;
    // loat destinationPlatinumMultipler; // destination.distance * destinationPlatinumMultipler + basePlatinumReward
    // int basePlatinumReward; // 20
    // int customPlatinumReward;

    public void LoadCustomDeliveryOptions()
    {
        GameObject tempDeliveryOption;
        TextMeshProUGUI[] tempDeliveryText;

        foreach (Destination dest in destinations)
        {
            // Instantiate and edit visbile UI properties
            tempDeliveryOption = Instantiate(newDestinationPrefab, destinationsPanel.transform);
            tempDeliveryText = tempDeliveryOption.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(TextMeshProUGUI text in tempDeliveryText)
            {
                if(text.gameObject.name == "DestinationName")
                {
                    text.text = dest.Name;
                } 
                else if (text.gameObject.name == "BaseDistance")
                {
                    text.text = dest.Distance.ToString() + "w";
                }
            }

            // Access Button's onClick event
            tempDeliveryOption.GetComponent<Button>().onClick.AddListener(() => ChooseCustomDelivery(dest.Name));
            tempDeliveryOption.GetComponent<Button>().onClick.AddListener(CalculateCustomReward);
        }
    }

    public void ChooseCustomDelivery(string _name)
    {
        selectedDestination = destinations.Find(d => d.Name == _name);
    }

    public void LoadDeliveryItems()
    {
        GameObject tempDeliveryItem;

        foreach(BaseItem item in deliveryItems)
        {
            // Instantiate and edit visbile UI properties
            tempDeliveryItem = Instantiate(deliveryItemChoicePrefab, deliveryItemsPanel.transform);
            tempDeliveryItem.GetComponentInChildren<TextMeshProUGUI>().text = "Size: " + item.itemName;
            tempDeliveryItem.GetComponent<Image>().sprite = item.iconSprite;

            // Access Button's onClick event
            tempDeliveryItem.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.cargoController.AddItemToInventory(item, 1));
            tempDeliveryItem.GetComponent<Button>().onClick.AddListener(CalculateCustomReward);
        }

    }

    void CalculateCustomReward()
    {
        if(selectedMenuState == HubMenuState.CustomDeliveryMode && selectedDestination != null && GameManager.Instance.GetComponent<CargoController>().GetItemInventoryCount() > 1)
        {
            customGoldReward = baseGoldReward + (int) (selectedDestination.Distance * destinationGoldMultipler);

            foreach(ItemStack item in GameManager.Instance.GetComponent<CargoController>().GetItemInventory().itemInventory)
            {
                customGoldReward += item.baseItem.deliveryReward;
            }

            goldRewardText.text = customGoldReward.ToString();
            goldRewardGO.SetActive(true);
        }
    }

    public void ChangeMenus(string _state)
    {
        switch (_state)
        {
            case "Hub":
                {
                    menuState = HubMenuState.GameMode;
                    gameModeMenu.SetActive(true);
                    storyDeliveryMenu.SetActive(false);
                    randomDeliveryMenu.SetActive(false);
                    customDeliveryMenu.SetActive(false);
                    storyDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    break;
                }
            case "Story":
                {
                    menuState = HubMenuState.StoryDeliveryMode;
                    gameModeMenu.SetActive(false);
                    storyDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    storyDeliveryMenu.SetActive(true);
                    randomDeliveryMenu.SetActive(false);
                    customDeliveryMenu.SetActive(false);
                    break;
                }
            case "Random":
                {
                    menuState = HubMenuState.RandomDeliveryMode;
                    gameModeMenu.SetActive(false);
                    storyDeliveryMenu.SetActive(false);
                    storyDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.SetActive(true);
                    customDeliveryMenu.SetActive(false);
                    break;
                }
            case "Custom":
                {
                    menuState = HubMenuState.CustomDeliveryMode;
                    gameModeMenu.SetActive(false);
                    storyDeliveryMenu.SetActive(false);
                    randomDeliveryMenu.SetActive(false);
                    storyDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    customDeliveryMenu.SetActive(true);
                    break;
                }
            default:
                {
                    Debug.Log("Invalid State");
                    menuState = HubMenuState.GameMode;
                    storyDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    randomDeliveryMenu.GetComponentInChildren<Scrollbar>().value = 0;
                    gameModeMenu.SetActive(true);
                    storyDeliveryMenu.SetActive(false);
                    randomDeliveryMenu.SetActive(false);
                    customDeliveryMenu.SetActive(false);
                    break;
                }
        }
    }

    public void SelectState(string _state)
    {
        switch (_state)
        {
            case "Hub":
                {
                    selectedMenuState = HubMenuState.GameMode;
                    break;
                }
            case "Story":
                {
                    selectedMenuState = HubMenuState.StoryDeliveryMode;
                    break;
                }
            case "Random":
                {
                    selectedMenuState = HubMenuState.RandomDeliveryMode;
                    break;
                }
            case "Custom":
                {
                    selectedMenuState = HubMenuState.CustomDeliveryMode;
                    CalculateCustomReward();
                    break;
                }
            default:
                {
                    Debug.Log("Invalid State");
                    selectedMenuState = HubMenuState.GameMode;
                    break;
                }
        }
    }


    #endregion


}

[System.Serializable]
public class Destination
{
    [SerializeField]
    string name;
    [SerializeField]
    float distance;

    public Destination(string _name, float _distance) {
        name = _name;
        distance = _distance;
    }

    public string Name
    {
        get => name;
    }

    public float Distance
    {
        get => distance;
    }

}

