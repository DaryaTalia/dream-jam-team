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

    public enum CompletionState { New, Story1, Story2, Story3, Story4 };
    [SerializeField]
    CompletionState _completionStatus;

    public CompletionState CompletionStatus
    {
        get => _completionStatus;
    }

    public SpawnManager spawnManager;
    public HubManager hubManager;
    public CargoController cargoController;
    public StormModeJourney stormMode;

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
        _gameStatus = GameState.CalmMode;
        hubManager.menuState = HubManager.HubMenuState.GameMode;

        cargoController.Reset(this);
        selectedDelivery = new Delivery();
        selectedDelivery.Name = hubManager.deliveryUndecided;

        GameplayUICanvas.SetActive(false);
        cargoController.CargoVehicle.SetActive(false);
        HubUICanvas.SetActive(true);
    }

    private void StormMode()
    {
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
            StormMode();
        }
    }

}

