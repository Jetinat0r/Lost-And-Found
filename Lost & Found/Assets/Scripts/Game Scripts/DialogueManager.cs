using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    //Used for starting a dialogue via another dialogue really
    //[System.Serializable]
    //public struct DialogueHolder
    //{
    //    public DialogueScriptableObject dialogue;
    //    public string displayName;
    //    public UnityEvent runEventsOnComplete;
    //}

    //For don't destroy on load
    [SerializeField]
    private GameObject dialogueBoxCanvas;
    //For enabling/disabling visuals
    [SerializeField]
    private GameObject dialogueBoxContainer;

    //Used to hide portions of the dialogue UI for the "None" and "Empty" moods
    [SerializeField]
    private GameObject portraitContainer;
    [SerializeField]
    private GameObject nameContainer;


    [SerializeField]
    private DialogueBox dialogueBox;

    public delegate void RunOnComplete();
    private RunOnComplete runOnComplete;

    private UnityEvent runEventsOnComplete;
    private List<EventFunctionParams> runEventFunctionsOnComplete;

    private DialogueScriptableObject nextDialogue = null;
    private string nextDialogueDisplayName = "";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(dialogueBoxCanvas);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region StartDialogue Overloads
    //public void StartDialogue(DialogueScriptableObject _dialogue, string _displayName)
    //{
    //    nextDialogue = null;
    //    nextDialogueDisplayName = "";

    //    ToggleDialogueContainer(true);

    //    if(_dialogue.nextDialogue != null)
    //    {
    //        nextDialogue = _dialogue.nextDialogue;
    //        nextDialogueDisplayName = _dialogue.nextDialogueDisplayName;
    //    }

    //    dialogueBox.SetupDialogueBox(_dialogue, _displayName);
    //}

    //public void StartDialogue(DialogueScriptableObject _dialogue, string _displayName, RunOnComplete _runOnComplete)
    //{
    //    nextDialogue = null;
    //    nextDialogueDisplayName = "";

    //    ToggleDialogueContainer(true);

    //    runOnComplete = _runOnComplete;

    //    if (_dialogue.nextDialogue != null)
    //    {
    //        nextDialogue = _dialogue.nextDialogue;
    //        nextDialogueDisplayName = _dialogue.nextDialogueDisplayName;
    //    }

    //    dialogueBox.SetupDialogueBox(_dialogue, _displayName);
    //}

    //public void StartDialogue(DialogueScriptableObject _dialogue, string _displayName, UnityEvent _runEventsOnComplete)
    //{
    //    nextDialogue = null;
    //    nextDialogueDisplayName = "";

    //    ToggleDialogueContainer(true);

    //    runEventsOnComplete = _runEventsOnComplete;

    //    if (_dialogue.nextDialogue != null)
    //    {
    //        nextDialogue = _dialogue.nextDialogue;
    //        nextDialogueDisplayName = _dialogue.nextDialogueDisplayName;
    //    }

    //    dialogueBox.SetupDialogueBox(_dialogue, _displayName);
    //}

    public void StartDialogue(DialogueScriptableObject _dialogue, string _displayName, RunOnComplete _runOnComplete = null, UnityEvent _runEventsOnComplete = null, List<EventFunctionParams> _runEventFunctionsOnComplete = null)
    {
        nextDialogue = null;
        nextDialogueDisplayName = "";

        ToggleDialogueContainer(true);

        runOnComplete = _runOnComplete;
        runEventsOnComplete = _runEventsOnComplete;
        runEventFunctionsOnComplete = _runEventFunctionsOnComplete;

        if (_dialogue.nextDialogue != null)
        {
            nextDialogue = _dialogue.nextDialogue;
            nextDialogueDisplayName = _dialogue.nextDialogueDisplayName;
        }

        dialogueBox.SetupDialogueBox(_dialogue, _displayName);
    }

    //public void StartDialogue(DialogueHolder _dialogueHolder)
    //{
    //    nextDialogue = null;
    //    nextDialogueDisplayName = "";

    //    ToggleDialogueContainer(true);

    //    if(_dialogueHolder.runEventsOnComplete != null)
    //    {
    //        runEventsOnComplete = _dialogueHolder.runEventsOnComplete;
    //    }

    //    dialogueBox.SetupDialogueBox(_dialogueHolder.dialogue, _dialogueHolder.displayName);
    //}
    #endregion

    public void EndDialogue()
    {
        ToggleDialogueContainer(false);

        if(nextDialogue != null)
        {
            StartDialogue(nextDialogue, nextDialogueDisplayName, runOnComplete, runEventsOnComplete);
        }
        else
        {
            //If runOnComplete != null, Invoke
            runOnComplete?.Invoke();
            runEventsOnComplete?.Invoke();
            if(runEventFunctionsOnComplete != null)
            {
                foreach (EventFunctionParams func in runEventFunctionsOnComplete)
                {
                    EventFinder.instance.CallFunction(func);
                }
            }

            runOnComplete = null;
            runEventsOnComplete = null;
        }
    }

    private void ToggleDialogueContainer(bool _enable)
    {
        dialogueBoxContainer.SetActive(_enable);
    }

    public void UpdateDisplay(PortraitMood _mood)
    {
        switch (_mood)
        {
            case (PortraitMood.Empty):
                portraitContainer.SetActive(false);
                nameContainer.SetActive(true);
                break;

            case (PortraitMood.None):
                portraitContainer.SetActive(false);
                nameContainer.SetActive(false);
                break;

            default:
                portraitContainer.SetActive(true);
                nameContainer.SetActive(true);
                break;
        }
    }
}
