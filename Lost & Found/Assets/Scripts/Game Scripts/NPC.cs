using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private InteractionTarget interactionTarget;

    
    [Tooltip("Name used for quest searches, here so if we change a character's name we don't have to change the id elsewhere")]
    public string idNpcName;

    public string displayNpcName;

    public bool canTalkTo = true;

    [SerializeField]
    private GameObject itemGraphics;
    //NOTE: itemHighlight should be a child of itemGraphics
    [SerializeField]
    private GameObject itemHighlight;

    //TODO: Change to a pooling system?
    [SerializeField]
    private GameObject dialogueBoxPrefab;

    private void Start()
    {
        interactionTarget = GetComponent<InteractionTarget>();

        if(interactionTarget == null)
        {
            Debug.LogWarning("No interaction target on NPC: " + displayNpcName);
        }
    }

    public void ChangeItemHighlight(bool toggleOn)
    {
        itemHighlight.SetActive(toggleOn);
    }

    public void TalkTo()
    {
        //Debug.Log("Talk to");

        QuestScriptableObject npcQuest = null;
        foreach(QuestScriptableObject quest in QuestManager.instance.curQuests)
        {
            if(quest.idNpcName == idNpcName)
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
            interactionTarget.canInteract = true;
        }
    }
}
