using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingCamera : MonoBehaviour
{
    public static KingCamera instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        SceneManager.sceneLoaded += RemovePeasantCameras;
    }

    private void RemovePeasantCameras(Scene scene, LoadSceneMode sceneMode)
    {
        for (int i = 1; i < Camera.allCamerasCount; i++)
        {
            Camera _peasantCamera = Camera.allCameras[i];

            if (_peasantCamera.gameObject.CompareTag("MainCamera"))
            {
                DestroyImmediate(Camera.allCameras[i].gameObject);
            }
        }
    }
}
