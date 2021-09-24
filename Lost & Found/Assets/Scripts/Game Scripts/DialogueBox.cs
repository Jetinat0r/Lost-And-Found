using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    private DialogueScriptableObject dialogueScriptableObject;
    private string characterName;

    private Queue<string> text = new Queue<string>();

    [SerializeField]
    private TMP_Text dialogueTextArea;
    [SerializeField]
    private TMP_Text nameTextArea;

    //TODO: Allow for multiple boxes of dialogue to be written out
    public void SetupDialogueBox(DialogueScriptableObject _dialogue, string _characterName)
    {
        dialogueScriptableObject = _dialogue;
        characterName = _characterName;

        foreach(string _text in dialogueScriptableObject.dialogueText)
        {
            text.Enqueue(_text);
        }

        //dialogueTextArea.text = dialogueScriptableObject.dialogueText[0];
        AdvanceText();
        nameTextArea.text = characterName;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AdvanceText();
        }
    }

    private void AdvanceText()
    {
        if(text.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        dialogueTextArea.text = text.Dequeue();
    }
}
