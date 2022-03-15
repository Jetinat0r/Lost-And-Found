using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionObject : InteractionTarget
{
    public string connectionTitle;

    public void AttemptTransition()
    {
        WorldNodeConnector _connector = SceneController.instance.GetConnectorFromTitle(connectionTitle);
        if (_connector.isLocked)
        {
            //Create new dialogue
            DialogueScriptableObject _dialogue = ScriptableObject.CreateInstance("DialogueScriptableObject") as DialogueScriptableObject;

            //Add text
            _dialogue.dialogueText = new List<string>();
            _dialogue.dialogueText.Add(_connector.lockedText);

            //Add mood
            _dialogue.moodsForLines = new List<PortraitMood>();
            _dialogue.moodsForLines.Add(PortraitMood.None);

            //"Add" portraits
            _dialogue.SetNpcPortraits(ScriptableObject.CreateInstance("NpcPortraitsScriptableObject") as NpcPortraitsScriptableObject);

            //Play Dialogue (and tell it to re-enable player movement)
            DialogueManager.instance.StartDialogue(_dialogue, "", GameManager.instance.EnablePlayerInput, itPostInteractEvents);
        }
        else
        {
            //Create a delegate
            SceneController.RunOnSceneLoad _runOnSceneLoad = null;

            //Assign methods to delegate
            //_runOnSceneLoad += GameManager.instance.EnablePlayerMovement;

            //Attempt transition
            SceneController.instance.MoveThroughConnection(connectionTitle, _runOnSceneLoad);
        }
    }
}
