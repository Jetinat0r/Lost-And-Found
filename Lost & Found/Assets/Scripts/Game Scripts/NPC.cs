using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : InteractionTarget
{
    //private InteractionTarget interactionTarget;

    
    [Tooltip("Name used for quest searches, here so if we change a character's name we don't have to change the id elsewhere")]
    public string idNpcName;

    public string displayNpcName;

    public bool canTalkTo = true;

    [SerializeField]
    private GameObject itemGraphics;

    //TODO: Change to a pooling system?
    [SerializeField]
    private GameObject dialogueBoxPrefab;

    public DialogueScriptableObject defaultDialogue;

    private void Start()
    {
        //interactionTarget = GetComponent<InteractionTarget>();

        //if(interactionTarget == null)
        //{
        //    Debug.LogWarning("No interaction target on NPC: " + displayNpcName);
        //}
    }

    //Kept separate for InteractionTarget bc not all targets will be NPCs/QuestItemPhysicals and so that I can do extra stuff with each state
    //Determines how the object will exist
    //Valid States:
    // - Inactive: Item is visible but not interactable
    // - Active: Item is visible and interactable
    // - Completed: Item is not visible and not interactable (may just destroy, though this may cause problems w/ player interact)
    //
    //Called upon load in, quest state change, and interact
    public override void DetermineState()
    {
        GamePeriod curPeriod = GameManager.instance.GetCurrentPeriod();

        foreach (QuestInfo questInfo in curPeriod.questInfos)
        {
            //AKA if this npc was spawned due to a quest
            //Don't do this, compare the id's of the npcs first
            if(questInfo.questNpc.GetNpcPrefab(questInfo.quest.curQuestState).GetComponent<NPC>().idNpcName == idNpcName)
            {

            }


        }
        //this is where i will look at the state of a quest (the "refactor w/ lambdas thing from QuestItemPhysical")
        //TODO: If state is active, allow interact in InteractionTarget
    }

    public void ChangeItemHighlight(bool toggleOn)
    {
        //itemHighlight.SetActive(toggleOn);
    }

    public void TalkTo()
    {
        //Debug.Log("Talk to");

        //QuestScriptableObject npcQuest = null;

        Dictionary<QuestScriptableObject, QuestState> _questDict = new Dictionary<QuestScriptableObject, QuestState>();
        
        //Adds all available quests for the npc to a dictionary
        foreach(QuestScriptableObject quest in QuestManager.instance.curQuests)
        {
            if(quest.GetCurNpcId() == idNpcName)
            {
                _questDict.Add(quest, quest.curQuestState);
                //npcQuest = quest;

                //break;
            }
        }

        //Quest State Priority
        //1. End
        //2. Start
        //3. InProgress
        //4. Completed
        //5. Failed
        //6. Inactive
        QuestScriptableObject _npcQuest = null;
            
        foreach(KeyValuePair<QuestScriptableObject, QuestState> _questPair in _questDict)
        {
            if(_npcQuest == null)
            {
                _npcQuest = _questPair.Key;
            }
            else
            {
                QuestState _curQuestState = _questDict[_npcQuest];

                switch (_curQuestState)
                {
                    case (QuestState.End):
                        //Do nothing
                        break;

                    case (QuestState.Start):
                        if(_questPair.Value == QuestState.End)
                        {
                            _npcQuest = _questPair.Key;
                        }
                        break;

                    case (QuestState.InProgress):
                        if (_questPair.Value == QuestState.End || _questPair.Value == QuestState.Start)
                        {
                            _npcQuest = _questPair.Key;
                        }
                        break;

                    case (QuestState.Completed):
                        if (_questPair.Value == QuestState.End || _questPair.Value == QuestState.Start
                            || _questPair.Value == QuestState.InProgress)
                        {
                            _npcQuest = _questPair.Key;
                        }
                        break;

                    case (QuestState.Failed):
                        if (_questPair.Value == QuestState.End || _questPair.Value == QuestState.Start
                            || _questPair.Value == QuestState.InProgress || _questPair.Value == QuestState.Completed)
                        {
                            _npcQuest = _questPair.Key;
                        }
                        break;

                    case (QuestState.Inactive):
                        if(_questPair.Value != QuestState.Inactive)
                        {
                            _npcQuest = _questPair.Key;
                        }
                        break;

                    default:
                        Debug.LogWarning("Invalid quest state??");
                        break;
                }
            }
        }

        //Display Dialogue
        if(_npcQuest != null)
        {
           DialogueManager.instance.StartDialogue(_npcQuest.GetCurrentDialogue(), displayNpcName);
        }
        else
        {
            Debug.Log("No quest assigned for character id ( " + idNpcName + "), using default dialogue...");

            if(defaultDialogue == null)
            {
                Debug.LogWarning("No default dialogue found!!!");
                return;
            }

            DialogueManager.instance.StartDialogue(defaultDialogue, displayNpcName);

        }
    }

    public void CheckCanInteract()
    {
        if (canTalkTo)
        {
            itCanInteract = true;
        }
    }
}
