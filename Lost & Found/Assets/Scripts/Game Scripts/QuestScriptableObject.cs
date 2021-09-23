using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestState
{
    Inactive,   //Talk to NPC before they have a quest for the player
    Start,      //Talk to NPC when they have a new quest for the player
    InProgress, //Talk to NPC during the quest they gave the player
    End,        //Talk to NPC after collecting everything necessary, to finish the quest
    Completed   //Talk to NPC after the quest has been completed
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "ScriptableObjects/QuestScriptableObject")]
public class QuestScriptableObject : ScriptableObject
{
    public string questName;

    public List<string> questItems;

    [SerializeField]
    private QuestState initialQuestState = QuestState.Inactive;
    public QuestState curQuestState = QuestState.Inactive;

    public UnityEvent runOnComplete;
    public List<string> unlockOnComplete;

    [Space]
    [Header("Dialogue")]
    public DialogueScriptableObject inactiveDialogue;
    public DialogueScriptableObject startDialogue;
    public DialogueScriptableObject inProgressDialogue;
    public DialogueScriptableObject endDialogue;
    public DialogueScriptableObject completedDialogue;

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

                return startDialogue;
            case (QuestState.InProgress):
                return inProgressDialogue;
            case (QuestState.End):
                //TODO: Not this
                curQuestState = QuestState.Completed;
                runOnComplete?.Invoke();
                //TODO: remove
                AudioManager.instance.Play("Quest Complete 1");

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
    }
}
