using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestItem", menuName = "ScriptableObjects/QuestItemScriptableObject")]
public class QuestItemScriptableObject : ScriptableObject
{
    public string idItemName;
    public string displayItemName;
    public string itemDescription;

    public bool isDisplayed = true;
}
