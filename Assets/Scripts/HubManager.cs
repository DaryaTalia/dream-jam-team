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

        deliveryStories = new List<GameObject>();
        randomDeliveries = new List<GameObject>();
    }

    private void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        resourceTooltip.Hide();

        foreach (RaycastResult result in raycastResultList)
        {
            if (result.gameObject.TryGetComponent(out ResourceButton btn))
            {
                resourceTooltip.SetText(btn.item.itemName, btn.item.deliveryReward.ToString());
                resourceTooltip.Show(result.screenPosition);
            }
        }
    }

    #region Cargo Resources
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

    public TextMeshProUGUI GetTextMeshProUGUI()
    {
        return goldText;
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

    #endregion

    #region UI

    #region Hub UI
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
                    break;
                }
            case "Story":
                {
                    menuState = HubMenuState.StoryDeliveryMode;
                    gameModeMenu.SetActive(false);
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
                    customDeliveryMenu.SetActive(true);
                    break;
                }
            default:
                {
                    Debug.Log("Invalid State");
                    menuState = HubMenuState.GameMode;
                    gameModeMenu.SetActive(true);
                    storyDeliveryMenu.SetActive(false);
                    randomDeliveryMenu.SetActive(false);
                    customDeliveryMenu.SetActive(false);
                    break;
                }
        }
    }

    #endregion

    #region Story Deliveries

    [Header("Story Delivery Properties")]

    [SerializeField]
    GameObject storyDeliveryContent;
    [SerializeField]
    GameObject storyDeliveryPrefab;
    [SerializeField]
    List<GameObject> deliveryStories;
    [SerializeField]
    GameObject storyItemPrefab;

    public void LoadStoryDeliveries()
    {
        foreach(GameObject go in deliveryStories)
        {
            Destroy(go);
        }
        deliveryStories.Clear();
        GameObject currentDelivery;

        //TODO: load each story based on enum completion status
        for(int story = 0; story <= ((int)GameManager.Instance.CompletionStatus); ++story)
        {
            currentDelivery = Instantiate(storyDeliveryPrefab, storyDeliveryContent.transform);
            deliveryStories.Add(currentDelivery);

            int goldAmount = 0;
            GameObject storyObject;

            foreach (BaseItem item in GameManager.Instance.StoryDeliveries[story].DeliverItems)
            {
                goldAmount += item.deliveryReward;

                storyObject = Instantiate(storyItemPrefab, currentDelivery.GetComponent<StoryDeliveryUI>().GetItemsPanel().transform);

                storyObject.GetComponentInChildren<Image>().sprite = item.iconSprite;
            }

            var storyDelivery = GameManager.Instance.StoryDeliveries[story];

            currentDelivery.GetComponent<StoryDeliveryUI>().SetStoryDeliveryUI(
                GameManager.Instance.StoryDeliveries[story].Name, 
                GameManager.Instance.StoryDeliveries[story].QuestDescription, 
                goldAmount.ToString(), 
                () => SelectStoryDelivery(storyDelivery));

            BaseGoldReward = goldAmount;
        }
    }

    public void SelectStoryDelivery(Delivery storyDelivery)
    {
        SelectedDestination = storyDelivery.MyDestination;
        foreach(BaseItem item in storyDelivery.DeliverItems)
        {
            Debug.Log(GameManager.Instance.cargoController.AddItemToInventory(item, 1));
        }

        if (SelectedDestination != null && GameManager.Instance.cargoController.GetItemInventoryCount() > 0)
        {
            GameManager.Instance.SelectedDelivery = new Delivery
            {
                Name = storyDelivery.Name,
                MyDestination = SelectedDestination
            };

            startDeliveryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Start Delivery";

            GameManager.Instance.StoryInProgress = true;
        }
    }

    #endregion

    #region Random Deliveries

    [Header("Random Delivery Properties")]
    [SerializeField]
    GameObject randomDeliveryPanel;
    [SerializeField]
    GameObject randomDeliveryPrefab;
    [SerializeField]
    List<GameObject> randomDeliveries;
    int maxRandomDeliveries = 4;
    public Delivery lastRandomDelivery;
    public static int lifetimeRandomDeliveries = 0;

    public void LoadRandomDeliveries()
    {
        GameObject randomDelivery;
        GameObject randomItem;
        Delivery newDelivery;
        int randomGold;
        int randomItemsCount;
        int thisRandomItem;
        
        if(lastRandomDelivery != null)
        {
            GameManager.Instance.RandomDeliveries.Remove(lastRandomDelivery);
            GameObject go = randomDeliveries.Find(x => x.name == lastRandomDelivery.Name);
            randomDeliveries.Remove(go);
            Destroy(go);
            lastRandomDelivery = null;
        }

        while (randomDeliveries != null && randomDeliveries.Count <= maxRandomDeliveries)
        {
            randomGold = 0;
            randomItemsCount = 0;
            thisRandomItem = 0;
            ++lifetimeRandomDeliveries;

            // Make a new Random Delivery
            randomDelivery = Instantiate(randomDeliveryPrefab, randomDeliveryPanel.transform);
            randomDeliveries.Add(randomDelivery);

            newDelivery = new Delivery();
            newDelivery.Name = "Random " + lifetimeRandomDeliveries;
            randomDelivery.name = newDelivery.Name;
            GameManager.Instance.RandomDeliveries.Add(lastRandomDelivery);
            newDelivery.DeliverItems = new List<BaseItem>();

            // Random Destination
            newDelivery.MyDestination = GameManager.Instance.DeliveryDestinations[Random.Range(0, GameManager.Instance.DeliveryDestinations.Count)];

            //Select a random number of items
            randomItemsCount = Random.Range(1, 6);

            for(int i = 0; i < randomItemsCount; ++i)
            {
                randomItem = Instantiate(storyItemPrefab, randomDelivery.GetComponent<StoryDeliveryUI>().GetItemsPanel().transform);

                thisRandomItem = Random.Range(0, deliveryItems.Count);
                randomItem.GetComponentInChildren<Image>().sprite = deliveryItems[thisRandomItem].iconSprite;

                randomGold += deliveryItems[thisRandomItem].deliveryReward;

                newDelivery.DeliverItems.Add(deliveryItems[thisRandomItem]);
            }

            StoryDeliveryUI randomDeliveryUI = randomDelivery.GetComponent<StoryDeliveryUI>();
            randomDeliveryUI.SetStoryDeliveryUI( newDelivery.Name, newDelivery.MyDestination.Name, randomGold.ToString(), () => SelectRandomDelivery(newDelivery));
        }
    }

    public void SelectRandomDelivery(Delivery delivery)
    {
        SelectedDestination = delivery.MyDestination;
        foreach (BaseItem item in delivery.DeliverItems)
        {
            Debug.Log(GameManager.Instance.cargoController.AddItemToInventory(item, 1));
        }

        if (SelectedDestination != null && GameManager.Instance.cargoController.GetItemInventoryCount() > 0)
        {
            GameManager.Instance.SelectedDelivery = new Delivery
            {
                Name = delivery.Name,
                MyDestination = SelectedDestination
            };

            lastRandomDelivery = delivery;

            startDeliveryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Start Delivery";

            GameManager.Instance.StoryInProgress = false;
        }
    }

    #endregion

    #region Custom Deliveries

    [Header("Custom Delivery Properties")]

    [SerializeField]
    GameObject destinationsPanel;
    [SerializeField]
    GameObject newDestinationPrefab; // includes children DestinationName and BaseDistance
    public Destination SelectedDestination;

    [SerializeField]
    GameObject deliveryItemsPanel;
    [SerializeField]
    GameObject deliveryItemChoicePrefab; // Includes Image for Icon and TextMeshProUGUI for "Size: x"

    [SerializeField]
    GameObject goldRewardGO; // enable or disable as needed
    [SerializeField]
    TextMeshProUGUI customGoldRewardText;
    public float DestinationGoldMultipler = .4f; // destination.distance * destinationGoldMultipler + baseGoldReward
    public int BaseGoldReward; // 20
    int customGoldReward;

    #region Additional Currency
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
    #endregion

    // When the Select button is clicked on for custom deliveries.
    public void SelectCustomDelivery()
    {
        if(SelectedDestination != null && GameManager.Instance.cargoController.GetItemInventoryCount() > 0)
        {
            GameManager.Instance.SelectedDelivery = new Delivery
            {
                Name = "Custom Delivery",
                MyDestination = SelectedDestination
            };
            // TODO: Update distance based on selected destination in CC
            startDeliveryBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Start Delivery";

            GameManager.Instance.StoryInProgress = false;
        }
    }

    List<GameObject> customDeliveryItems;

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

        customDeliveryItems = new List<GameObject>();

        GameObject tempItem;

        // Load Items
        foreach(BaseItem item in deliveryItems)
        {
            tempItem = Instantiate(deliveryItemChoicePrefab, deliveryItemsPanel.transform);
            tempItem.gameObject.name = item.itemName;
            customDeliveryItems.Add(tempItem);
            tempItem.GetComponent<Image>().sprite = item.iconSprite;
            tempItem.GetComponent<Image>().color = new Color(1, 1, 1, .4f);
            tempItem.GetComponentInChildren<TextMeshProUGUI>().text = "Size: " + item.size.ToString();

            tempItem.GetComponent<Button>().onClick.AddListener(() => ChooseDeliveryItem(item, item.itemName));
            tempItem.GetComponent<Button>().onClick.AddListener(CalculateCustomReward);
        }


    }

    public void ChooseCustomDelivery(string _name)
    {
        SelectedDestination = GameManager.Instance.DeliveryDestinations.Find(d => d.Name == _name);
    }

    public void ChooseDeliveryItem(BaseItem _item, string _name)
    {
        if(GameManager.Instance.cargoController.AddItemToInventory(_item, 1))
        {
            Debug.Log("New Item Added: " + _item.itemName);
            foreach(GameObject item in customDeliveryItems)
            {
                if(item.name == _name)
                {
                    item.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
                }
            }
        } else
        {
            Debug.Log("Item not Added: " + _item.itemName);
        }
    }

    public void ResetCustomItems()
    {
        if(customDeliveryItems != null && customDeliveryItems.Count > 0)
        {
            foreach (GameObject item in customDeliveryItems)
            {
                item.GetComponent<Image>().color = new Color(1, 1, 1, .4f);

            }
        }
    }

    public void CalculateCustomReward()
    {
        // Reset internal count
        customGoldReward = 0;

        // Check Items
        if (menuState == HubMenuState.CustomDeliveryMode && GameManager.Instance.GetComponent<CargoController>().GetItemInventoryCount() >= 1)
        {
            foreach (ItemStack item in GameManager.Instance.GetComponent<CargoController>().GetItemInventory().itemInventory)
            {
                customGoldReward += item.baseItem.deliveryReward;
            }

            goldRewardGO.SetActive(true);
        }

        // Check Destination
        if (menuState == HubMenuState.CustomDeliveryMode && SelectedDestination != null)
        {
            customGoldReward += BaseGoldReward + (int) (SelectedDestination.Distance * DestinationGoldMultipler);

            goldRewardGO.SetActive(true);
        }

        customGoldRewardText.text = customGoldReward.ToString();
    }

    #endregion

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

