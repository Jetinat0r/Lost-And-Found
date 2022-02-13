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
[System.Serializable]

public struct PhysicalQuestItemInfo
{
    public QuestItemPhysical itemPrefab;
    public ObjectSceneInfo objectSceneInfo;
}
[System.Serializable]
public struct QuestInfo
{
    public QuestScriptableObject quest;
    public QuestState initialQuestState;

    public QuestNpcInfo questNpc;
}
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
    public NpcSceneInfo InactiveNpcSceneInfo;
    [Space(10)]

    public GameObject startNpcPrefab;
    public NpcSceneInfo StartNpcSceneInfo;
    [Space(10)]

    public GameObject inProgressNpcPrefab;
    public NpcSceneInfo InProgressNpcSceneInfo;
    [Space(10)]

    public GameObject endNpcPrefab;
    public NpcSceneInfo EndNpcSceneInfo;
    [Space(10)]

    public GameObject completedNpcPrefab;
    public NpcSceneInfo CompletedNpcSceneInfo;
    [Space(10)]

    public GameObject failedNpcPrefab;
    public NpcSceneInfo FailedNpcSceneInfo;
    #endregion

    public int GetPriority(QuestState _curState)
    {
        int _priority = -1;

        //Gets priority based on current quest state
        switch (_curState)
        {
            case (QuestState.Inactive):
                _priority = InactiveNpcSceneInfo.priority;
                break;

            case (QuestState.Start):
                _priority = StartNpcSceneInfo.priority;
                break;

            case (QuestState.InProgress):
                _priority = InProgressNpcSceneInfo.priority;
                break;

            case (QuestState.End):
                _priority = EndNpcSceneInfo.priority;
                break;

            case (QuestState.Completed):
                _priority = CompletedNpcSceneInfo.priority;
                break;

            case (QuestState.Failed):
                _priority = FailedNpcSceneInfo.priority;
                break;
        }

        return _priority;
    }

    public NpcSceneInfo GetScene(QuestState _curState)
    {
        NpcSceneInfo _activeScene = InactiveNpcSceneInfo;

        switch (_curState)
        {
            case (QuestState.Inactive):
                _activeScene = InactiveNpcSceneInfo;
                break;

            case (QuestState.Start):
                _activeScene = StartNpcSceneInfo;
                break;

            case (QuestState.InProgress):
                _activeScene = InProgressNpcSceneInfo;
                break;

            case (QuestState.End):
                _activeScene = EndNpcSceneInfo;
                break;

            case (QuestState.Completed):
                _activeScene = CompletedNpcSceneInfo;
                break;

            case (QuestState.Failed):
                _activeScene = FailedNpcSceneInfo;
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

}
[System.Serializable]
public struct FillerNpcInfo
{
    public GameObject npcPrefab;
    public NpcSceneInfo sceneInfo;

    //These are default dialogues for if the npc in question has no quest associated with them.
    //Until I implement popularity, unpopularDialogue is going to be the only one used
    public DialogueScriptableObject unpopularDialogue;
    public DialogueScriptableObject neutralDialogue;
    public DialogueScriptableObject popularDialogue;
}
#endregion

[CreateAssetMenu(fileName = "NewGamePeriod", menuName = "ScriptableObjects/Game Period")]
public class GamePeriod : ScriptableObject
{
    public TimeBlock timeBlock;
    public List<QuestInfo> questInfos;
    public List<PhysicalQuestItemInfo> physicalQuestItems;
    public List<FillerNpcInfo> fillerNpcs;

    public override string ToString()
    {
        string _information = "Day: " + timeBlock.day + "\n"
            + "Time: " + timeBlock.time;

        return _information;
    }
}
