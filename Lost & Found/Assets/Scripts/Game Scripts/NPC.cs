using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string questName;

    [SerializeField]
    private string npcName;

    private bool canTalkTo = true;

    [SerializeField]
    private GameObject itemGraphics;
    //NOTE: itemHighlight should be a child of itemGraphics
    [SerializeField]
    private GameObject itemHighlight;

    //TODO: Change to a pooling system?
    [SerializeField]
    private GameObject dialogueBoxPrefab;

    public void ChangeItemHighlight(bool toggleOn)
    {
        itemHighlight.SetActive(toggleOn);
    }

    public void TalkTo()
    {
        Debug.Log("Talk to");

        QuestScriptableObject npcQuest = null;
        foreach(QuestScriptableObject quest in QuestManager.instance.curQuests)
        {
            if(quest.questName == questName)
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

        Debug.Log("TALKING");

        //TODO: If this is the approach I go with, have a canvas set up in the world to attatch this to
        GameObject _dialogueBoxPrefab = Instantiate(dialogueBoxPrefab);
        DialogueBox _textBox = _dialogueBoxPrefab.GetComponentInChildren<DialogueBox>();

        _textBox.SetupDialogueBox(npcQuest.GetCurrentDialogue(), npcName);
    }

    public void Interact()
    {
        if (canTalkTo)
        {
            //TODO: pick up item
            TalkTo();
        }
    }
}
