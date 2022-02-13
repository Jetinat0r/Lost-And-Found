using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        //TODO: not this
        AudioManager.instance.Stop("Main Theme");
        AudioManager.instance.Play("Level One Loop 1");


        SceneController.instance.SetScene("Level_00");
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
