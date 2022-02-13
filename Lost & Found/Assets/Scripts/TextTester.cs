using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTester : MonoBehaviour
{
    [SerializeField]
    private DialogueBox textBox;
    [SerializeField]
    private DialogueScriptableObject dialogue;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            textBox.SetupDialogueBox(dialogue, "Todd");
            Destroy(gameObject);
            return;
        }
    }
}
