using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPortraitSet", menuName = "ScriptableObjects/NpcPortraitsScriptableObject")]
public class NpcPortraitsScriptableObject : ScriptableObject
{
    public Sprite npcNeutral;
    public Sprite npcHappy;
    public Sprite npcSad;
    public Sprite npcAngry;
    public Sprite npcThoughtful;
    public Sprite npcSurprised;
    public Sprite npcScheming;
    public Sprite npcExtra1;
    public Sprite npcExtra2;
    public Sprite npcExtra3;

    public Sprite GetPortrait(PortraitMood _mood)
    {
        Sprite _toReturn = npcNeutral;

        switch (_mood)
        {
            case (PortraitMood.Neutral):
                _toReturn = npcNeutral;
                break;

            case (PortraitMood.Happy):
                _toReturn = npcHappy;
                break;

            case (PortraitMood.Sad):
                _toReturn = npcSad;
                break;

            case (PortraitMood.Angry):
                _toReturn = npcAngry;
                break;

            case (PortraitMood.Thoughtful):
                _toReturn = npcThoughtful;
                break;

            case (PortraitMood.Surprised):
                _toReturn = npcSurprised;
                break;

            case (PortraitMood.Scheming):
                _toReturn = npcScheming;
                break;

            case (PortraitMood.Extra1):
                _toReturn = npcExtra1;
                break;

            case (PortraitMood.Extra2):
                _toReturn = npcExtra2;
                break;

            case (PortraitMood.Extra3):
                _toReturn = npcExtra3;
                break;

            default:
                Debug.LogWarning("Invalid NPC Mood!");
                break;
        }

        return _toReturn;
    }
}
