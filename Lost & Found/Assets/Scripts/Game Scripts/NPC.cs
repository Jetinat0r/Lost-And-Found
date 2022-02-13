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

        QuestScriptableObject npcQuest = null;
        foreach(QuestScriptableObject quest in QuestManager.instance.curQuests)
        {
            if(quest.GetCurNpcId() == idNpcName)
            {
                npcQuest = quest;

                break;
            }
        }

        if(npcQuest == null)
        {
            Debug.LogWarning("No Quest Assigned!!!");

            return;
        }

        //Debug.Log("TALKING");

        //TODO: If this is the approach I go with, have a canvas set up in the world to attatch this to
        GameObject _dialogueBoxPrefab = Instantiate(dialogueBoxPrefab);
        DialogueBox _textBox = _dialogueBoxPrefab.GetComponentInChildren<DialogueBox>();

        _textBox.SetupDialogueBox(npcQuest.GetCurrentDialogue(), displayNpcName);
    }

    public void CheckCanInteract()
    {
        if (canTalkTo)
        {
            itCanInteract = true;
        }
    }
}
