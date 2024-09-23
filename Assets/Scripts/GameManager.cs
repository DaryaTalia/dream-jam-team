using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, NewGame, LoadGame, StoryMenu, SettingsMenu, PauseMenu, CalmMode, StormMode };
    [SerializeField]
    GameState _gameStatus;

    public GameState GameStatus
    {
        get => _gameStatus;
    }

    public enum CompletionState { New, Story1, Story2, Story3, Story4, Story5 };
    [SerializeField]
    CompletionState _completionStatus;
    bool storyInProgress;

    public bool StoryInProgress
    {
        get => storyInProgress;
        set
        {
            storyInProgress = value;
        }
    }

    public CompletionState CompletionStatus
    {
        get => _completionStatus;
    }

    public SpawnManager spawnManager;
    public HubManager hubManager;
    public CargoController cargoController;
    public StormModeJourney stormMode;
    public PlayerStats playerStats;
    public PlayerAttack playerAttack;
    public SpawnManager enemySpawn;
    public TerrainManager terrainManager;

    public GameObject HubUICanvas;
    public GameObject GameplayUICanvas;
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } 
        else {
            _instance = this;
        }
    }

    private void Start()
    {
        _gameStatus = GameState.NewGame;

        StartCalmMode();

        hubManager.LoadCustomDeliveryOptions();
        // hubManager.LoadRandomDeliveries();

        foreach(Sound sound in AudioManager.instance.sounds)
        {
            if(sound.mixerGroup.name == "SFX")
            {
                sound.volume = PlayerPrefsExtended.GetFloat("Sound Volume", .5f);
            }
            else
            if (sound.mixerGroup.name == "Music")
            {
                if(PlayerPrefsExtended.GetBool("Music toggle"))
                {
                    sound.volume = PlayerPrefsExtended.GetFloat("Sound Volume", .5f);

                } else
                {
                    sound.volume = PlayerPrefsExtended.GetFloat("Sound Volume", 0f);
                }
            }
        }
    }

    [Header("Resources")]
    public Inventory playerResources;

    [Header("Deliveries")]
    [SerializeField]
    Delivery selectedDelivery;

    public Delivery SelectedDelivery
    {
        get => selectedDelivery;
        set
        {
            selectedDelivery = value;
        }
    }

    [SerializeField]
    List<Delivery> storyDeliveries;
    [SerializeField]
    List<Delivery> randomDeliveries;
    [SerializeField]
    List<Destination> deliveryDestinations;

    public List<Delivery> StoryDeliveries
    {
        get => storyDeliveries;
        set
        {
            storyDeliveries = value;
        }
    }

    public Delivery GetStoryDelivery(string _name)
    {
        foreach(Delivery delivery in storyDeliveries)
        {
            if(delivery.Name == _name)
            {
                return delivery;
            }
        }

        return null;
    }

    public List<Delivery> RandomDeliveries
    {
        get => randomDeliveries;
        set
        {
            randomDeliveries = value;
        }
    }

    public Delivery GetRandomDelivery(string _name)
    {
        foreach (Delivery delivery in randomDeliveries)
        {
            if (delivery.Name == _name)
            {
                return delivery;
            }
        }

        return null;
    }

    public List<Destination> DeliveryDestinations
    {
        get => deliveryDestinations;
        set
        {
            deliveryDestinations = value;
        }
    }

    public Destination GetDeliveryDestination(string _name)
    {
        foreach (Destination destination in deliveryDestinations)
        {
            if (destination.Name == _name)
            {
                return destination;
            }
        }

        return null;
    }


    #endregion
    [Header("Currency")]
    [SerializeField]
    int _gold; // common
    int _silver; // uncommon
    int _platinum; // rare

    public int Gold
    {
        get => _gold;
        set
        {
            if(value < 1)
            {
                _gold = 0;
            } else
            {
                _gold = value;
            }
        }
    }

    private void CalmMode()
    {
        AudioManager.instance.Play("Calm");
        AudioManager.instance.Stop("Storm");
        AudioManager.instance.Stop("Ambient Storm");
        _gameStatus = GameState.CalmMode;
        hubManager.menuState = HubManager.HubMenuState.GameMode;

        if (storyInProgress && _completionStatus != CompletionState.Story4)
        {
            ++_completionStatus;
            storyInProgress = false;
        }

        foreach(GameObject go in stormMode.resources)
        {
            Destroy(go);
        }
        
        foreach(GameObject go in stormMode.checkPointInstances)
        {
            Destroy(go);
        }

        hubManager.LoadStoryDeliveries();
        hubManager.LoadRandomDeliveries();
        hubManager.ResetCustomItems();

        SelectedDelivery = null;
        hubManager.SelectedDestination = null;

        cargoController.Reset(this);

        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position = new Vector3(-5, -1, 0);

        selectedDelivery = new Delivery();
        selectedDelivery.Name = hubManager.deliveryUndecided;

        GameplayUICanvas.SetActive(false);
        cargoController.CargoVehicle.SetActive(false);
        playerStats.gameObject.SetActive(false);

        if (enemySpawn != null)
        {
            enemySpawn.Reset();
        }

        GameManager.Instance.hubManager.GetTextMeshProUGUI().text = GameManager.Instance.Gold.ToString();

        terrainManager.Reset();

        HubUICanvas.SetActive(true);
    }

    private void StormMode()
    {
        AudioManager.instance.Play("Ambient Storm");
        AudioManager.instance.Play("Storm");
        AudioManager.instance.Stop("Calm");
        _gameStatus = GameState.StormMode;

        stormMode.journeyLength = selectedDelivery.MyDestination.Distance;

        List<float> itemCheckpoints = new List<float>();

        float averageDistance = stormMode.journeyLength / cargoController.GetItemInventory().itemInventory.Count;
        for(int i = 1; i < cargoController.GetItemInventory().itemInventory.Count + 1; ++i)
        {
            itemCheckpoints.Add(averageDistance * i);
        }

        stormMode.AddCheckpoints(itemCheckpoints);

        GameplayUICanvas.SetActive(true);
        HubUICanvas.SetActive(false);

        stormMode.InitJourney();

        cargoController.CargoVehicle.SetActive(true);
        if(enemySpawn == null)
        {
            enemySpawn = GameObject.FindGameObjectWithTag("Spawner").GetComponent<SpawnManager>();
        }
        playerStats.gameObject.SetActive(true);
        // Keep the player in front of the cargo vehicle (closer to the camera)
        playerStats.gameObject.transform.position = new Vector3(
            gameObject.transform.position.x, 
            gameObject.transform.position.y, 
            GameManager.Instance.cargoController.CargoVehicle.transform.position.z - 2);

        playerStats.Init();
    }

    public void StartCalmMode()
    {
        if(GameStatus == GameState.StormMode || GameStatus == GameState.NewGame || GameStatus == GameState.LoadGame)
        {
            CalmMode();
        }
    }

    public void StartStormMode()
    {
        if (GameStatus == GameState.CalmMode && selectedDelivery.Name != hubManager.deliveryUndecided)
        {

            if(cargoController.GetItemInventoryCount() < 1)
            {
                Debug.Log("Not Enough Items Selected");
                return;
            }

            if(hubManager.SelectedDestination == null)
            {
                Debug.Log("Correct Destination Not Selected");
                return;
            }

            hubManager.CalculateCustomReward();
            StormMode();
        }
    }

}

