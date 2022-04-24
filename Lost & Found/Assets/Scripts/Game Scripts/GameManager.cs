using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private readonly string SaveDataVersionNumber = "0.2.1";

    [Serializable]
    private class SaveData
    {
        private string saveDataVersion;
        private TimeBlock gamePeriodTimeBlock;
        private bool isLoadStealthy;

        private List<string> curQuestInfoIds = new List<string>();
        private List<QuestState> curQuestStates = new List<QuestState>();

        private List<string> curFillerNpcInfoIds = new List<string>();
        private List<string> curPhysicalQuestItemInfoIds = new List<string>();
        private List<string> curFloatingItemIds = new List<string>();

        private List<string> curHeldItemIds = new List<string>();

        //Constructor
        public SaveData(
            string _saveDataVersion,
            GamePeriod _gamePeriod,
            bool _isLoadStealthy,
            List<QuestInfo> _curQuestInfos,
            List<FillerNpcInfo> _curFillerNpcInfos,
            List<PhysicalQuestItemInfo> _curPhysicalQuestItemInfos,
            List<QuestItemScriptableObject> _curFloatingItems,
            List<QuestItemScriptableObject> _curHeldItems)
        {
            SetData(_saveDataVersion, _gamePeriod, _isLoadStealthy, _curQuestInfos, _curFillerNpcInfos, _curPhysicalQuestItemInfos, _curFloatingItems, _curHeldItems);
        }

        //Acts like the constructor
        public void SetData(
            string _saveDataVersion,
            GamePeriod _gamePeriod,
            bool _isLoadStealthy,
            List<QuestInfo> _curQuestInfos,
            List<FillerNpcInfo> _curFillerNpcInfos,
            List<PhysicalQuestItemInfo> _curPhysicalQuestItemInfos,
            List<QuestItemScriptableObject> _curFloatingItems,
            List<QuestItemScriptableObject> _curHeldItems)
        {
            saveDataVersion = _saveDataVersion;

            gamePeriodTimeBlock = _gamePeriod.timeBlock;
            isLoadStealthy = _isLoadStealthy;

            foreach (QuestInfo _questInfo in _curQuestInfos)
            {
                curQuestInfoIds.Add(_questInfo.id);
                curQuestStates.Add(_questInfo.quest.curQuestState);
            }

            foreach (FillerNpcInfo _fillerNpcInfo in _curFillerNpcInfos)
            {
                curFillerNpcInfoIds.Add(_fillerNpcInfo.id);
            }

            foreach (PhysicalQuestItemInfo _physicalQuestItemInfo in _curPhysicalQuestItemInfos)
            {
                curPhysicalQuestItemInfoIds.Add(_physicalQuestItemInfo.id);
            }

            foreach (QuestItemScriptableObject _curFloatingItem in _curFloatingItems)
            {
                curFloatingItemIds.Add(_curFloatingItem.idItemName);
            }

            if (_curHeldItems != null)
            {
                foreach (QuestItemScriptableObject _curHeldItem in _curHeldItems)
                {
                    curHeldItemIds.Add(_curHeldItem.idItemName);
                }
            }
        }

        //Made to retrieve data by reference
        public void RetrieveData(
            out string _saveDataVersion,
            out TimeBlock _gamePeriodTimeBlock,
            out bool _isLoadStealthy,
            out List<string> _curQuestInfoIds,
            out List<QuestState> _curQuestStates,
            out List<string> _curFillerNpcInfoIds,
            out List<string> _curPhysicalQuestItemInfoIds,
            out List<string> _curFloatingItemids,
            out List<string> _curHeldItems)
        {
            _saveDataVersion = saveDataVersion;

            _gamePeriodTimeBlock = new TimeBlock
            {
                day = gamePeriodTimeBlock.day,
                time = gamePeriodTimeBlock.time
            };
            _isLoadStealthy = isLoadStealthy;

            _curQuestInfoIds = new List<string>(curQuestInfoIds);
            _curQuestStates = new List<QuestState>(curQuestStates);

            _curFillerNpcInfoIds = new List<string>(curFillerNpcInfoIds);
            _curPhysicalQuestItemInfoIds = new List<string>(curPhysicalQuestItemInfoIds);
            _curFloatingItemids = new List<string>(curFloatingItemIds);

            _curHeldItems = new List<string>(curHeldItemIds);
        }
    }
    
    private class LoadedPeriod
    {
        public GamePeriod gamePeriod;
        public bool isStealthy;

        public LoadedPeriod(GamePeriod _gamePeriod, bool _isStealthy)
        {
            gamePeriod = _gamePeriod;
            isStealthy = _isStealthy;
        }
    }

    public static GameManager instance;

    public TimeBlock curTime;
    public List<GamePeriod> timeSlots;

    public GameObject playerPrefab;
    private GameObject spawnedPlayer;
    private bool playerHasControl = true;

    [HideInInspector]
    public List<QuestInfo> curQuestInfos = new List<QuestInfo>();
    [HideInInspector]
    public List<FillerNpcInfo> curFillerNpcInfos = new List<FillerNpcInfo>();
    [HideInInspector]
    public List<PhysicalQuestItemInfo> curPhysicalQuestItemInfos = new List<PhysicalQuestItemInfo>();
    [HideInInspector]
    public List<QuestItemScriptableObject> curFloatingItems = new List<QuestItemScriptableObject>();

    [SerializeField]
    private Journal journal;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJournal();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleJournal()
    {
        //Don't open the journal if there is no player
        //TODO: Make it so that when the player doesn't have control they can't open the journal (maybe move call to player?)
        if(spawnedPlayer == null)
        {
            return;
        }

        if (!journal.isOpen && playerHasControl)
        {
            DisablePlayerInput();
            journal.OpenJournal();
        }
        else
        {
            EnablePlayerInput();
            journal.CloseJournal();
        }
    }

    //public void Test(EventFunctionParams functionParams)
    //{
    //    Debug.Log("Test: " + functionParams.name);
    //}

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

    //EventFinder Overload
    public void ChangeGamePeriod(EventFunctionParams functionParams)
    {
        ChangeGamePeriod(functionParams.boolParams[0]);
    }

    private void LoadPeriod(GamePeriod periodToLoad, bool _isStealthy = false, bool _saveGame = true)
    {
        if (_saveGame)
        {
            SaveGame(periodToLoad, _isStealthy);
        }

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

        foreach (QuestItemScriptableObject item in periodToLoad.floatingItems)
        {
            curFloatingItems.Add(item);
        }
        #endregion

        if (!_isStealthy)
        {
            FadeTransitionManager.instance.SetTransitionText(periodToLoad.GetDisplayTime());
        }
        SceneController.instance.GotoNode(periodToLoad.startingNodeId);
    }

    public void SaveGame(GamePeriod gamePeriod, bool _isLoadStealthy)
    {
        //Safely gets the player's current held items
        List<QuestItemScriptableObject> _curHeldItems = new List<QuestItemScriptableObject>();
        if (HasSpawnedPlayer())
        {
            _curHeldItems = PlayerInventory.instance.curHeldItems;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/LnF_SaveData.txt");
        SaveData data = new SaveData(SaveDataVersionNumber, gamePeriod, _isLoadStealthy, curQuestInfos, curFillerNpcInfos, curPhysicalQuestItemInfos, curFloatingItems, _curHeldItems);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    //Loads relevant Game Data into the necessary spots, and returns the next GamePeriod to load
    //If anything fails, returns null
    private LoadedPeriod LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/LnF_SaveData.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/LnF_SaveData.txt", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            string _saveDataVersion = "-1";
            TimeBlock _nextGamePeriodTimeBlock;
            bool _isLoadStealthy;
            List<string> _curQuestInfoIds, _curFillerNpcInfoIds, _curPhysicalQuestItemInfoIds, _curFloatingItemIds, _curHeldItemIds;
            List<QuestState> _curQuestStates;

            data.RetrieveData(out _saveDataVersion, out _nextGamePeriodTimeBlock, out _isLoadStealthy, out _curQuestInfoIds, out _curQuestStates, out _curFillerNpcInfoIds, out _curPhysicalQuestItemInfoIds, out _curFloatingItemIds, out _curHeldItemIds);

            //Check if the save data is up to date
            if(_saveDataVersion != SaveDataVersionNumber)
            {
                Debug.LogWarning("Outdated and incompatible save data! Current Version (" + SaveDataVersionNumber + "); Saved Version (" + _saveDataVersion + ")");
                return null;
            }

            //Debug.Log("Loaded Game Period with Time Block (Day: " + _nextGamePeriodTimeBlock.day + "; Time: " + _nextGamePeriodTimeBlock.time + ")");

            //Get the nextGamePeriod
            GamePeriod _nextGamePeriod = null;
            int _curTimeSlotIndex = -1;
            for(int i = 0; i < timeSlots.Count; i++)
            {
                if(timeSlots[i].timeBlock.IsEqual(_nextGamePeriodTimeBlock))
                {
                    _nextGamePeriod = timeSlots[i];
                    _curTimeSlotIndex = i;
                    break;
                }
            }

            //Check if the GamePeriod actually exists
            if(_nextGamePeriod == null)
            {
                Debug.LogWarning("No Game Period found for TimeBlock: " + _nextGamePeriodTimeBlock);
                return null;
            }

            //Loads data by GamePeriod
            for(int i = 0; i < _curTimeSlotIndex; i++)
            {
                //Get QuestInfos
                foreach(QuestInfo _questInfo in timeSlots[i].questInfos)
                {
                    if (_curQuestInfoIds.Contains(_questInfo.id))
                    {
                        curQuestInfos.Add(_questInfo);
                        //Get index of QuestInfo for QuestState
                        int index = -1;
                        for(int j = 0; j < _curQuestInfoIds.Count; j++)
                        {
                            if(_curQuestInfoIds[j] == _questInfo.id)
                            {
                                index = j;
                                break;
                            }
                        }

                        //Extra checking, should literally never happen
                        if(index == -1)
                        {
                            Debug.LogWarning("Critical error in loading questInfos!");
                        }

                        _questInfo.quest.curQuestState = _curQuestStates[index];


                        _curQuestInfoIds.Remove(_questInfo.id);
                        _curQuestStates.RemoveAt(index);
                    }
                }

                //Get FillerNpcInfos
                foreach(FillerNpcInfo _fillerNpcInfo in timeSlots[i].fillerNpcInfos)
                {
                    if (_curFillerNpcInfoIds.Contains(_fillerNpcInfo.id))
                    {
                        curFillerNpcInfos.Add(_fillerNpcInfo);
                        _curFillerNpcInfoIds.Remove(_fillerNpcInfo.id);
                    }
                }

                //Things below this point assume that a player has been successfully created

                //Get PhysicalQuestItemInfos and maybe some of CurHeldItems
                foreach(PhysicalQuestItemInfo _physicalQuestItemInfo in timeSlots[i].physicalQuestItemInfos)
                {
                    //PhysicalQuestItemInfos
                    if (_curPhysicalQuestItemInfoIds.Contains(_physicalQuestItemInfo.id))
                    {
                        curPhysicalQuestItemInfos.Add(_physicalQuestItemInfo);
                        _curPhysicalQuestItemInfoIds.Remove(_physicalQuestItemInfo.id);
                    }

                    //CurHeldItems
                    if (_curHeldItemIds.Contains(_physicalQuestItemInfo.itemPrefab.GetItem().idItemName))
                    {
                        PlayerInventory.instance.PickupItem(_physicalQuestItemInfo.itemPrefab.GetItem());
                        _curHeldItemIds.Remove(_physicalQuestItemInfo.itemPrefab.GetItem().idItemName);
                    }
                }

                //Get FloatingItems and hopefully the rest of CurHeldItems
                foreach(QuestItemScriptableObject _floatingItem in timeSlots[i].floatingItems)
                {
                    //FloatingItems
                    if (_curFloatingItemIds.Contains(_floatingItem.idItemName))
                    {
                        curFloatingItems.Add(_floatingItem);
                        _curFloatingItemIds.Remove(_floatingItem.idItemName);
                    }

                    //CurHeldItems
                    if (_curHeldItemIds.Contains(_floatingItem.idItemName))
                    {
                        PlayerInventory.instance.PickupItem(_floatingItem);
                        _curHeldItemIds.Remove(_floatingItem.idItemName);
                    }
                }
            }


            Debug.Log("Game data loaded!");
            return new LoadedPeriod(_nextGamePeriod, _isLoadStealthy);
        }
        else
        {
            Debug.LogWarning("There is no save data!");
            return null;
        }
    }

    //Starts a new game
    public void StartGame()
    {
        SpawnPlayer();
        ChangeGamePeriod();
        ClearInventory();
    }

    //Loads game from given file, if that file does not exist or something goes wrong while loading, starts a new game
    public void LoadGame()
    {
        SpawnPlayer();
        LoadedPeriod _periodToLoad = LoadData();

        if(_periodToLoad == null)
        {
            Debug.Log("No save data found or save data invalid, starting new game...");
            StartGame();
        }
        else
        {
            LoadPeriod(_periodToLoad.gamePeriod, _periodToLoad.isStealthy, false);
        }
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
                playerHasControl = false;
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

    //EventFinder Override
    public void DisablePlayerInput(EventFunctionParams functionParams)
    {
        DisablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        if(spawnedPlayer != null)
        {
            PlayerMovement _playerMovement = spawnedPlayer.GetComponent<PlayerMovement>();
            PlayerInteract _playerInteract = spawnedPlayer.GetComponent<PlayerInteract>();
            if (_playerMovement != null)
            {
                playerHasControl = true;
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

    //EventFinder Override
    public void EnablePlayerInput(EventFunctionParams functionParams)
    {
        EnablePlayerInput();
    }

    public void ActivateQuests(List<string> _questIds)
    {
        foreach(QuestInfo _questInfo in curQuestInfos)
        {
            if(_questInfo.quest.curQuestState == QuestState.Inactive)
            {
                foreach(string _id in _questIds)
                {
                    if(_id == _questInfo.quest.idQuestName)
                    {
                        _questInfo.quest.OnInactiveToStart();
                        break;
                    }
                }
            }
        }
    }

    public void ActivateQuests(string _questId)
    {
        foreach (QuestInfo _questInfo in curQuestInfos)
        {
            if (_questInfo.quest.curQuestState == QuestState.Inactive)
            {
                if (_questId == _questInfo.quest.idQuestName)
                {
                    _questInfo.quest.OnInactiveToStart();
                    break;
                }
            }
        }
    }

    //EventFinder Overload
    public void ActivateQuests(EventFunctionParams functionParams)
    {
        List<string> _questIds = new List<string>(functionParams.stringParams);
        ActivateQuests(_questIds);
    }

    public void AddItemsToInventory(List<string> _itemIds)
    {
        if (HasSpawnedPlayer())
        {
            foreach (string _id in _itemIds)
            {
                bool hasFound = false;

                foreach (QuestItemScriptableObject _gmItem in curFloatingItems)
                {
                    if (_id == _gmItem.idItemName)
                    {
                        PlayerInventory.instance.PickupItem(_gmItem);
                        break;
                    }
                }

                if (!hasFound)
                {
                    Debug.LogWarning("Item (" + _id + ") not found in GameManager's floating items!");
                }
            }
        }
    }

    public void AddItemsToInventory(string _itemId)
    {
        if (HasSpawnedPlayer())
        {
            bool hasFound = false;

            foreach (QuestItemScriptableObject _gmItem in curFloatingItems)
            {
                if (_itemId == _gmItem.idItemName)
                {
                    PlayerInventory.instance.PickupItem(_gmItem);
                    break;
                }
            }

            if (!hasFound)
            {
                Debug.LogWarning("Item (" + _itemId + ") not found in GameManager's floating items!");
            }
        }
    }

    //EventFinder Override
    public void AddItemsToInventory(EventFunctionParams functionParams)
    {
        List<string> _itemIds = new List<string>(functionParams.stringParams);
        AddItemsToInventory(_itemIds);
    }

    //Removes EVERY Item from the Inventory, if it exists
    public void ClearInventory()
    {
        if (HasSpawnedPlayer())
        {
            PlayerInventory.instance.ClearInventory();
        }
        else
        {
            Debug.LogWarning("No Player to clear inventory from!");
        }
    }

    public void StartCutscene(string cutsceneTitle)
    {
        if(SceneInfoContainer.instance != null)
        {
            SceneInfoContainer.instance.StartCutscene(cutsceneTitle);
        }
        else
        {
            Debug.LogWarning("Can't start cutscene (" + cutsceneTitle + ") because there is no SceneInfoContainer in the scene!");
        }
    }

    //EventFinder Override
    public void StartCutscene(EventFunctionParams functionParams)
    {
        StartCutscene(functionParams.stringParams[0]);
    }
}
