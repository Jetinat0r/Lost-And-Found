using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    private DialogueScriptableObject dialogueScriptableObject;
    private string characterName;

    private Queue<string> text = new Queue<string>();
    private Coroutine textDisplayCoroutine = null;
    private bool isTextExhausted = false;
    //storedText is for skipping text
    private string storedText;

    private readonly char[] illegalChars = {'_'};

    [SerializeField]
    private TMP_Text dialogueTextArea;
    [SerializeField]
    private TMP_Text nameTextArea;
    [SerializeField]
    private float timeBetweenChars = 0.1f;

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
        nameTextArea.text = characterName;

        storedText = text.Dequeue();
        isTextExhausted = false;
        textDisplayCoroutine = StartCoroutine(AdvanceText(storedText, timeBetweenChars));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //AdvanceText();
            if (isTextExhausted)
            {
                //If there is no more text, destroy
                //TODO: don't destroy it you idiot.
                if(text.Count == 0)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    //Start new line of text
                    storedText = text.Dequeue();
                    isTextExhausted = false;
                    textDisplayCoroutine = StartCoroutine(AdvanceText(storedText, timeBetweenChars));
                }
            }
            else
            {
                //Skip to end of current line of text
                StopCoroutine(textDisplayCoroutine);
                SkipDialogue(storedText);
            }
        }
    }

    private IEnumerator AdvanceText(string _text, float _timeBetweenChars)
    {
        dialogueTextArea.text = "";

        int i = 0;
        char[] nextTextArray = _text.ToCharArray();
        while(i < nextTextArray.Length)
        {
            string toAdd = "";
            //Used to override the _timeBetweenChars variable
            float specialTimeOut = -1;

            switch (nextTextArray[i])
            {
                //Print the next char, regardless of what it is
                case ('\\'):
                    i += 1;
                    toAdd = nextTextArray[i].ToString();
                    break;

                //Search for the next '>' and print the whole sequence
                //If no '>' exists, log a warning and continue printing
                case ('<'):
                    int j = -1;
                    for(int l = i + 1; l < nextTextArray.Length; l++)
                    {
                        if(nextTextArray[l] == '>')
                        {
                            j = l;
                            break;
                        }
                    }

                    if(j == -1)
                    {
                        Debug.LogWarning("Disconnected \'<\' in dialogue!");
                        toAdd = nextTextArray[i].ToString();
                    }
                    else
                    {
                        while(i < j)
                        {
                            toAdd += nextTextArray[i].ToString();
                            i++;
                        }
                        toAdd += nextTextArray[i].ToString();

                        specialTimeOut = 0;
                    }

                    break;

                //Use to add delay between chars or words, aka just padding.
                //If you want to have a '_' in the dialogue, for example "Xx_CoolGuy_xX" use '\' before the '_' (i.e. "Xx\_CoolGuy\_xX")
                case ('_'):
                    //Do nothing
                    break;

                //Default case, just print the next letter
                default:
                    toAdd = nextTextArray[i].ToString();
                    break;
            }



            dialogueTextArea.text += toAdd;

            i++;

            if (specialTimeOut == -1)
            {
                yield return new WaitForSeconds(_timeBetweenChars);
            }
            else
            {
                yield return new WaitForSeconds(specialTimeOut);
            }
        }

        isTextExhausted = true;

        yield return null;
    }

    private void SkipDialogue(string _text)
    {
        string newText = "";
        foreach(char c in _text.ToCharArray())
        {
            if(!(illegalChars.Contains(c)))
            {
                newText += c.ToString();
            }
        }

        isTextExhausted = true;
        dialogueTextArea.text = newText;
    }

    //IEnumerator Type()
    //{
    //    foreach (char letter in sentences[index].ToCharArray())
    //    {
    //        textDisplay.text += letter;
    //        yield return new WaitForSeconds(typingSpeed);
    //    }
    //}
}
