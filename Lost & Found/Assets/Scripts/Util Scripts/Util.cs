using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void DestroyRecursively(GameObject obj)
    {
        foreach(Transform childObj in obj.transform)
        {
            DestroyRecursively(childObj.gameObject);
        }

        Object.Destroy(obj);
    }
}
