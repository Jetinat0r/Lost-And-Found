using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 movementVector;

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        //Subscribe to events here
        
    }

    void Update()
    {
        //Input
        movementVector.x = Input.GetAxisRaw("Horizontal");
        //movementVector.y = Input.GetAxisRaw("Vertical");
    }

    //PUT THE "IF(CAN'T MOVE)" HERE
    private void FixedUpdate()
    {
        //Movement
        animator.SetFloat("horizontalSpeed", Mathf.Abs(movementVector.x));

        if(movementVector.x != 0)
        {
            transform.localScale = new Vector3(movementVector.x, transform.localScale.y, transform.localScale.z);
        }

        rb.MovePosition(rb.position + (movementVector.normalized * movementSpeed * Time.fixedDeltaTime));
    }
}
