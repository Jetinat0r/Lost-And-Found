using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItem", menuName = "ScriptableObjects/QuestItemScriptableObject")]
public class QuestItemScriptableObject : ScriptableObject
{
    public string itemName;
    public string itemDescription;
}
