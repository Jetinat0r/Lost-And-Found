using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Object Info Structs
[System.Serializable]
public struct TimeBlock
{
    public int day;
    public int time;

    public void AdvanceTime(int _day, int _time)
    {
        day += _day;
        time += _time;

        bool isNewDay = false;
        while (time >= 24)
        {
            day += 1;
            time -= 24;
            isNewDay = true;
        }

        if (isNewDay)
        {
            time = 6;
        }
    }

    public void SetTime(int _day, int _time)
    {
        day = _day;
        time = _time;
    }

    public bool IsEqual(TimeBlock other)
    {
        if (day == other.day && time == other.time)
        {
            return true;
        }

        return false;
    }
}
[System.Serializable]
public struct ObjectSceneInfo
{
    public string nodeId;
    public Vector3 positionInScene;
}
//[System.Serializable]

//public struct PhysicalQuestItemInfo
//{
//    public List<TimeBlock> spawnTimes;

//    public QuestItemPhysical itemPrefab;
//    public ObjectSceneInfo objectSceneInfo;
//}
//[System.Serializable]
//public struct QuestInfo
//{
//    public List<TimeBlock> spawnTimes;

//    public QuestScriptableObject quest;
//    public QuestState initialQuestState;

//    public QuestNpcInfo questNpc;
//}
[System.Serializable]
public struct NpcSceneInfo
{
    public int priority;
    public ObjectSceneInfo objectSceneInfo;
}
[System.Serializable]
public struct QuestNpcInfo
{
    #region NPC States
    public GameObject inactiveNpcPrefab;
    public NpcSceneInfo inactiveNpcSceneInfo;
    public DialogueScriptableObject inactiveDefaultNpcDialogue;
    [Space(10)]

    public GameObject startNpcPrefab;
    public NpcSceneInfo startNpcSceneInfo;
    public DialogueScriptableObject startDefaultNpcDialogue;
    [Space(10)]

    public GameObject inProgressNpcPrefab;
    public NpcSceneInfo inProgressNpcSceneInfo;
    public DialogueScriptableObject inProgressDefaultNpcDialogue;
    [Space(10)]

    public GameObject endNpcPrefab;
    public NpcSceneInfo endNpcSceneInfo;
    public DialogueScriptableObject endDefaultNpcDialogue;
    [Space(10)]

    public GameObject completedNpcPrefab;
    public NpcSceneInfo completedNpcSceneInfo;
    public DialogueScriptableObject completedDefaultNpcDialogue;
    [Space(10)]

    public GameObject failedNpcPrefab;
    public NpcSceneInfo failedNpcSceneInfo;
    public DialogueScriptableObject failedDefaultNpcDialogue;
    #endregion

    public int GetPriority(QuestState _curState)
    {
        int _priority = -1;

        //Gets priority based on current quest state
        switch (_curState)
        {
            case (QuestState.Inactive):
                _priority = inactiveNpcSceneInfo.priority;
                break;

            case (QuestState.Start):
                _priority = startNpcSceneInfo.priority;
                break;

            case (QuestState.InProgress):
                _priority = inProgressNpcSceneInfo.priority;
                break;

            case (QuestState.End):
                _priority = endNpcSceneInfo.priority;
                break;

            case (QuestState.Completed):
                _priority = completedNpcSceneInfo.priority;
                break;

            case (QuestState.Failed):
                _priority = failedNpcSceneInfo.priority;
                break;
        }

        return _priority;
    }

    public NpcSceneInfo GetScene(QuestState _curState)
    {
        NpcSceneInfo _activeScene = inactiveNpcSceneInfo;

        switch (_curState)
        {
            case (QuestState.Inactive):
                _activeScene = inactiveNpcSceneInfo;
                break;

            case (QuestState.Start):
                _activeScene = startNpcSceneInfo;
                break;

            case (QuestState.InProgress):
                _activeScene = inProgressNpcSceneInfo;
                break;

            case (QuestState.End):
                _activeScene = endNpcSceneInfo;
                break;

            case (QuestState.Completed):
                _activeScene = completedNpcSceneInfo;
                break;

            case (QuestState.Failed):
                _activeScene = failedNpcSceneInfo;
                break;
        }

        return _activeScene;
    }

    public GameObject GetNpcPrefab(QuestState _curState)
    {
        GameObject _prefab = inactiveNpcPrefab;

        //Gets priority based on current quest state
        switch (_curState)
        {
            case (QuestState.Inactive):
                _prefab = inactiveNpcPrefab;
                break;

            case (QuestState.Start):
                _prefab = startNpcPrefab;
                break;

            case (QuestState.InProgress):
                _prefab = inProgressNpcPrefab;
                break;

            case (QuestState.End):
                _prefab = endNpcPrefab;
                break;

            case (QuestState.Completed):
                _prefab = completedNpcPrefab;
                break;

            case (QuestState.Failed):
                _prefab = failedNpcPrefab;
                break;
        }

        return _prefab;
    }

    public DialogueScriptableObject GetDefaultDialogue(QuestState _curState)
    {
        DialogueScriptableObject _dialogue = inactiveDefaultNpcDialogue;

        //Gets priority based on current quest state
        switch (_curState)
        {
            case (QuestState.Inactive):
                _dialogue = inactiveDefaultNpcDialogue;
                break;

            case (QuestState.Start):
                _dialogue = startDefaultNpcDialogue;
                break;

            case (QuestState.InProgress):
                _dialogue = inProgressDefaultNpcDialogue;
                break;

            case (QuestState.End):
                _dialogue = endDefaultNpcDialogue;
                break;

            case (QuestState.Completed):
                _dialogue = completedDefaultNpcDialogue;
                break;

            case (QuestState.Failed):
                _dialogue = failedDefaultNpcDialogue;
                break;
        }

        return _dialogue;
    }
}
//[System.Serializable]
//public struct FillerNpcInfo
//{
//    public List<TimeBlock> spawnTimes;

//    public GameObject npcPrefab;
//    public NpcSceneInfo sceneInfo;

//    //These are default dialogues for if the npc in question has no quest associated with them.
//    //Until I implement popularity, unpopularDialogue is going to be the only one used
//    public DialogueScriptableObject unpopularDialogue;
//    public DialogueScriptableObject neutralDialogue;
//    public DialogueScriptableObject popularDialogue;
//}
#endregion

[CreateAssetMenu(fileName = "NewGamePeriod", menuName = "ScriptableObjects/Game Period")]
public class GamePeriod : ScriptableObject
{
    public TimeBlock timeBlock;

    [SerializeField]
    private string displayDate;
    [SerializeField]
    private string displayTime;

    //The node that the period will drop you in when the period is loaded
    public string startingNodeId;

    public List<QuestInfo> questInfos;
    public List<PhysicalQuestItemInfo> physicalQuestItemInfos;
    public List<FillerNpcInfo> fillerNpcInfos;
    public List<QuestItemScriptableObject> floatingItems;

    public string GetDisplayTime()
    {
        return displayDate + "\n"
            + displayTime;
    }

    public override string ToString()
    {
        string _information = "Display Date: " + displayDate + "\n"
            + "DisplayTime: " + displayTime + "\n"
            + "Day: " + timeBlock.day + "\n"
            + "Time: " + timeBlock.time + "\n"
            + "Quest Infos: " + questInfos.Count + "\n"
            + "Physical Quest Item Infos: " + physicalQuestItemInfos.Count + "\n"
            + "Filler NPCs: " + fillerNpcInfos.Count;

        return _information;
    }
}