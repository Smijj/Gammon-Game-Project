using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Vector2 input;

    public Transform movePoint;
    public LayerMask WhatStopsMovement;
    public Animator anim;

    private void Start() {
        movePoint.parent = null;
    }

    private void Update() {
        

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f) {

            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Moving on the x-axis
            if (Mathf.Abs(input.x) == 1f) {
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(input.x, 0, 0), 0.2f, WhatStopsMovement)) {
                    movePoint.position += new Vector3(input.x, 0, 0);
                }
            }
            // Moving on the y-axis
            else if (Mathf.Abs(input.y) == 1f) {       
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, input.y, 0), 0.2f, WhatStopsMovement)) {
                    movePoint.position += new Vector3(0, input.y, 0);
                }
            }

            // If the player is on the movepoint, stop movement animation
            //anim.SetBool("moving", false);

        } else {
            // If the player isnt on the movePoint, then they must be moving towards the move point thus use the walking animation.
            //anim.SetBool("moving", true);
        }
        
    }
}
