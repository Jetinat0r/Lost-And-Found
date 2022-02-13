using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionObject : InteractionTarget
{
    public string connectionTitle;

    public void AttemptTransition()
    {
        SceneController.instance.MoveThroughConnection(connectionTitle);
    }
}
