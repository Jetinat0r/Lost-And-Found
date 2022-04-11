using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private class NpcSpawn
    {
        public GameObject npcPrefab;
        public NPC npcScript;
        public DialogueScriptableObject defaultDialogue;

        public int priority;
        public ObjectSceneInfo objectSceneInfo;
    }

    private Coroutine sceneObjectLoadRoutine;
    
    [SerializeField]
    private WorldObject worldObject;
    public WorldNode curWorldNode = null;
    public string curNodeId = "main_menu";

    //Used for transitioning between scenes w/o needing a whole bunch of overloaded functions
    private string lastConnectionTitle = "";

    //Used specifically for doors really, because the door has to stop the player, but *maybe*
    //allow them to move after the transition
    public delegate void RunOnSceneLoad();
    private RunOnSceneLoad runOnSceneLoad = null;

    private List<NPC> spawnedNpcs;
    private List<QuestItemPhysical> spawnedQuestItems;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void SetScene(string _sceneName)
    {
        //Start fade transition
        await FadeTransitionManager.instance.StartTransition();

        //Change the scene
        Scene _goToScene = SceneManager.GetSceneByName(_sceneName);

        if (_sceneName != "" && _goToScene != null)
        {
            SceneManager.LoadScene(_sceneName);

            //MovePlayer();

            sceneObjectLoadRoutine = StartCoroutine(WaitForSceneLoad());
        }
        else
        {
            Debug.LogWarning("Scene: (" + _sceneName + ") not found!");
        }
    }

    public async void SetScene(string _sceneName, string _connectionTitle)
    {
        //Start fade transition
        await FadeTransitionManager.instance.StartTransition();

        //Change the scene
        Scene _goToScene = SceneManager.GetSceneByName(_sceneName);

        if (_sceneName != "" && _goToScene != null)
        {
            SceneManager.LoadScene(_sceneName);

            //MovePlayer(_connectionTitle);
            lastConnectionTitle = _connectionTitle;

            sceneObjectLoadRoutine = StartCoroutine(WaitForSceneLoad());
        }
        else
        {
            Debug.LogWarning("Scene: (" + _sceneName + ") not found!");
        }
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return null;

        LoadSceneObjects();
    }

    //This is for finding a node through the world w/o requiring an initial node
    //Will be used for moving to and from main_menu mainly
    public void GotoNode(string _nodeTitle)
    {
        curWorldNode = worldObject.GetNode(_nodeTitle);
        curNodeId = curWorldNode.title;

        SetScene(curWorldNode.GetSceneTitle(GameManager.instance.GetCurrentPeriod()));
    }

    //EventFinder Overload
    public void GotoNode(EventFunctionParams functionParams)
    {
        //Debug.Log(functionParams.stringParams);
        GotoNode(functionParams.stringParams[0]);
    }

    public WorldNodeConnector GetConnectorFromTitle(string _connectionTitle)
    {
        return worldObject.GetConnectorFromTitle(curNodeId, _connectionTitle);
    } 

    public void MoveThroughConnection(string _connectionTitle)
    {
        curWorldNode = curWorldNode.GetConnectedNode(_connectionTitle);
        curNodeId = curWorldNode.title;

        SetScene(curWorldNode.GetSceneTitle(GameManager.instance.GetCurrentPeriod()), _connectionTitle);
    }

    public void MoveThroughConnection(string _connectionTitle, RunOnSceneLoad _runOnSceneLoad)
    {
        curWorldNode = curWorldNode.GetConnectedNode(_connectionTitle);
        curNodeId = curWorldNode.title;

        runOnSceneLoad = _runOnSceneLoad;

        SetScene(curWorldNode.GetSceneTitle(GameManager.instance.GetCurrentPeriod()), _connectionTitle);
    }

    //EventFinder Overload (Only for the 1 string argument version)
    public void MoveThroughConnection(EventFunctionParams functionParams)
    {
        MoveThroughConnection(functionParams.stringParams[0]);
    }

    private void LoadSceneObjects()
    {
        //Moves the player to the correct position in the scene
        if(lastConnectionTitle != "")
        {
            MovePlayer(lastConnectionTitle);
            lastConnectionTitle = "";
        }
        else
        {
            MovePlayer();
        }

        //_curPeriod stores ALL info about what is to be spawned
        GamePeriod _curPeriod = GameManager.instance.GetCurrentPeriod();
        List<NpcSpawn> _npcs = new List<NpcSpawn>();

        //Populates a list with all quest NPCs that can spawn and where
        foreach (QuestInfo _questInfo in GameManager.instance.curQuestInfos)
        {
            #region Check Time Block
            bool isRightTime = false;
            foreach(TimeBlock _timeBlock in _questInfo.spawnTimes)
            {
                if (_timeBlock.Equals(GameManager.instance.curTime))
                {
                    isRightTime = true;
                }
            }

            //If the _questInfo is irrelevant at the current Time Block, don't bother spawning, but still check the rest of the things in curQuestInfos
            if (!isRightTime)
            {
                continue;
            }
            #endregion

            NpcSpawn _curNpc = null;

            foreach(NpcSpawn _npc in _npcs)
            {
                if(_questInfo.questNpc.GetNpcPrefab(_questInfo.quest.curQuestState).GetComponent<NPC>().idNpcName == _npc.npcScript.idNpcName)
                {
                    _curNpc = _npc;

                    break;
                }
            }

            if(_curNpc == null)
            {
                //Adds a new npc to the list

                _curNpc = new NpcSpawn();

                //Debug.Log("State: " + _questInfo.quest.curQuestState);

                _curNpc.npcPrefab = _questInfo.questNpc.GetNpcPrefab(_questInfo.quest.curQuestState);

                if(_curNpc.npcPrefab == null)
                {
                    Debug.LogWarning("NPC Does not exist!");
                   continue;
                }

                _curNpc.npcScript = _curNpc.npcPrefab.GetComponent<NPC>();
                _curNpc.priority = _questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState);
                _curNpc.objectSceneInfo = _questInfo.questNpc.GetScene(_questInfo.quest.curQuestState).objectSceneInfo;
                _curNpc.defaultDialogue = _questInfo.questNpc.GetDefaultDialogue(_questInfo.quest.curQuestState);

                _npcs.Add(_curNpc);
            }
            else
            {
                //If the npc is already in the list, checks to see if its spawn pos should be overridden and does so if necessary
                if (_questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState) > _curNpc.priority)
                {
                    _curNpc.priority = _questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState);
                    _curNpc.objectSceneInfo = _questInfo.questNpc.GetScene(_questInfo.quest.curQuestState).objectSceneInfo;
                    _curNpc.defaultDialogue = _questInfo.questNpc.GetDefaultDialogue(_questInfo.quest.curQuestState);
                }
            }
        }

        //Adds non-quest NPCs to the previousely aforementioned list
        foreach(FillerNpcInfo _fillerInfo in _curPeriod.fillerNpcInfos)
        {
            #region Check Time Block
            bool isRightTime = false;
            foreach (TimeBlock _timeBlock in _fillerInfo.spawnTimes)
            {
                if (_timeBlock.Equals(GameManager.instance.curTime))
                {
                    isRightTime = true;
                }
            }

            //If the _questInfo is irrelevant at the current Time Block, don't bother spawning, but still check the rest of the things in curQuestInfos
            if (!isRightTime)
            {
                continue;
            }
            #endregion


            NpcSpawn _curNpc = null;

            foreach(NpcSpawn _npc in _npcs)
            {
                if(_fillerInfo.npcPrefab.GetComponent<NPC>().idNpcName == _npc.npcScript.idNpcName)
                {
                    _curNpc = _npc;

                    break;
                }
            }

            if (_curNpc == null)
            {
                //Adds a new npc to the list
                _curNpc = new NpcSpawn();

                _curNpc.npcPrefab = _fillerInfo.npcPrefab;
                _curNpc.npcScript = _fillerInfo.npcPrefab.GetComponent<NPC>();
                _curNpc.priority = _fillerInfo.sceneInfo.priority;
                _curNpc.objectSceneInfo = _fillerInfo.sceneInfo.objectSceneInfo;
                _curNpc.defaultDialogue = _fillerInfo.unpopularDialogue;

                _npcs.Add(_curNpc);
            }
            else
            {
                //If the npc is already in the list, checks to see if its spawn pos should be overridden and does so if necessary
                if (_fillerInfo.sceneInfo.priority > _curNpc.priority)
                {
                    _curNpc.priority = _fillerInfo.sceneInfo.priority;
                    _curNpc.objectSceneInfo = _fillerInfo.sceneInfo.objectSceneInfo;
                }
            }
        }

        spawnedNpcs = new List<NPC>();
        //Spawns in each npc
        foreach(NpcSpawn _npc in _npcs)
        {
            if(_npc.objectSceneInfo.nodeId == curNodeId)
            {
                GameObject _newNpc = Instantiate(_npc.npcPrefab, _npc.objectSceneInfo.positionInScene, Quaternion.identity);
                NPC _curNewNpc = _newNpc.GetComponent<NPC>();
                //TODO: Tell the NPCs that they have LIFE!
                //      maybe include a new bool to see if they should use quest things? but not yet
                //Or do nothing
                spawnedNpcs.Add(_curNewNpc);

                if(_npc.defaultDialogue != null)
                {
                    _curNewNpc.defaultDialogue = _npc.defaultDialogue;
                }
            }
        }

        spawnedQuestItems = new List<QuestItemPhysical>();
        //Populates a list with all quest items that can spawn and where
        //foreach(QuestItemPhysical)
        foreach (PhysicalQuestItemInfo _itemInfo in _curPeriod.physicalQuestItemInfos)
        {
            #region Check Time Block
            bool isRightTime = false;
            foreach (TimeBlock _timeBlock in _itemInfo.spawnTimes)
            {
                if (_timeBlock.Equals(GameManager.instance.curTime))
                {
                    isRightTime = true;
                }
            }

            //If the _questInfo is irrelevant at the current Time Block, don't bother spawning, but still check the rest of the things in curQuestInfos
            if (!isRightTime)
            {
                continue;
            }
            #endregion

            if (_itemInfo.objectSceneInfo.nodeId == curNodeId)
            {
                QuestItemPhysical _newItem = Instantiate(_itemInfo.itemPrefab, _itemInfo.objectSceneInfo.positionInScene, Quaternion.identity);
                //Tell quest items how to behave
                spawnedQuestItems.Add(_newItem);

                _newItem.DetermineState();
            }
        }

        //Runs events
        runOnSceneLoad?.Invoke();
    }

    public void DetermineSpawnedObjectStates()
    {
        //Not used
        //foreach(NPC _spawnedNpc in spawnedNpcs)
        //{
        //    _spawnedNpc.DetermineState();
        //}

        foreach(QuestItemPhysical _spawnedQuestItem in spawnedQuestItems)
        {
            _spawnedQuestItem.DetermineState();
        }
    }

    private void MovePlayer()
    {
        SceneInfoContainer sceneInfoContainer = SceneInfoContainer.instance;

        if(sceneInfoContainer != null)
        {
            if (!GameManager.instance.HasSpawnedPlayer())
            {
                GameManager.instance.SpawnPlayer();
            }

            GameManager.instance.MovePlayer(sceneInfoContainer.GetConnectionPosition());
        }
        else
        {
            Debug.LogWarning("No SceneInfoContainer on Scene!");
        }
    }

    private void MovePlayer(string _connectionTitle)
    {
        SceneInfoContainer sceneInfoContainer = SceneInfoContainer.instance;

        if (sceneInfoContainer != null)
        {
            if (!GameManager.instance.HasSpawnedPlayer())
            {
                GameManager.instance.SpawnPlayer();
            }

            GameManager.instance.MovePlayer(sceneInfoContainer.GetConnectionPosition(_connectionTitle));
        }
        else
        {
            Debug.LogWarning("No SceneInfoContainer on Scene!");
        }
    }
}
