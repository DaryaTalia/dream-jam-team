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

        resourceTooltip.Hide();

        LoadResources();

        goldText.text = GameManager.Instance.Gold.ToString();
        startDeliveryBtn.GetComponentInChildren<TextMeshProUGUI>().text = deliveryUndecided;
    }

    private void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        if (raycastResultList.Count == 0)
        {
            resourceTooltip.Hide();
        }

        foreach (RaycastResult result in raycastResultList)
        {
            if (result.gameObject.TryGetComponent(out ResourceButton btn))
            {
                resourceTooltip.SetText(btn.item.itemName, btn.item.deliveryReward.ToString());
                resourceTooltip.Show(result.screenPosition);
            }
        }
    }


    [Header("Cargo Resources")]
    [SerializeField] private List<BaseItem> possibleResources;
    [SerializeField] private Inventory storeInventory;

    [SerializeField] ResourceTooltip resourceTooltip;

    public bool BuyResource(BaseItem item)
    {
        if (GameManager.Instance.Gold < item.cost)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        GameManager.Instance.Gold -= item.cost;
        GameManager.Instance.playerResources.AddItemToInventory(item, 1);
        UpdateEquipment();
        //storeInventory.RemoveItemFromInventory(item, 1);

        goldText.text = GameManager.Instance.Gold.ToString();
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

    public string deliveryUndecided = "Delivery Undecided";

    [SerializeField]
    TextMeshProUGUI goldText;

    [SerializeField]
    GameObject resourceListContainer;
    [SerializeField]
    GameObject equipmentListContainer;
    private List<GameObject> equipmentSlots = new List<GameObject>();

    public enum HubMenuState { GameMode, StoryDeliveryMode, RandomDeliveryMode, CustomDeliveryMode };
    [SerializeField]
    public HubMenuState menuState;
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
        foreach(BaseItem resource in possibleResources)
        {
            resourceContent = Instantiate(resourceItemPrefab, resourceListContainer.transform);
            ResourceButton resourceButton = resourceContent.GetComponent<ResourceButton>();
            resourceButton.SetItem(resource);
        }

        UpdateEquipment();
    }

    private void UpdateEquipment()
    {
        foreach (GameObject slot in equipmentSlots)
        {
            Destroy(slot);
        }
        foreach (ItemStack stack in GameManager.Instance.playerResources.itemInventory)
        {
            // Load Equipment List
            GameObject equipmentResource = Instantiate(equipmentItemPrefab, equipmentListContainer.transform);
            equipmentSlots.Add(equipmentResource);
            var slot = equipmentResource.GetComponent<EquipmentSlot>();
            slot.SetEquipment(stack);
        }
    }


    [Header("Custom Delivery Properties")]

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
    TextMeshProUGUI customGoldRewardText;
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

    // When the Select button is clicked on for custom deliveries.
    public void SelectCustomDelivery()
    {
        if(selectedDestination != null && GameManager.Instance.cargoController.GetItemInventoryCount() > 0)
        {
            GameManager.Instance.SelectedDelivery = new Delivery
            {
                Name = "Custom Delivery",
                MyDestination = selectedDestination
            };
            // TODO: Update distance based on selected destination in CC
            startDeliveryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Start Delivery";
        }
    }

    public void LoadCustomDeliveryOptions()
    {
        GameObject tempDeliveryOption;
        TextMeshProUGUI tempDeliveryText;

        // Load Delivery Destinations
        foreach (Destination dest in GameManager.Instance.DeliveryDestinations)
        {
            // Instantiate and edit visbile UI properties
            tempDeliveryOption = Instantiate(newDestinationPrefab, destinationsPanel.transform);
            tempDeliveryText = tempDeliveryOption.GetComponentInChildren<TextMeshProUGUI>();
            tempDeliveryText.text = dest.Name + "\t(" + dest.Distance + ")";

            // Access Button's onClick event
            tempDeliveryOption.GetComponent<Button>().onClick.AddListener(() => ChooseCustomDelivery(dest.Name));
            tempDeliveryOption.GetComponent<Button>().onClick.AddListener(CalculateCustomReward);
        }


        GameObject tempItem;

        // Load Items
        foreach(BaseItem item in deliveryItems)
        {
            tempItem = Instantiate(deliveryItemChoicePrefab, deliveryItemsPanel.transform);
            tempItem.GetComponent<Image>().sprite = item.iconSprite;
            tempItem.GetComponentInChildren<TextMeshProUGUI>().text = "Size: " + item.size.ToString();

            tempItem.GetComponent<Button>().onClick.AddListener(() => ChooseDeliveryItem(item));
            tempItem.GetComponent<Button>().onClick.AddListener(CalculateCustomReward);
        }


    }

    public void ChooseCustomDelivery(string _name)
    {
        selectedDestination = GameManager.Instance.DeliveryDestinations.Find(d => d.Name == _name);
    }

    public void ChooseDeliveryItem(BaseItem _item)
    {
        if(GameManager.Instance.cargoController.AddItemToInventory(_item, 1))
        {
            Debug.Log("New Item Added: " + _item.itemName);
        } else
        {
            Debug.Log("Item not Added: " + _item.itemName);
        }
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

        goldRewardGO.SetActive(false);

    }

    void CalculateCustomReward()
    {
        if(menuState == HubMenuState.CustomDeliveryMode && selectedDestination != null && GameManager.Instance.GetComponent<CargoController>().GetItemInventoryCount() >= 1)
        {
            customGoldReward = baseGoldReward + (int) (selectedDestination.Distance * destinationGoldMultipler);

            foreach(ItemStack item in GameManager.Instance.GetComponent<CargoController>().GetItemInventory().itemInventory)
            {
                customGoldReward += item.baseItem.deliveryReward;
            }

            customGoldRewardText.text = customGoldReward.ToString();
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

