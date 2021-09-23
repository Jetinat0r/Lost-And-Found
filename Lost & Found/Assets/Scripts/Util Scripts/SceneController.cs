using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //TODO: Move to GameManager or something
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetScene(string _sceneName)
    {
        Scene _goToScene = SceneManager.GetSceneByName(_sceneName);

        if(_goToScene != null)
        {
            SceneManager.LoadScene(_sceneName);
        }
        else
        {
            Debug.LogWarning("Scene: " + _sceneName + " not found!");
        }
    }
}
