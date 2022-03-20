using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public bool removeFromParent = false;

    private void Awake()
    {
        if (removeFromParent)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
    }
}
