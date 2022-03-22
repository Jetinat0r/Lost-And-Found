using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField]
    private int layer = 0;
    [SerializeField]
    private bool parallaxX = true;
    [SerializeField]
    private bool parallaxY = false;

    private Vector2 startPos;

    private void Awake()
    {
        startPos = transform.position;
        ParallaxManager.instance.AddObject(this);
    }

    public void UpdatePosition(Vector2 _camPos, float _distMultiplier)
    {
        //Layer is negative to align with how SpriteRenderer handles layers (pos in front of the player, neg behind them)
        Vector2 _totalNewPos = startPos - ((_camPos - startPos) * -layer * _distMultiplier);

        //Ternary operator! if(parralaxX/Y) then {use that}, else {0}
        float _newX = parallaxX ? _totalNewPos.x : 0f;
        float _newY = parallaxY ? _totalNewPos.y : 0f;

        transform.position = new Vector2(_newX, _newY);
    }

    private void OnDestroy()
    {
        ParallaxManager.instance.RemoveObject(this);
    }
}
