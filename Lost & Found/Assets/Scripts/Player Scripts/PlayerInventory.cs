using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<QuestItemScriptableObject> curHeldItems;

    private void Awake()
    {
        //Singleton convention, allows access from anything at any time via PlayerInventory.instance
        if(PlayerInventory.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PickupItem(QuestItemScriptableObject questItem)
    {
        curHeldItems.Add(questItem);

        //TODO: Refactor to be smaller
        //Priority: Low
        foreach (QuestScriptableObject quest in QuestManager.instance.GetActiveQuests())
        {
            if (quest.idQuestItemNames.Contains(questItem.idItemName))
            {
                bool hasAllItems = true;

                //If this loop does not break, the player has all the necessary items
                foreach(string itemName in quest.idQuestItemNames)
                {
                    bool hasItem = false;

                    foreach(QuestItemScriptableObject heldQuestItem in curHeldItems)
                    {
                        if(heldQuestItem.idItemName == itemName)
                        {
                            hasItem = true;

                            break;
                        }
                    }

                    if (!hasItem)
                    {
                        hasAllItems = false;
                        break;
                    }
                }

                if (hasAllItems)
                {
                    quest.OnInProgressToEnd();
                }
            }
        }
    }
}
