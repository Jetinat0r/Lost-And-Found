using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFillerNpcInfo", menuName = "ScriptableObjects/Filler NPC Info")]
public class FillerNpcInfo : ScriptableObject
{
    public List<TimeBlock> spawnTimes;
    //TODO: Add an "end time" to remove when the curTimeBlock == it

    public GameObject npcPrefab;
    public NpcSceneInfo sceneInfo;

    //These are default dialogues for if the npc in question has no quest associated with them.
    //Until I implement popularity, unpopularDialogue is going to be the only one used
    public DialogueScriptableObject unpopularDialogue;
    public DialogueScriptableObject neutralDialogue;
    public DialogueScriptableObject popularDialogue;
}
