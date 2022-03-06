using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestInfo", menuName = "ScriptableObjects/Quest Info")]
public class QuestInfo : ScriptableObject
{
    public List<TimeBlock> spawnTimes;
    //TODO: Add an "end time" to remove when the curTimeBlock == it

    public QuestScriptableObject quest;
    //public QuestState initialQuestState;

    public QuestNpcInfo questNpc;
}
