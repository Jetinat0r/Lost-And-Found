using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TimeBlock curTime;
    public List<GamePeriod> timeSlots;

    public GameObject playerPrefab;
    private GameObject spawnedPlayer;

    [HideInInspector]
    public List<QuestInfo> curQuestInfos = new List<QuestInfo>();
    [HideInInspector]
    public List<FillerNpcInfo> curFillerNpcInfos = new List<FillerNpcInfo>();
    [HideInInspector]
    public List<PhysicalQuestItemInfo> curPhysicalQuestItemInfos = new List<PhysicalQuestItemInfo>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //TODO: Move to GameManager or something
    //Why did i leave this comment. This IS GameManager
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Test(EventFunctionParams functionParams)
    {
        Debug.Log("Test: " + functionParams.name);
    }

    // funcParams is ALWAYS string[], int[], float[], bool[]
    //public void Test(string[] strings, int[] ints, float[] floats, bool[] bools)
    //{
    //    Debug.Log("Test Succ!" + ints[0]);
    //}
    
    public GamePeriod GetCurrentPeriod()
    {
        foreach (GamePeriod period in timeSlots)
        {
            if (period.timeBlock.IsEqual(curTime))
            {
                return period;
            }
        }

        Debug.LogWarning("Time Period not found! Returning first period...");
        return timeSlots[0];
    }

    //Loads the next available period, starting with [0]
    public void ChangeGamePeriod(bool _isStealthy = false)
    {
        GamePeriod periodToLoad = null;

        //Looks for the Game Period corresponding to the current Time Block,
        //and then gets the next one from the list.
        //If the time block is the last one, we should be using a different behavior
        for (int i = 0; i < timeSlots.Count - 1; i++)
        {
            if (curTime.Equals(timeSlots[i].timeBlock))
            {
                periodToLoad = timeSlots[i + 1];
                break;
            }
        }

        if (periodToLoad == null)
        {
            Debug.Log("No Game Periods found with curTime, or on the last Game Period. Loading first period...");
            periodToLoad = timeSlots[0];
        }

        LoadPeriod(periodToLoad, _isStealthy);
    }

    //Loads the period with the given TimeBlock
    //If the time block is not found, loads the first period
    public void ChangeGamePeriod(TimeBlock timeBlock, bool _isStealthy = false)
    {
        GamePeriod periodToLoad = null;

        for(int i = 0; i < timeSlots.Count; i++)
        {
            if (timeBlock.Equals(timeSlots[i].timeBlock))
            {
                periodToLoad = timeSlots[i];
                break;
            }
        }

        if(periodToLoad == null)
        {
            periodToLoad = timeSlots[0];
            Debug.LogWarning("No Game Period found with time: (Day: " + timeBlock.day + "; Time: " + timeBlock.time + ")! Loading first Game Period...");
        }

        LoadPeriod(periodToLoad, _isStealthy);
    }

    private void LoadPeriod(GamePeriod periodToLoad, bool _isStealthy = false)
    {
        //TODO: Display Time Block on UI to indicate change
        if (!_isStealthy)
        {
            //Display UI, else don't (bc it's stealthy!)
        }
        curTime = periodToLoad.timeBlock;

        #region Load GamePeriod Infos into "cur's"
        foreach (QuestInfo info in periodToLoad.questInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            info.quest.InitializeQuestState();
            curQuestInfos.Add(info);
            QuestManager.instance.AddQuest(info.quest);
        }

        foreach (FillerNpcInfo info in periodToLoad.fillerNpcInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            curFillerNpcInfos.Add(info);
        }

        foreach(PhysicalQuestItemInfo info in periodToLoad.physicalQuestItemInfos)
        {
            //TODO: Add some checks to ensure the same info isn't added twice anywhere
            curPhysicalQuestItemInfos.Add(info);
        }
        #endregion

        if (!_isStealthy)
        {
            FadeTransitionManager.instance.SetTransitionText(periodToLoad.GetDisplayTime());
        }
        SceneController.instance.GotoNode(periodToLoad.startingNodeId);
    }

    public void SpawnPlayer()
    {
        if(spawnedPlayer != null)
        {
            //Debug.LogWarning("Player already exists!");
            return;
        }

        spawnedPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(spawnedPlayer);
    }

    public bool HasSpawnedPlayer()
    {
        return spawnedPlayer != null;
    }

    public void MovePlayer(Vector3 _pos)
    {
        if(spawnedPlayer != null)
        {
            spawnedPlayer.transform.position = _pos;
        }
        else
        {
            Debug.LogWarning("Player does not exist, and therefore cannot be moved!");
        }
    }

    public void DeletePlayer()
    {
        if(spawnedPlayer != null)
        {
            Destroy(spawnedPlayer);
            spawnedPlayer = null;
        }
        else
        {
            Debug.Log("DeletePlayer() called when no player exists!");
        }
    }

    public void DisablePlayerInput()
    {
        if(spawnedPlayer != null)
        {
            PlayerInteract _playerInteract = spawnedPlayer.GetComponent<PlayerInteract>();
            PlayerMovement _playerMovement = spawnedPlayer.GetComponent<PlayerMovement>();
            if (_playerMovement != null)
            {
                _playerMovement.DisableMovement();
                _playerInteract.DisableInteract();
            }
            else
            {
                Debug.LogWarning("No movement script attatched to player!");
            }
        }
        else
        {
            Debug.LogWarning("No player exists!");
        }
    }

    public void EnablePlayerInput()
    {
        if(spawnedPlayer != null)
        {
            PlayerMovement _playerMovement = spawnedPlayer.GetComponent<PlayerMovement>();
            PlayerInteract _playerInteract = spawnedPlayer.GetComponent<PlayerInteract>();
            if (_playerMovement != null)
            {
                _playerMovement.EnableMovement();
                _playerInteract.EnableInteract();
            }
            else
            {
                Debug.LogWarning("No movement script attatched to player!");
            }
        }
        else
        {
            Debug.LogWarning("No player exists!");
        }
    }
}
