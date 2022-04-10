using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JournalItemDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text displayName;
    [SerializeField]
    private TMP_Text displayDescription;

    public void SetData(string _displayName, string _displayDescription)
    {
        displayName.text = _displayName;
        displayDescription.text = _displayDescription;
    }
}
