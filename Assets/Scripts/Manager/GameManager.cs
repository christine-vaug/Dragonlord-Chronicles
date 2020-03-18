using System.Collections;
using System.Collections.Generic;
using StateManagement;
using UnityEngine;


/// <summary>
/// This class handles the various game states (overworld, main menu, battle, etc.)
/// There should only be one instance of GameManager per scene.
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager instance;

    //managers
    ResourcesManager resourceMgr;
    EntityManager entityMgr;
    QuestManager questMgr;

    //states
    Dictionary<GameStateType, IGameState> stateTypeMap;
    Stack<GameStateType> states;
    IGameState current;

	// Use this for initialization
	void Awake () {

        Screen.SetResolution(1280, 720, true);

        if (instance != null) {

            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        //GlobalFlags.LoadGlobalFlags();
        GlobalFlags.SetCurrentOverworldScene("Overworld");
        GlobalFlags.SetCurrentBattleScene("Battle");
        
        InitializeManagers();
        InitializeGameStates();

	}

    void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        current.OnStateUpdate(Time.deltaTime);
	}

    void InitializeManagers () {

        resourceMgr = new ResourcesManager();
        questMgr = new QuestManager();
        entityMgr = new EntityManager();

        resourceMgr.Awake();
        entityMgr.Awake();
        questMgr.Awake();

        resourceMgr.Start();
        entityMgr.Start();
        questMgr.Start();
    }

    void InitializeGameStates () {

        stateTypeMap = new Dictionary<GameStateType, IGameState>();
        states = new Stack<GameStateType>();

        stateTypeMap.Add(GameStateType.MainMenu, new MainMenuState());
        stateTypeMap.Add(GameStateType.Overworld, new OverworldState());
        stateTypeMap.Add(GameStateType.Battle, new BattleState());
        stateTypeMap.Add(GameStateType.Inventory, new InventoryState());
        stateTypeMap.Add(GameStateType.SceneLoad, new SceneLoadState());
        stateTypeMap.Add(GameStateType.Credits, new CreditsState());

        states.Push(GameStateType.MainMenu);
        current = stateTypeMap[ states.Peek() ];
        current.OnStateEnter();
    }

    public void PushState (GameStateType next, params object[] parameters) {
        current.OnStatePause();
        states.Push(next);
        current = stateTypeMap[states.Peek()];
        current.OnStateEnter(parameters);
    }

    public void PopState () {

        current.OnStateExit();
        states.Pop();

        current = stateTypeMap[states.Peek()];
        current.OnStateResume();
    }
    
}
