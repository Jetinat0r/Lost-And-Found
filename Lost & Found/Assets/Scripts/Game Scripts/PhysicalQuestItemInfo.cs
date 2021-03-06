using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPhysicalQuestItemInfo", menuName = "ScriptableObjects/Physical Quest Item Info")]
public class PhysicalQuestItemInfo : ScriptableObject
{
    //Used for saving and loading
    public string id;

    public List<TimeBlock> spawnTimes;
    //TODO: Add an "end time" to remove when the curTimeBlock == it

    public QuestItemPhysical itemPrefab;
    public ObjectSceneInfo objectSceneInfo;
}
