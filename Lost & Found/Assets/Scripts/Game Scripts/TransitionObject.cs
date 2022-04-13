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
            //See if the player has the items to open the door
            bool _hasReqItems = true;
            foreach(string _reqItem in _connector.reqItems)
            {
                bool _hasThisItem = false;
                foreach(QuestItemScriptableObject _heldItem in PlayerInventory.instance.curHeldItems)
                {
                    if(_reqItem == _heldItem.idItemName)
                    {
                        _hasThisItem = true;
                        break;
                    }
                }

                if (!_hasThisItem)
                {
                    _hasReqItems = false;
                    break;
                }
            }

            //Make a dialogue saying that the door can't be opened
            if (!_hasReqItems)
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
                DialogueManager.instance.StartDialogue(_dialogue, "", GameManager.instance.EnablePlayerInput, itPostInteractEvents, itPostInteractFunctionEvents);

                return;
            }
        }

        #region Start Transition
        //Create a delegate
        SceneController.RunOnSceneLoad _runOnSceneLoad = null;

        //Assign methods to delegate
        //_runOnSceneLoad += GameManager.instance.EnablePlayerMovement;

        //Attempt transition
        SceneController.instance.MoveThroughConnection(connectionTitle, _runOnSceneLoad, itPostInteractFunctionEvents);
        #endregion
    }
}
