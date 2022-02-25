using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestItemPhysical : InteractionTarget
{
    //private InteractionTarget interactionTarget;

    [SerializeField]
    private GameObject itemSprite;

    [SerializeField]
    private QuestItemScriptableObject questItemScriptableObject;

    private bool canPickUp = false;

    public UnityEvent onInteract;

    //[SerializeField]
    //private GameObject itemGraphics;

    private void Start()
    {
        //interactionTarget = GetComponent<InteractionTarget>();

        //if (interactionTarget == null)
        //{
        //    Debug.LogWarning("No interaction target on Physical Quest Item: " + questItemScriptableObject.idItemName);
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
        //Determine the item's most prevalent state (stated above)
        QuestState highestState = QuestState.Inactive;
        foreach(QuestInfo info in GameManager.instance.curQuestInfos)
        {
            foreach(string id in info.quest.idQuestItemNames)
            {
                if(id == questItemScriptableObject.idItemName)
                {
                    switch (info.quest.curQuestState)
                    {
                        case (QuestState.End):
                        case (QuestState.Completed):
                        case (QuestState.Failed):
                            highestState = info.quest.curQuestState;
                            break;

                        case (QuestState.InProgress):
                            if(highestState != QuestState.End && highestState != QuestState.Completed
                                && highestState != QuestState.Failed)
                            {
                                highestState = info.quest.curQuestState;
                            }
                            break;
                    }
                }
            }
        }


        //If "InProgress" determine if the player has already picked up the item
        //If so, make it so that the item dissapears and can't be picked up
        //(here done by setting highestState to one that acts like that)
        if(highestState == QuestState.InProgress)
        {
            foreach(QuestItemScriptableObject heldItem in PlayerInventory.instance.curHeldItems)
            {
                if(heldItem.idItemName == questItemScriptableObject.idItemName)
                {
                    highestState = QuestState.Completed;
                }
            }
        }
        
        //If state is done, don't show and don't allow interact
        //If state is active, allow interact in InteractionTarget
        //If state is inactive, show but don't allow interact
        switch (highestState)
        {
            //Don't display the item; already picked up
            case (QuestState.End):
            case (QuestState.Completed):
            case (QuestState.Failed):
                itCanInteract = false;
                itemSprite.SetActive(false);
                break;

            //Display the item; can be picked up
            case (QuestState.InProgress):
                itCanInteract = true;
                break;

            //Display the item; can't be picked up
            default:
                itCanInteract = false;
                break;
        }
    }

    //TODO: Refactor with lambdas
    //or change entirely
    //OUTDATED?
    public bool CheckIfQuestActive(List<QuestScriptableObject> curActiveQuests)
    {
        foreach(QuestScriptableObject quest in curActiveQuests)
        {
            if(quest.curQuestState == QuestState.InProgress)
            {
                foreach (string itemName in quest.idQuestItemNames)
                {
                    if (itemName == questItemScriptableObject.idItemName)
                    {
                        //TODO: Activate pickup
                        //canPickUp = true;

                        //ChangeItemHighlight(true);

                        return true;
                    }
                }
            }
            
            
        }

        //ChangeItemHighlight(false);
        canPickUp = false;

        return false;
    }

    //public void ChangeItemHighlight(bool toggleOn)
    //{
    //    itemHighlight.SetActive(toggleOn);
    //}

    #region AvailableEvents
    public void PickupItem()
    {
        //Debug.Log("Add item");
        PlayerInventory.instance.PickupItem(questItemScriptableObject);

        //TODO: call onTake
        //Destroy(gameObject);
        itemSprite.SetActive(false);
        itCanInteract = false;
    }

    public void StartMinigame()
    {
        //TODO: Implement
        //TODO: add an event for pickupItem if the minigame completes successfully
    }
    #endregion

    //OUTDATED
    public void CheckCanInteract()
    {
        if (CheckIfQuestActive(QuestManager.instance.curQuests))
        {
            itCanInteract = true;
        }
    }
}
