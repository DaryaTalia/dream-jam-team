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

    public SpawnManager spawnManager;
    public HubManager hubManager;
    public CargoController cargoController;

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
        _gameStatus = GameState.MainMenu;
        hubManager.menuState = HubManager.HubMenuState.GameMode;
        hubManager.LoadCustomDeliveryOptions();
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
        cargoController.Reset(this);
    }

    private void StormMode()
    {
        _gameStatus = GameState.StormMode;
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
        if (GameStatus == GameState.CalmMode)
        {
            StormMode();
        }
    }

}
