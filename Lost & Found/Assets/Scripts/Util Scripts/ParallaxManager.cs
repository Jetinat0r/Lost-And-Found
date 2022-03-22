using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public static ParallaxManager instance;

    [SerializeField]
    private Vector2 centerPoint = Vector2.zero;
    [SerializeField]
    private float distMultiplier = 1f;

    private List<ParallaxObject> parallaxObjects = new List<ParallaxObject>();
    //Should always be set to the active camera. I don't have anything ensuring that, too bad.
    private Camera activeCamera;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            activeCamera = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        foreach(ParallaxObject _obj in parallaxObjects)
        {
            _obj.UpdatePosition(activeCamera.transform.position, -distMultiplier);
        }
    }

    public void ChangeCamera(Camera _cam)
    {
        activeCamera = _cam;
    }

    public void AddObject(ParallaxObject _obj)
    {
        parallaxObjects.Add(_obj);
    }

    public void RemoveObject(ParallaxObject _obj)
    {
        parallaxObjects.Remove(_obj);
    }
}
