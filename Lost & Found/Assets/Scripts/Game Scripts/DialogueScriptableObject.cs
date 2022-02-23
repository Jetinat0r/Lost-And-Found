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
    None,           //Doesn't display portrait
    Empty           //Doesn't display portrait and name
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


    public Sprite GetPortrait(PortraitMood _mood)
    {
        return portraits.GetPortrait(_mood);
    }
}
