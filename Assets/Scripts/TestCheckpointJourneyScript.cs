using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestCheckpointJourneyScript : MonoBehaviour
{
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

    public List<GameObject> items;
    public GameObject _resources;       // holds the instantiated resource graphics
    public List<GameObject> resources;
    public List<ItemStack> resourceStacks;

    // Start is called before the first frame update
    void Start()
    {
        Checkpoints.Add(30f);
        Checkpoints.Add(60f);
        Checkpoints.Add(90f);

        nextCheckpoint = Checkpoints[0];

        journeyLength = Checkpoints[Checkpoints.Count - 1];

        foreach(float point in Checkpoints)
        {
            float xPosition = journeySlider.transform.position.x + point;
            checkPointInstances.Add(Instantiate(checkpointSprite, gameplayUI.transform, false));
        }

        float sliderLength = journeySlider.gameObject.GetComponent<RectTransform>().rect.width - 10;

        int i = 0;
        foreach(GameObject sprite in checkPointInstances)
        {
            sprite.GetComponent<Image>().color = Color.red;
            sprite.transform.position = new Vector3(((Checkpoints[i] * sliderLength) / journeyLength) + 60, journeySlider.transform.position.y + 35, 0);
            Debug.Log("Checkpoint " + i + " position: " + sprite.transform.position.x);
            ++i;
        }

        foreach (ItemStack itemStack in resourceStacks)
        {
            resources.Add(Instantiate(itemStack.baseItem.prefab, _resources.transform));
        }

        i = 0;
        foreach (GameObject resource in resources)
        {
            int index = i;
            resource.gameObject.name = resourceStacks[index].baseItem.itemName;
            resource.GetComponentInChildren<TextMeshProUGUI>().text = resourceStacks[index].quantity.ToString();
            resource.GetComponentInChildren<Button>().onClick.AddListener(() => UseResource(resourceStacks[index].baseItem.itemName));
            ++i;
        }

        Image[] cargoImages = GameManager.Instance.cargoController.CargoVehicle.GetComponentsInChildren<Image>();
        foreach(Image myHealth in cargoImages)
        {
            if(myHealth.gameObject.name == "HealthMask")
            {
                cargoHealthSlider = myHealth;
                cargoHealthSlider.fillAmount = GameManager.Instance.cargoController.Health / GameManager.Instance.cargoController.MaxHealth;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Checkpoints.Count > 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
        {
            //Testing
            GameManager.Instance.cargoController.Health -= 20;
            cargoHealthSlider.fillAmount = (GameManager.Instance.cargoController.Health) / GameManager.Instance.cargoController.MaxHealth;

            nextCheckpoint = Checkpoints[1];
            checkPointInstances[0].GetComponent<Image>().color = Color.green;
            checkPointInstances.RemoveAt(0);
            Checkpoints.RemoveAt(0);
            Destroy(items[0]);
            items.RemoveAt(0);
        } else if (Checkpoints.Count == 1 && GameManager.Instance.cargoController.DistanceTraveled >= nextCheckpoint)
        {
            //Testing
            GameManager.Instance.cargoController.Health -= 20;
            cargoHealthSlider.fillAmount = (GameManager.Instance.cargoController.Health) / GameManager.Instance.cargoController.MaxHealth;

            nextCheckpoint = -1;
            checkPointInstances[0].GetComponent<Image>().color = Color.green;
            checkPointInstances.RemoveAt(0);
            Checkpoints.Clear();
            Destroy(items[0]);
            items.RemoveAt(0);
        }

        if(nextCheckpoint != -1)
        {
            journeySlider.fillAmount = GameManager.Instance.cargoController.DistanceTraveled / journeyLength;
        }
    }


    public void UseResource(string _name)
    {
        foreach(ItemStack itemStack in resourceStacks)
        {
            if(itemStack.baseItem.itemName == _name && itemStack.quantity > 0)
            {
                --itemStack.quantity;
                GameObject myResource = resources.Find(r => r.gameObject.name == _name);        // Find the correct resource game object by text mesh name

                if (myResource != null)
                {
                    myResource.GetComponentInChildren<TextMeshProUGUI>().text = itemStack.quantity.ToString();                 // Update the text of the game object
                } else
                {
                    Debug.Log(myResource);
                }     
                    
                break;
            }
        }
    }
}
