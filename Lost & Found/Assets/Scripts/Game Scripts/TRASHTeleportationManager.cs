using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRASHTeleportationManager : MonoBehaviour
{
    public static TRASHTeleportationManager instance;

    public Transform classroomPosition;
    public Transform hallwayPosition;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void tpToPlace(GameObject objToTp, string placeToTp)
    {
        if(placeToTp == "classroom")
        {
            objToTp.transform.position = classroomPosition.position;
        }
        else if (placeToTp == "hallway")
        {
            objToTp.transform.position = hallwayPosition.position;
        }
        else
        {
            Debug.LogWarning("No place to tp!");
        }
    }
}
