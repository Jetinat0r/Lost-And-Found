using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private class NpcSpawn
    {
        public GameObject npcPrefab;
        public NPC npcScript;
        public int priority;

        public ObjectSceneInfo objectSceneInfo;
    }

    private Coroutine sceneObjectLoadRoutine;
    
    [SerializeField]
    private WorldObject worldObject;
    public WorldNode curWorldNode = null;
    public string curNodeId = "main_menu";

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

    public void SetScene(string _sceneName)
    {
        Scene _goToScene = SceneManager.GetSceneByName(_sceneName);

        if (_sceneName != "" && _goToScene != null)
        {
            SceneManager.LoadScene(_sceneName);

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

    public void MoveThroughConnection(string _connectionTitle)
    {
        curWorldNode = curWorldNode.GetConnectedNode(_connectionTitle);
        curNodeId = curWorldNode.title;

        SetScene(curWorldNode.GetSceneTitle(GameManager.instance.GetCurrentPeriod()));
    }

    private void LoadSceneObjects()
    {
        //_curPeriod stores ALL info about what is to be spawned
        GamePeriod _curPeriod = GameManager.instance.GetCurrentPeriod();
        List<NpcSpawn> _npcs = new List<NpcSpawn>();

        //Populates a list with all quest NPCs that can spawn and where
        foreach (QuestInfo _questInfo in _curPeriod.questInfos)
        {
            //TODO: Move this
            //_questInfo.quest.curQuestState = _questInfo.initialQuestState;

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
                    return;
                }

                _curNpc.npcScript = _curNpc.npcPrefab.GetComponent<NPC>();
                _curNpc.priority = _questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState);
                _curNpc.objectSceneInfo = _questInfo.questNpc.GetScene(_questInfo.quest.curQuestState).objectSceneInfo;

                _npcs.Add(_curNpc);
            }
            else
            {
                //If the npc is already in the list, checks to see if its spawn pos should be overridden and does so if necessary
                if (_questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState) > _curNpc.priority)
                {
                    _curNpc.priority = _questInfo.questNpc.GetPriority(_questInfo.quest.curQuestState);
                    _curNpc.objectSceneInfo = _questInfo.questNpc.GetScene(_questInfo.quest.curQuestState).objectSceneInfo;
                }
            }
        }

        //Adds non-quest NPCs to the previousely aforementioned list
        foreach(FillerNpcInfo _fillerInfo in _curPeriod.fillerNpcs)
        {
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

                _curNpc.npcPrefab = _fillerInfo.npcPrefab;
                _curNpc.npcScript = _fillerInfo.npcPrefab.GetComponent<NPC>();
                _curNpc.priority = _fillerInfo.sceneInfo.priority;
                _curNpc.objectSceneInfo = _fillerInfo.sceneInfo.objectSceneInfo;

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

        //Populates a list with all quest items that can spawn and where
        //foreach(QuestItemPhysical)

        foreach(NpcSpawn _npc in _npcs)
        {
            if(_npc.objectSceneInfo.nodeId == curNodeId)
            {
                GameObject _newNpc = Instantiate(_npc.npcPrefab, _npc.objectSceneInfo.positionInScene, Quaternion.identity);
                //TODO: Tell the NPCs that they have LIFE!
                //      maybe include a new bool to see if they should use quest things? but not yet
            }
        }

        foreach(PhysicalQuestItemInfo _itemInfo in _curPeriod.physicalQuestItems)
        {
            if(_itemInfo.objectSceneInfo.nodeId == curNodeId)
            {
                QuestItemPhysical _newItem = Instantiate(_itemInfo.itemPrefab, _itemInfo.objectSceneInfo.positionInScene, Quaternion.identity);
                //TODO: Tell quest items how to behave
            }
        }
    }
}
