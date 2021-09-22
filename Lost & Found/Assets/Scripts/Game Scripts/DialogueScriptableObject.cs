using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "ScriptableObjects/DialogueScriptableObject")]
public class DialogueScriptableObject : ScriptableObject
{
    //Text separated by box
    //Keep character limit in mind (whatever that ends up being)
    public List<string> dialogueText;

    //TODO: Different text boxes?
}
