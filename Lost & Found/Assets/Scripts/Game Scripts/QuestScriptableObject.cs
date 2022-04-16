using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestState
{
    Inactive = 0,   //Talk to NPC before they have a quest for the player
    Start,          //Talk to NPC when they have a new quest for the player
    InProgress,     //Talk to NPC during the quest they gave the player
    End,            //Talk to NPC after collecting everything necessary, to finish the quest
    Completed,      //Talk to NPC after the quest has been completed
    Failed          //Talk to NPC after failing the quest
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "ScriptableObjects/QuestScriptableObject")]
[Serializable]
public class QuestScriptableObject : ScriptableObject
{
    //TODO: Add struct fields so that the quest can move its characters around depending on its state
    //      and a bool to check if the quest should bother using the fields (to not break anything we put down before implementation)

    [Tooltip("Name used for quest searching (i.e. intro_quest_01)")]
    public string idQuestName;

    [Tooltip("Names used for searching (i.e. knife_story_01)")]
    public List<string> idQuestItemNames;

    public QuestState initialQuestState = QuestState.Inactive;
    public QuestState curQuestState = QuestState.Inactive;

    //public UnityEvent runOnComplete;
    [Tooltip("Names of quests to attempt to unlock upon the completion of this one")]
    public List<string> unlockOnComplete;

    [Space]
    public string inactiveIdNpcName;
    public string startIdNpcName;
    public string inProgressIdNpcName;
    public string endIdNpcName;
    public string completedIdNpcName;
    public string failedIdNpcName;

    [Tooltip("Name displayed in player notebook")]
    public string displayQuestName;

    //To tell the player who to go to
    public string displayNpcName;

    [Tooltip("Description displayed in player notebook")]
    [TextArea(1, 3)]
    public string displayQuestDescription;

    //Descriptions for each of the quest items
    //[Tooltip("WARNING - THIS IS GOING TO BE MOVED TO THE ITEM SCRIP OBJ")]
    //public List<string> displayQuestItemDescriptions;

    [Tooltip("Should the quest be displayed in the journal")]
    public bool isDisplayed = true;

    [Tooltip("How many reputation points this quest gains you upon completion")]
    public float reputationPoints;

    [Space]
    [Header("Dialogue")]
    public DialogueScriptableObject inactiveDialogue;
    public DialogueScriptableObject startDialogue;
    public DialogueScriptableObject inProgressDialogue;
    public DialogueScriptableObject endDialogue;
    public DialogueScriptableObject completedDialogue;
    public DialogueScriptableObject failedDialogue;

    [Header("Events")]
    public List<EventFunctionParams> onInactiveToStart;
    public List<EventFunctionParams> onStartToInProgress;
    public List<EventFunctionParams> onInProgressToEnd;
    public List<EventFunctionParams> onEndToCompleted;
    public List<EventFunctionParams> onStateToFailed;

    public DialogueScriptableObject GetCurrentDialogue()
    {
        //TODO: Make talk
        switch (curQuestState)
        {
            case (QuestState.Inactive):
                return inactiveDialogue;
            case (QuestState.Start):
                //TODO: Not this
                curQuestState = QuestState.InProgress;
                if (CheckRequiredItems())
                {
                    OnInProgressToEnd();
                }

                return startDialogue;
            case (QuestState.InProgress):
                if (CheckRequiredItems())
                {
                    OnInProgressToEnd();
                }

                return inProgressDialogue;
            case (QuestState.End):
                //TODO: Not this
                curQuestState = QuestState.Completed;
                //runOnComplete?.Invoke();

                return endDialogue;
            case (QuestState.Completed):
                return completedDialogue;
            default:
                Debug.LogWarning("No Quest State!");
                return null;
        }
    }

    public void InitializeQuestState()
    {
        curQuestState = initialQuestState;

        switch (curQuestState)
        {
            case (QuestState.Start):
                OnInactiveToStart();
                break;

            case (QuestState.InProgress):
                OnStartToInProgress();

                if (CheckRequiredItems())
                {
                    OnInProgressToEnd();
                }
                break;

            case (QuestState.End):
                OnInProgressToEnd();
                break;

            case (QuestState.Completed):
                OnEndToCompleted();
                break;

            case (QuestState.Failed):
                OnStateToFailed();
                break;
        }

        if(curQuestState != QuestState.Inactive)
        {
            //TODO: Add to questbook
        }
    }

    public void OnInactiveToStart()
    {
        curQuestState = QuestState.Start;

        CallEvents(onInactiveToStart);
    }

    public void OnStartToInProgress()
    {
        curQuestState = QuestState.InProgress;

        CallEvents(onStartToInProgress);

        if (CheckRequiredItems())
        {
            OnInProgressToEnd();
        }
    }

    public void OnInProgressToEnd()
    {
        curQuestState = QuestState.End;

        CallEvents(onInProgressToEnd);
        //TODO: remove specified items from inventory
        //TODO: fail other specified quests
    }

    public void OnEndToCompleted()
    {
        curQuestState = QuestState.Completed;

        GameManager.instance.ActivateQuests(unlockOnComplete);

        CallEvents(onEndToCompleted);
    }

    public void OnStateToFailed()
    {
        //TODO: If(QuestState != Inactive) Then(Notify player) also check if the player has already completed or failed said quest
        curQuestState = QuestState.Failed;

        CallEvents(onStateToFailed);
    }

    private void CallEvents(List<EventFunctionParams> funcParamsList)
    {
        foreach (EventFunctionParams funcParams in funcParamsList)
        {
            EventFinder.instance.CallFunction(funcParams);
        }

        SceneController.instance.DetermineSpawnedObjectStates();
    }

    public string GetCurNpcId()
    {
        string _id = inactiveIdNpcName;

        switch (curQuestState)
        {
            case (QuestState.Inactive):
                _id = inactiveIdNpcName;
                break;

            case (QuestState.Start):
                _id = startIdNpcName;
                break;

            case (QuestState.InProgress):
                _id = inProgressIdNpcName;
                break;

            case (QuestState.End):
                _id = endIdNpcName;
                break;

            case (QuestState.Completed):
                _id = completedIdNpcName;
                break;

            case (QuestState.Failed):
                _id = failedIdNpcName;
                break;
        }

        return _id;
    }

    public bool CheckRequiredItems()
    {
        foreach(string _itemId in idQuestItemNames)
        {
            if (GameManager.instance.HasSpawnedPlayer())
            {
                if (!PlayerInventory.instance.CheckItem(_itemId))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
