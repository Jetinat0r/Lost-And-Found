using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestItemPhysical : MonoBehaviour
{
    private InteractionTarget interactionTarget;

    [SerializeField]
    private QuestItemScriptableObject questItemScriptableObject;

    private bool canPickUp = false;

    public UnityEvent onInteract;

    //[SerializeField]
    //private GameObject itemGraphics;

    private void Start()
    {
        interactionTarget = GetComponent<InteractionTarget>();

        if(interactionTarget == null)
        {
            Debug.LogWarning("No interaction target on Physical Quest Item: " + questItemScriptableObject.idItemName);
        }
    }

    //TODO: Refactor with lambdas
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
                        canPickUp = true;

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

        Destroy(gameObject);
    }

    public void StartMinigame()
    {
        //TODO: Implement
        //TODO: add an event for pickupItem if the minigame completes successfully
    }
    #endregion

    public void CheckCanInteract()
    {
        if (CheckIfQuestActive(QuestManager.instance.curQuests))
        {
            interactionTarget.canInteract = true;
        }
    }
}
