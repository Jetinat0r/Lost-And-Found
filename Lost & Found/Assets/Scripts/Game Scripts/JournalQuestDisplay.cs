using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JournalQuestDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text displayQuestName;
    [SerializeField]
    private TMP_Text displayNpcName;
    [SerializeField]
    private TMP_Text displayDescription;

    public void SetData(string _displayQuestName, string _displayNpcName, string _displayDescription)
    {
        displayQuestName.text = _displayQuestName;
        displayNpcName.text = _displayNpcName;
        displayDescription.text = _displayDescription;
    }
}
