using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StormModeJourney : MonoBehaviour
{
    [SerializeField]
    bool journeyActive;

    public bool JourneyActive
    {
        get => journeyActive;
    }

    public List<float> Checkpoints;
    public float nextCheckpoint;
    public float journeyLength;

    public GameObject gameplayUI;
    public Image journeySlider;
    public Image cargoHealthSlider;

    public Vector3 checkpointStartingRange;
    public Vector3 checkpointEndingRange;

    public GameObject checkpointSprite;
    public List<GameObject> checkPointInstances;
    public GameObject resourcePanel;       // holds the instantiated resource graphics
    public GameObject itemPanel;       // holds the instantiated item graphics
    public GameObject resourcePrefab;
    public GameObject itemPrefab;
    public List<GameObject> items;
    public List<GameObject> resources;
    public List<ItemStack> resourceStacks;

    public void AddCheckpoints(List<float> _checkpoints)
    {
        Checkpoints = _checkpoints;
    }

    public void InitJourney()
    {
        foreach (GameObject go in resources)
        {
            Destroy(go);
        }
        resources.Clear();

        resourceStacks.Clear();

        foreach(GameObject go in checkPointInstances)
        {
            Destroy(go);
        }
        checkPointInstances.Clear();

        nextCheckpoint = Checkpoints[0];

        journeyLength = Checkpoints[Checkpoints.Count - 1];

        foreach (float point in Checkpoints)
        {
            float xPosition = journeySlider.transform.position.x + point;
            checkPointInstances.Add(Instantiate(checkpointSprite, gameplayUI.transform, false));
        }

        float sliderLength = journeySlider.gameObject.GetComponent<RectTransform>().rect.width - 10;

        int i = 0;
        // Update Checkpoint Instance Images as "Unreached" (Color.red)
        foreach (GameObject sprite in checkPointInstances)
        {
            sprite.GetComponent<Image>().color = Color.red;
            sprite.transform.position = new Vector3(((Checkpoints[i] * sliderLength) / journeyLength) + 60, journeySlider.transform.position.y + 35, 0);
            Debug.Log("Checkpoint " + i + " position: " + sprite.transform.position.x);
            ++i;
        }

        // Add Items to screen
        foreach (ItemStack item in GameManager.Instance.cargoController.GetItemInventory().itemInventory)
        {
            items.Add(Instantiate(item.baseItem.prefab, itemPanel.transform));
            items[items.Count - 1].GetComponent<Image>().sprite = item.baseItem.iconSprite;
        }


        // Add Resources to screen
        resourceStacks = GameManager.Instance.playerResources.itemInventory;

        i = 0;
        GameObject myResource;
        foreach(ItemStack _resource in resourceStacks)
        {
            myResource = Instantiate(resourcePrefab, resourcePanel.transform);
            resources.Add(myResource);

            myResource.gameObject.name = _resource.baseItem.itemName;

            myResource.GetComponent<Image>().sprite = _resource.baseItem.iconSprite;
            myResource.GetComponentInChildren<TextMeshProUGUI>().text = _resource.quantity.ToString();
            myResource.GetComponentInChildren<Button>().onClick.AddListener(() => UseResource(_resource.baseItem.itemName));
            ++i;
        }

        // initialize Cargo Health Meter
        Image[] cargoImages = GameManager.Instance.cargoController.CargoVehicle.GetComponentsInChildren<Image>();
        foreach (Image myHealth in cargoImages)
        {
            if (myHealth.gameObject.name == "HealthMask")
            {
                cargoHealthSlider = myHealth;
                cargoHealthSlider.fillAmount = GameManager.Instance.cargoController.Health / GameManager.Instance.cargoController.MaxHealth;
                break;
            }
        }

        journeyActive = true;
    }

    void Update()
    {
        if(journeyActive)
        {
            // Have we reached the next checkpoint?
            if (Checkpoints.Count > 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
            {
                // Faker
                //GameManager.Instance.cargoController.DamageCargo(23);
                //cargoHealthSlider.fillAmount = GameManager.Instance.cargoController.Health / GameManager.Instance.cargoController.MaxHealth;

                nextCheckpoint = Checkpoints[1];
                checkPointInstances[0].GetComponent<Image>().color = Color.green;
                checkPointInstances.RemoveAt(0);
                Checkpoints.RemoveAt(0);
                Destroy(items[0]);
                items.RemoveAt(0);

                CheckpointEvent();
            }
            // Have we reached the last checkpoint?
            else if (Checkpoints.Count == 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
            {
                nextCheckpoint = -1;
                checkPointInstances[0].GetComponent<Image>().color = Color.green;
                checkPointInstances.RemoveAt(0);
                Checkpoints.Clear();
                Destroy(items[0]);
                items.RemoveAt(0);

                CheckpointEvent();
            }

            if (nextCheckpoint != -1)
            {
                // If the journey is still ongoing, continue updating the progress slider
                journeySlider.fillAmount = GameManager.Instance.cargoController.DistanceTraveled / journeyLength;
            }

            if(GameManager.Instance.cargoController.Health <= 0)
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        // End the Journey/Delivery
        journeyActive = false;

        GameManager.Instance.StartCalmMode();
    }

    void CheckpointEvent()
    {
        GameManager.Instance.Gold += GameManager.Instance.cargoController.GetItemInventory().itemInventory[0].baseItem.deliveryReward;
        GameManager.Instance.cargoController.GetItemInventory().itemInventory.RemoveAt(0);

        if(Checkpoints.Count < 1)
        {
            GameManager.Instance.Gold += GameManager.Instance.hubManager.BaseGoldReward + (int)(GameManager.Instance.hubManager.SelectedDestination.Distance * GameManager.Instance.hubManager.DestinationGoldMultipler);

            // End the Journey/Delivery
            journeyActive = false;

            GameManager.Instance.StartCalmMode();
        }
    }

    public void UseResource(string _name)
    {
        foreach (ItemStack itemStack in resourceStacks)
        {
            if (itemStack.baseItem.itemName == _name && itemStack.quantity > 0)
            {
                GameObject myResource = resources.Find(r => r.gameObject.name == _name);        // Find the correct resource game object by text mesh name

                if (itemStack.baseItem.prefab != null)
                {
                    Instantiate(itemStack.baseItem.prefab);
                }
                else
                {
                    Debug.LogWarning("This Resource does not have a Prefab attached: " + _name);
                }

                if (myResource != null)
                {
                    GameManager.Instance.playerResources.RemoveItemFromInventory(itemStack.baseItem, 1);

                    if (itemStack.quantity < 1)
                    {
                        resources.Remove(myResource);
                        Destroy(myResource);
                    } else
                    {
                        myResource.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.quantity.ToString();                 // Update the text of the game object
                    }
                }
                else
                {
                    Debug.Log(myResource);
                }

                break;
            }
        }
    }
}
