using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    [SerializeField]
    private string placeToTp;

    public void Interact()
    {
        //If anyone is looking at my code, DO NOT DO THIS
        //                                      EVER
        //                                  I'M SERIOUS
        TRASHTeleportationManager.instance.tpToPlace(FindObjectOfType<PlayerInteract>().gameObject, placeToTp);
    }
}
