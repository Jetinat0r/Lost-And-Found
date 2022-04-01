using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortraitMood
{
    Neutral = 0,
    Happy,
    Sad,
    Angry,
    Thoughtful,
    Surprised,
    Scheming,
    Extra1,
    Extra2,
    Extra3,
    Empty,          //Doesn't display portrait
    None            //Doesn't display portrait and name
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "ScriptableObjects/DialogueScriptableObject")]
public class DialogueScriptableObject : ScriptableObject
{
    [Header("NPC Portraits")]
    [SerializeField]
    private NpcPortraitsScriptableObject portraits;

    [Space(10)]
    [Header("Text Lines")]

    //Text separated by box
    //Keep character limit in mind (whatever that ends up being)
    [TextArea(3, 10)]
    public List<string> dialogueText;
    public float timeBetweenChars = 0.1f;

    [Space(10)]
    [Header("Portrait Moods for Text Lines")]
    public List<PortraitMood> moodsForLines;


    public DialogueScriptableObject nextDialogue = null;
    public string nextDialogueDisplayName;

    public Sprite GetPortrait(PortraitMood _mood)
    {
        return portraits.GetPortrait(_mood);
    }

    //Really just made this so that locked doors could work :P
    public void SetNpcPortraits(NpcPortraitsScriptableObject _portraits)
    {
        portraits = _portraits;
    }
}
