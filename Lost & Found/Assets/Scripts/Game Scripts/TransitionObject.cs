using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionObject : InteractionTarget
{
    public string connectionTitle;

    public void AttemptTransition()
    {
        //Create a delegate
        SceneController.RunOnSceneLoad _runOnSceneLoad = null;

        //Assign methods to delegate
        //_runOnSceneLoad += GameManager.instance.EnablePlayerMovement;

        //Attempt transition
        SceneController.instance.MoveThroughConnection(connectionTitle, _runOnSceneLoad);
    }
}
