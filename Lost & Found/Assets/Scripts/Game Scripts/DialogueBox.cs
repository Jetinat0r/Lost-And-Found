using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    private DialogueScriptableObject dialogueScriptableObject;
    private string characterName;

    [SerializeField]
    private TMP_Text dialogueTextArea;
    [SerializeField]
    private TMP_Text nameTextArea;

    //TODO: Allow for multiple boxes of dialogue to be written out
    public void SetupDialogueBox(DialogueScriptableObject _dialogue, string _characterName)
    {
        dialogueScriptableObject = _dialogue;
        characterName = _characterName;

        dialogueTextArea.text = dialogueScriptableObject.dialogueText[0];
        nameTextArea.text = characterName;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Destroy(gameObject);
        }
    }
}
