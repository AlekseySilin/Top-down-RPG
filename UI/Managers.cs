using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Ensure that various managers exists
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(MissionManager))]
[RequireComponent(typeof(DataManager))]


public class Managers : MonoBehaviour {
    //Other code can freely access inventory and player components
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static MissionManager Mission { get; private set; }
    public static DataManager Data { get; private set; }
    //The list of managers to loop through durng startup sequence
    private List<IGameManager> _startSequence;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Data = GetComponent<DataManager>();
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Mission = GetComponent<MissionManager>();
        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(Mission);
        //Because DataManager uses other managers (in order to update them),
       // you should make sure that the other managers appear earlier in the startup sequence.
        _startSequence.Add(Data);
       
        //Launch the startup sequence asynchronously
        StartCoroutine(StartupManagers());
    }
    private IEnumerator StartupManagers()
    {
        NetworkService network = new NetworkService();
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup(network);
        }
        yield return null;
        int numModules = _startSequence.Count;
        int numReady = 0;
        while (numReady<numModules)
        {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }
            if (numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
                Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }
            //pause one frame before checking again
            yield return null;
        }
        Debug.Log("All managers started up");
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }


	
}
