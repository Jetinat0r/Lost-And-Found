using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    //For don't destroy on load
    [SerializeField]
    private GameObject dialogueBoxCanvas;
    //For enabling/disabling visuals
    [SerializeField]
    private GameObject dialogueBoxContainer;


    [SerializeField]
    private DialogueBox dialogueBox;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(dialogueBoxCanvas);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DialogueScriptableObject _dialogue, string _displayName)
    {
        ToggleDialogueContainer(true);

        dialogueBox.SetupDialogueBox(_dialogue, _displayName);
    }

    public void EndDialogue()
    {
        ToggleDialogueContainer(false);
    }

    private void ToggleDialogueContainer(bool _enable)
    {
        dialogueBoxContainer.SetActive(_enable);
    }
}
